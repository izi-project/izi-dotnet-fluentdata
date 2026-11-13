namespace Izi.FluentData.Transformer;

/// <summary>
/// Same-type rules for one property. <see cref="Convert{TOut}"/> switches the value to another type
/// and must be finished with <see cref="PropertyConversion{T, TIn, TOut}.Into"/>.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public sealed class PropertyRule<T, TProperty> : ITransformerStep<T>
{
    private readonly Transformer<T> _owner;
    private readonly Func<T, TProperty> _read;
    private readonly Func<T, TProperty, T> _write;
    private readonly List<ITransformerRule<TProperty, TProperty>> _rules = [];
    private IPendingConversion<T, TProperty>? _conversion;

    internal PropertyRule(
        Transformer<T> owner,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write)
    {
        _owner = owner;
        _read = read;
        _write = write;
    }

    /// <summary>Appends a same-type rule. Further calls stay on this property, ahead of steps registered later.</summary>
    /// <param name="rule">The rule to run on the property value.</param>
    /// <returns>This property rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><see cref="Convert{TOut}"/> was already called.</exception>
    public PropertyRule<T, TProperty> Add(ITransformerRule<TProperty, TProperty> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            if (_conversion is not null)
            {
                throw new InvalidOperationException(
                    "Add preprocesses the property before Convert. This property already has a conversion.");
            }

            _rules.Add(rule);
            _owner.InvalidateSnapshot();
        }

        return this;
    }

    /// <summary>Appends a same-type delegate, wrapped in a <see cref="TransformerRule{TIn, TOut}"/>.</summary>
    /// <param name="transform">The function that maps the property value.</param>
    /// <returns>This property rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public PropertyRule<T, TProperty> Add(Func<TProperty, CancellationToken, ValueTask<TProperty>> transform)
    {
        return Add(new TransformerRule<TProperty, TProperty>(transform));
    }

    /// <summary>
    /// Converts the property value. The same-type rules already added become the conversion input and are
    /// not written back. <see cref="PropertyConversion{T, TIn, TOut}.Into"/> must record the destination
    /// before <see cref="Transformer{T}.TransformAsync"/>; otherwise that call throws
    /// <see cref="InvalidOperationException"/> and no step runs.
    /// </summary>
    /// <typeparam name="TOut">The converted type.</typeparam>
    /// <param name="rule">The conversion.</param>
    /// <returns>The conversion chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">This property was already converted.</exception>
    public PropertyConversion<T, TProperty, TOut> Convert<TOut>(ITransformerRule<TProperty, TOut> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            if (_conversion is not null)
            {
                throw new InvalidOperationException("Convert was already called for this property.");
            }

            var conversion = new PropertyConversion<T, TProperty, TOut>(
                _owner,
                this,
                (value, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return rule.TransformAsync(value, token);
                });
            _conversion = conversion;
            _owner.InvalidateSnapshot();
            return conversion;
        }
    }

    internal IPendingConversion<T, TProperty>? CurrentConversion => _conversion;

    internal void ReplaceConversion(IPendingConversion<T, TProperty> conversion)
    {
        _conversion = conversion;
        _owner.InvalidateSnapshot();
    }

    /// <summary>Copies the rules configured at this moment.</summary>
    /// <returns>A step that does not observe later <see cref="Add(ITransformerRule{TProperty, TProperty})"/> calls.</returns>
    internal ITransformerStep<T> Snapshot()
    {
        var rules = _rules.ToArray();
        if (_conversion is not null)
        {
            return _conversion.Snapshot(_read, rules);
        }

        return new SameTypeStep(_read, _write, rules);
    }

    private sealed class SameTypeStep : ITransformerStep<T>
    {
        private readonly Func<T, TProperty> _read;
        private readonly Func<T, TProperty, T> _write;
        private readonly ITransformerRule<TProperty, TProperty>[] _rules;

        public SameTypeStep(
            Func<T, TProperty> read,
            Func<T, TProperty, T> write,
            ITransformerRule<TProperty, TProperty>[] rules)
        {
            _read = read;
            _write = write;
            _rules = rules;
        }

        public ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
            => ApplyRules(instance, _read, _write, _rules, cancellationToken);

        public ITransformerStep<T> Snapshot() => this;
    }

    private static ValueTask<T> ApplyRules(
        T instance,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write,
        ITransformerRule<TProperty, TProperty>[] rules,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = read(instance);
        for (var i = 0; i < rules.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = rules[i].TransformAsync(value, cancellationToken);
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueRulesAsync(pending, rules, i + 1, instance, write, cancellationToken);
            }

            value = pending.Result;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<T>(write(instance, value));
    }

    private static async ValueTask<T> ContinueRulesAsync(
        ValueTask<TProperty> pending,
        ITransformerRule<TProperty, TProperty>[] rules,
        int index,
        T instance,
        Func<T, TProperty, T> write,
        CancellationToken cancellationToken)
    {
        var value = await pending.ConfigureAwait(false);
        for (; index < rules.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            value = await rules[index].TransformAsync(value, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return write(instance, value);
    }

    ValueTask<T> ITransformerStep<T>.ApplyAsync(T instance, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("A property rule must be snapshotted before it runs.");
    }

    ITransformerStep<T> ITransformerStep<T>.Snapshot()
    {
        return Snapshot();
    }

    /// <summary>
    /// Converts the property with <see cref="TransformerRules.Map{TIn, TOut}"/>.
    /// Same-type maps stay on <see cref="TransformerRuleExtensions.Map{T, TProperty}(PropertyRule{T, TProperty}, Func{TProperty, TProperty})"/>.
    /// </summary>
    /// <typeparam name="TOut">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the property value.</param>
    /// <returns>The conversion, so <see cref="PropertyConversion{T, TIn, TOut}.Into"/> can store it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TProperty, TOut> MapTo<TOut>(Func<TProperty, TOut> map)
        => Convert(TransformerRules.Map(map));

    /// <summary>
    /// Converts the property with <see cref="TransformerRules.MapAsync{TIn, TOut}"/>.
    /// </summary>
    /// <typeparam name="TOut">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the property value.</param>
    /// <returns>The conversion, so <see cref="PropertyConversion{T, TIn, TOut}.Into"/> can store it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TProperty, TOut> MapToAsync<TOut>(
        Func<TProperty, CancellationToken, ValueTask<TOut>> map)
        => Convert(TransformerRules.MapAsync(map));

}
