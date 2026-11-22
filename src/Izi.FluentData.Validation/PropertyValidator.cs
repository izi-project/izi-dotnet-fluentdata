using System.Globalization;

namespace Izi.FluentData.Validation;

/// <summary>Rules for one property of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TProperty">The property type, or the item type when this validator came from <c>RuleForEach</c>.</typeparam>
public sealed class PropertyValidator<T, TProperty> : IPropertyConfiguration<T>
{
    private readonly Validator<T> _owner;
    private readonly Func<T, TProperty>? _read;
    private readonly Func<T, IEnumerable<TProperty>>? _readItems;
    private readonly ParentNullCheck<T>[] _parents;
    private readonly List<BoundRule<TProperty>> _rules = [];
    private Func<T, bool>? _when;
    private IValidator<TProperty>? _child;
    private volatile ICompiledPropertyValidator<T>? _compiled;

    /// <summary>Creates a validator for one compiled path.</summary>
    /// <param name="owner">The validator this property belongs to.</param>
    /// <param name="propertyName">The member path.</param>
    /// <param name="read">Reads one value. Null when this validator enumerates items.</param>
    /// <param name="readItems">Reads the collection. Null when this validator reads one value.</param>
    /// <param name="parents">Intermediate members that can be null.</param>
    internal PropertyValidator(
        Validator<T> owner,
        string propertyName,
        Func<T, TProperty>? read,
        Func<T, IEnumerable<TProperty>>? readItems,
        ParentNullCheck<T>[] parents)
    {
        _owner = owner;
        PropertyName = propertyName;
        _read = read;
        _readItems = readItems;
        _parents = parents;
    }

    /// <summary>The name reported for this property. <see cref="WithName"/> replaces the whole path.</summary>
    public string PropertyName { get; private set; }

    /// <summary>Appends <paramref name="rule"/>.</summary>
    /// <param name="rule">The rule to run on the property value.</param>
    /// <returns>This property validator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public PropertyValidator<T, TProperty> AddRule(IValidatorRule<TProperty> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            _rules.Add(new BoundRule<TProperty>(rule));
            Invalidate();
        }

        return this;
    }

    /// <summary>Replaces the message of the last rule. The text is stored verbatim.</summary>
    /// <param name="message">The message. It does not name a property.</param>
    /// <returns>This property validator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public PropertyValidator<T, TProperty> WithMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        lock (_owner.Gate)
        {
            if (_rules.Count == 0)
            {
                throw new InvalidOperationException("WithMessage requires a rule on this property.");
            }

            _rules[^1] = _rules[^1].WithMessage(message);
            Invalidate();
        }

        return this;
    }

    /// <summary>Replaces the name used when this property is reported.</summary>
    /// <param name="name">The name to report. Parent null checks keep the member path.</param>
    /// <returns>This property validator.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null or empty.</exception>
    public PropertyValidator<T, TProperty> WithName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (name.Length == 0)
        {
            throw new ArgumentException("The property name must not be empty.", nameof(name));
        }

        lock (_owner.Gate)
        {
            PropertyName = name;
            Invalidate();
        }

        return this;
    }

    /// <summary>
    /// Runs this property only when <paramref name="predicate"/> is true.
    /// Another <c>When</c> is combined with AND. The condition covers the whole property, including the child and null-parent checks.
    /// </summary>
    /// <param name="predicate">The condition, given the object being validated.</param>
    /// <returns>This property validator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public PropertyValidator<T, TProperty> When(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        lock (_owner.Gate)
        {
            var existing = _when;
            _when = existing is null ? predicate : instance => existing(instance) && predicate(instance);
            Invalidate();
        }

        return this;
    }

    /// <summary>
    /// Validates the property value with <paramref name="validator"/>. Child names are prefixed with this property's name.
    /// A null value is skipped. The child does not run when this property's own rules fail.
    /// </summary>
    /// <param name="validator">The child validator. A later call replaces it.</param>
    /// <returns>This property validator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="validator"/> is <see langword="null"/>.</exception>
    public PropertyValidator<T, TProperty> SetValidator(IValidator<TProperty> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        lock (_owner.Gate)
        {
            _child = validator;
            Invalidate();
        }

        return this;
    }

    /// <summary>Validates this property on <paramref name="instance"/>.</summary>
    /// <param name="instance">The object that owns the property.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>
    /// Every failure from this property, including each <c>RuleForEach</c> item and each failed child property.
    /// A pass is <see cref="ValidationResult.Success"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is cancelled.</exception>
    public ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var pending = Compile().AppendAsync(instance, null, cancellationToken, 0);
        if (pending.IsCompletedSuccessfully)
        {
            return new ValueTask<ValidationResult>(ValidationRun.ToResult(pending.Result));
        }

        return FinishPropertyAsync(pending);
    }

    /// <summary>Compiles the rules currently registered. Callers may already hold the owner gate.</summary>
    /// <returns>The compiled step.</returns>
    internal ICompiledPropertyValidator<T> Compile()
    {
        var compiled = _compiled;
        if (compiled is not null)
        {
            return compiled;
        }

        lock (_owner.Gate)
        {
            compiled = _compiled;
            if (compiled is null)
            {
                var rules = _rules.ToArray();
                compiled = _readItems is null
                    ? new ImmutablePropertyValidator<T, TProperty>(PropertyName, _read!, _parents, _when, rules, _child)
                    : new ImmutableEachValidator<T, TProperty>(PropertyName, _readItems, _parents, _when, rules, _child);
                _compiled = compiled;
            }

            return compiled;
        }
    }

    /// <inheritdoc />
    ICompiledPropertyValidator<T> IPropertyConfiguration<T>.Compile() => Compile();

    private void Invalidate()
    {
        _compiled = null;
        _owner.InvalidateCompiled();
    }

    private async ValueTask<ValidationResult> FinishPropertyAsync(ValueTask<List<PropertyValidationResult>?> pending)
    {
        var failures = await pending.ConfigureAwait(false);
        return ValidationRun.ToResult(failures);
    }
}

/// <summary>Reads one property, runs its rules, then an optional child validator.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
internal sealed class ImmutablePropertyValidator<T, TProperty> : ICompiledPropertyValidator<T>
{
    private readonly string _name;
    private readonly Func<T, TProperty> _read;
    private readonly ParentNullCheck<T>[] _parents;
    private readonly Func<T, bool>? _when;
    private readonly BoundRule<TProperty>[] _rules;
    private readonly IValidator<TProperty>? _child;

    /// <summary>Creates a runner for one property.</summary>
    internal ImmutablePropertyValidator(
        string name,
        Func<T, TProperty> read,
        ParentNullCheck<T>[] parents,
        Func<T, bool>? when,
        BoundRule<TProperty>[] rules,
        IValidator<TProperty>? child)
    {
        _name = name;
        _read = read;
        _parents = parents;
        _when = when;
        _rules = rules;
        _child = child;
    }

    /// <inheritdoc />
    public ValueTask<List<PropertyValidationResult>?> AppendAsync(
        T instance,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_when is not null && !_when(instance))
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        if (ValidationRun.HasNullParent(_parents, instance, ref failures))
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        var value = _read(instance);
        var read = RulePipeline<TProperty>.Read(_rules, value, cancellationToken);
        if (!read.Completed)
        {
            return ContinueAsync(read.Pending, read.NextIndex, value, read.Messages, failures, cancellationToken, depth);
        }

        return Finish(value, read.Messages, failures, cancellationToken, depth);
    }

    private ValueTask<List<PropertyValidationResult>?> ContinueAsync(
        ValueTask<ValidationRuleResult> pending,
        int index,
        TProperty value,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
        => ContinueCoreAsync(pending, index, value, messages, failures, cancellationToken, depth);

    private async ValueTask<List<PropertyValidationResult>?> ContinueCoreAsync(
        ValueTask<ValidationRuleResult> pending,
        int index,
        TProperty value,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        messages = await RulePipeline<TProperty>.ContinueAsync(pending, _rules, index, value, messages, cancellationToken)
            .ConfigureAwait(false);
        var finished = Finish(value, messages, failures, cancellationToken, depth);
        if (finished.IsCompletedSuccessfully)
        {
            return finished.Result;
        }

        return await finished.ConfigureAwait(false);
    }

    private ValueTask<List<PropertyValidationResult>?> Finish(
        TProperty value,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        failures = ValidationRun.AddMessages(failures, _name, messages);
        if (messages is not null || _child is null || value is null)
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        var pending = ChildValidation.Run(_child, value, cancellationToken, depth);
        if (pending.IsCompletedSuccessfully)
        {
            return new ValueTask<List<PropertyValidationResult>?>(ValidationRun.Prefix(failures, _name, pending.Result));
        }

        return PrefixAsync(pending, failures);
    }

    private async ValueTask<List<PropertyValidationResult>?> PrefixAsync(
        ValueTask<ValidationResult> pending,
        List<PropertyValidationResult>? failures)
    {
        var child = await pending.ConfigureAwait(false);
        return ValidationRun.Prefix(failures, _name, child);
    }
}

/// <summary>Validates each item of a collection. Item names are <c>Items[0]</c> and are built only for a failure.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TItem">The item type.</typeparam>
internal sealed class ImmutableEachValidator<T, TItem> : ICompiledPropertyValidator<T>
{
    private readonly string _name;
    private readonly Func<T, IEnumerable<TItem>> _readItems;
    private readonly ParentNullCheck<T>[] _parents;
    private readonly Func<T, bool>? _when;
    private readonly BoundRule<TItem>[] _rules;
    private readonly IValidator<TItem>? _child;

    /// <summary>Creates a runner for each item.</summary>
    internal ImmutableEachValidator(
        string name,
        Func<T, IEnumerable<TItem>> readItems,
        ParentNullCheck<T>[] parents,
        Func<T, bool>? when,
        BoundRule<TItem>[] rules,
        IValidator<TItem>? child)
    {
        _name = name;
        _readItems = readItems;
        _parents = parents;
        _when = when;
        _rules = rules;
        _child = child;
    }

    /// <inheritdoc />
    public ValueTask<List<PropertyValidationResult>?> AppendAsync(
        T instance,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_when is not null && !_when(instance))
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        if (ValidationRun.HasNullParent(_parents, instance, ref failures))
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        var items = _readItems(instance);
        if (items is null)
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        if (items is IReadOnlyList<TItem> list)
        {
            return AppendList(list, failures, cancellationToken, depth);
        }

        return AppendEnumerable(items, failures, cancellationToken, depth);
    }

    private ValueTask<List<PropertyValidationResult>?> AppendList(
        IReadOnlyList<TItem> items,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var read = RulePipeline<TItem>.Read(_rules, item, cancellationToken);
            if (!read.Completed)
            {
                return ContinueListAsync(items, i, read.Pending, read.NextIndex, read.Messages, failures, cancellationToken, depth);
            }

            var finished = FinishItem(item, i, read.Messages, failures, cancellationToken, depth);
            if (!finished.IsCompletedSuccessfully)
            {
                return ContinueListAfterItemAsync(finished, items, i + 1, cancellationToken, depth);
            }

            failures = finished.Result;
        }

        return new ValueTask<List<PropertyValidationResult>?>(failures);
    }

    private async ValueTask<List<PropertyValidationResult>?> ContinueListAsync(
        IReadOnlyList<TItem> items,
        int index,
        ValueTask<ValidationRuleResult> pending,
        int nextRule,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        messages = await RulePipeline<TItem>.ContinueAsync(pending, _rules, nextRule, items[index], messages, cancellationToken)
            .ConfigureAwait(false);
        failures = await FinishItem(items[index], index, messages, failures, cancellationToken, depth).ConfigureAwait(false);
        for (var i = index + 1; i < items.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var read = RulePipeline<TItem>.Read(_rules, items[i], cancellationToken);
            if (!read.Completed)
            {
                messages = await RulePipeline<TItem>.ContinueAsync(
                    read.Pending, _rules, read.NextIndex, items[i], read.Messages, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                messages = read.Messages;
            }

            failures = await FinishItem(items[i], i, messages, failures, cancellationToken, depth).ConfigureAwait(false);
        }

        return failures;
    }

    private async ValueTask<List<PropertyValidationResult>?> ContinueListAfterItemAsync(
        ValueTask<List<PropertyValidationResult>?> pending,
        IReadOnlyList<TItem> items,
        int index,
        CancellationToken cancellationToken,
        int depth)
    {
        var failures = await pending.ConfigureAwait(false);
        for (var i = index; i < items.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var read = RulePipeline<TItem>.Read(_rules, items[i], cancellationToken);
            List<string>? messages;
            if (!read.Completed)
            {
                messages = await RulePipeline<TItem>.ContinueAsync(
                    read.Pending, _rules, read.NextIndex, items[i], read.Messages, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                messages = read.Messages;
            }

            failures = await FinishItem(items[i], i, messages, failures, cancellationToken, depth).ConfigureAwait(false);
        }

        return failures;
    }

    private ValueTask<List<PropertyValidationResult>?> AppendEnumerable(
        IEnumerable<TItem> items,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        var enumerator = items.GetEnumerator();
        var handedOff = false;
        try
        {
            var index = 0;
            while (enumerator.MoveNext())
            {
                var read = RulePipeline<TItem>.Read(_rules, enumerator.Current, cancellationToken);
                if (!read.Completed)
                {
                    handedOff = true;
                    return ContinueEnumerableAsync(
                        enumerator, index, read.Pending, read.NextIndex, read.Messages, failures, cancellationToken, depth);
                }

                var finished = FinishItem(enumerator.Current, index, read.Messages, failures, cancellationToken, depth);
                if (!finished.IsCompletedSuccessfully)
                {
                    handedOff = true;
                    return ContinueEnumerableAfterItemAsync(enumerator, finished, index + 1, cancellationToken, depth);
                }

                failures = finished.Result;
                index++;
            }
        }
        finally
        {
            if (!handedOff)
            {
                enumerator.Dispose();
            }
        }

        return new ValueTask<List<PropertyValidationResult>?>(failures);
    }

    private async ValueTask<List<PropertyValidationResult>?> ContinueEnumerableAsync(
        IEnumerator<TItem> enumerator,
        int index,
        ValueTask<ValidationRuleResult> pending,
        int nextRule,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        try
        {
            var current = enumerator.Current;
            messages = await RulePipeline<TItem>.ContinueAsync(pending, _rules, nextRule, current, messages, cancellationToken)
                .ConfigureAwait(false);
            failures = await FinishItem(current, index, messages, failures, cancellationToken, depth).ConfigureAwait(false);
            index++;
            while (enumerator.MoveNext())
            {
                cancellationToken.ThrowIfCancellationRequested();
                current = enumerator.Current;
                var read = RulePipeline<TItem>.Read(_rules, current, cancellationToken);
                if (!read.Completed)
                {
                    messages = await RulePipeline<TItem>.ContinueAsync(
                        read.Pending, _rules, read.NextIndex, current, read.Messages, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    messages = read.Messages;
                }

                failures = await FinishItem(current, index, messages, failures, cancellationToken, depth).ConfigureAwait(false);
                index++;
            }

            return failures;
        }
        finally
        {
            enumerator.Dispose();
        }
    }

    private async ValueTask<List<PropertyValidationResult>?> ContinueEnumerableAfterItemAsync(
        IEnumerator<TItem> enumerator,
        ValueTask<List<PropertyValidationResult>?> pending,
        int index,
        CancellationToken cancellationToken,
        int depth)
    {
        try
        {
            var failures = await pending.ConfigureAwait(false);
            while (enumerator.MoveNext())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var current = enumerator.Current;
                var read = RulePipeline<TItem>.Read(_rules, current, cancellationToken);
                List<string>? messages;
                if (!read.Completed)
                {
                    messages = await RulePipeline<TItem>.ContinueAsync(
                        read.Pending, _rules, read.NextIndex, current, read.Messages, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    messages = read.Messages;
                }

                failures = await FinishItem(current, index, messages, failures, cancellationToken, depth).ConfigureAwait(false);
                index++;
            }

            return failures;
        }
        finally
        {
            enumerator.Dispose();
        }
    }

    private ValueTask<List<PropertyValidationResult>?> FinishItem(
        TItem value,
        int index,
        List<string>? messages,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth)
    {
        if (messages is null && (_child is null || value is null))
        {
            return new ValueTask<List<PropertyValidationResult>?>(failures);
        }

        if (messages is not null)
        {
            return new ValueTask<List<PropertyValidationResult>?>(
                ValidationRun.AddMessages(failures, ItemName(index), messages));
        }

        var pending = ChildValidation.Run(_child!, value, cancellationToken, depth);
        if (pending.IsCompletedSuccessfully)
        {
            if (pending.Result.IsValid)
            {
                return new ValueTask<List<PropertyValidationResult>?>(failures);
            }

            return new ValueTask<List<PropertyValidationResult>?>(
                ValidationRun.Prefix(failures, ItemName(index), pending.Result));
        }

        return PrefixItemAsync(pending, index, failures);
    }

    private async ValueTask<List<PropertyValidationResult>?> PrefixItemAsync(
        ValueTask<ValidationResult> pending,
        int index,
        List<PropertyValidationResult>? failures)
    {
        var child = await pending.ConfigureAwait(false);
        if (child.IsValid)
        {
            return failures;
        }

        return ValidationRun.Prefix(failures, ItemName(index), child);
    }

    private string ItemName(int index)
        => string.Concat(_name, "[", index.ToString(CultureInfo.InvariantCulture), "]");
}
