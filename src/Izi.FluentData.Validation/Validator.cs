using System.Linq.Expressions;

namespace Izi.FluentData.Validation;

/// <summary>Configures and runs property rules for <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <remarks>
/// Configuration stays mutable between executions. <see cref="ValidateAsync"/> reuses a compiled runner
/// taken when the configuration last changed. A registration on another thread does not change a run
/// that is already in progress, and the next run sees the new rules. The run itself takes no lock.
/// Invalid data is reported on <see cref="ValidationResult"/>. This method throws only for a null instance,
/// a cancelled token, or a predicate that itself throws.
/// </remarks>
public sealed class Validator<T> : IValidator<T>
{
    private readonly object _gate = new();
    private readonly List<IPropertyConfiguration<T>> _steps = [];
    private volatile ImmutableValidator<T>? _compiled;

    /// <summary>Registers rules for one property.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">A property or field chain, for example <c>order =&gt; order.Address.Street</c>.</param>
    /// <returns>The property validator, so rules can be added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="property"/> is not a property or field chain.</exception>
    public PropertyValidator<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> property)
    {
        var access = ValidationAccess.Compile(property, nameof(property));
        var step = new PropertyValidator<T, TProperty>(this, access.Name, access.Read, readItems: null, access.Parents);
        Add(step);
        return step;
    }

    /// <summary>Registers rules for each item of a collection property.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="collection">A property or field chain whose value is enumerated.</param>
    /// <returns>The item validator. Failures are reported as <c>Items[0]</c>, <c>Items[1]</c>, and so on.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="collection"/> is not a property or field chain.</exception>
    public PropertyValidator<T, TItem> RuleForEach<TItem>(Expression<Func<T, IEnumerable<TItem>>> collection)
    {
        var access = ValidationAccess.Compile(collection, nameof(collection));
        var step = new PropertyValidator<T, TItem>(this, access.Name, read: null, access.Read, access.Parents);
        Add(step);
        return step;
    }

    /// <summary>Includes another validator for the same type. Its failed properties are merged in.</summary>
    /// <param name="validator">The validator to run as part of this one.</param>
    /// <returns>This validator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="validator"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="validator"/> is this validator, or including it would form a cycle.
    /// </exception>
    public Validator<T> Include(IValidator<T> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        if (ReferenceEquals(validator, this))
        {
            throw new InvalidOperationException("Validators cannot include each other in a cycle.");
        }

        // One lock serialises every Include. The cycle check takes no validator gate.
        // The gate is taken only to append the step, so two gates are never held together.
        lock (IncludeTie)
        {
            if (Reaches(validator, this))
            {
                throw new InvalidOperationException("Validators cannot include each other in a cycle.");
            }

            lock (_gate)
            {
                _steps.Add(new IncludeConfiguration(validator));
                InvalidateCompiled();
            }
        }

        return this;
    }

    private static readonly object IncludeTie = new();

    /// <summary>Validates <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">Checked before the run and between steps and rules.</param>
    /// <returns>
    /// The validation result. A fully valid run returns <see cref="ValidationResult.Success"/>.
    /// When every rule finishes without waiting, the returned <see cref="ValueTask{TResult}"/> is already completed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="instance"/> is <see langword="null"/>, including a null <see cref="Nullable{T}"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is cancelled. Cancellation is not turned into a validation message.
    /// This method may throw that exception directly instead of returning a faulted <see cref="ValueTask{TResult}"/>.
    /// </exception>
    /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">
    /// A <c>Matches</c> or <c>NotMatches</c> rule exceeded its match timeout. That is a processing failure, not a validation result.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// A nested child validator exceeded 128 levels, or the thread has insufficient stack.
    /// This may be thrown directly instead of a faulted <see cref="ValueTask{TResult}"/> when the run has not yet waited.
    /// </exception>
    public ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        // After a custom validator has run in this process, read the depth it stored. Otherwise skip AsyncLocal.
        return ValidateNested(instance, cancellationToken, ValidationDepth.ReadIfUsed());
    }

    /// <summary>Runs this validator at <paramref name="depth"/>. The caller has rejected a null instance.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">Checked before the run.</param>
    /// <param name="depth">Zero for a public call. Each child or include adds one.</param>
    /// <returns>The validation result.</returns>
    internal ValueTask<ValidationResult> ValidateNested(T instance, CancellationToken cancellationToken, int depth)
    {
        ValidationLimits.Check(depth);
        cancellationToken.ThrowIfCancellationRequested();
        return Compiled().ValidateAsync(instance, cancellationToken, depth);
    }

    private bool Reaches(IValidator<T> start, Validator<T> target)
    {
        var seen = new HashSet<Validator<T>>();
        var pending = new Stack<IValidator<T>>();
        pending.Push(start);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (ReferenceEquals(current, target))
            {
                return true;
            }

            if (current is not Validator<T> validator || !seen.Add(validator))
            {
                continue;
            }

            foreach (var included in validator.Included())
            {
                pending.Push(included);
            }
        }

        return false;
    }

    private IValidator<T>[] Included()
    {
        lock (_gate)
        {
            var included = new List<IValidator<T>>();
            for (var i = 0; i < _steps.Count; i++)
            {
                if (_steps[i] is IncludeConfiguration include)
                {
                    included.Add(include.Validator);
                }
            }

            return included.ToArray();
        }
    }

    /// <summary>Drops the cached runner. Callers already hold <see cref="Gate"/>.</summary>
    internal void InvalidateCompiled() => _compiled = null;

    /// <summary>The lock that guards configuration.</summary>
    internal object Gate => _gate;

    private void Add(IPropertyConfiguration<T> step)
    {
        lock (_gate)
        {
            _steps.Add(step);
            InvalidateCompiled();
        }
    }

    private ImmutableValidator<T> Compiled()
    {
        var compiled = _compiled;
        if (compiled is not null)
        {
            return compiled;
        }

        lock (_gate)
        {
            compiled = _compiled;
            if (compiled is null)
            {
                compiled = Compile();
                _compiled = compiled;
            }

            return compiled;
        }
    }

    private ImmutableValidator<T> Compile()
    {
        var count = _steps.Count;
        if (count == 0)
        {
            return new ImmutableValidator<T>([]);
        }

        var steps = new ICompiledPropertyValidator<T>[count];
        for (var i = 0; i < count; i++)
        {
            steps[i] = _steps[i].Compile();
        }

        return new ImmutableValidator<T>(steps);
    }

    private sealed class IncludeConfiguration : IPropertyConfiguration<T>
    {
        private readonly IValidator<T> _validator;

        internal IncludeConfiguration(IValidator<T> validator)
        {
            _validator = validator;
        }

        internal IValidator<T> Validator => _validator;

        public ICompiledPropertyValidator<T> Compile() => new IncludeStep(_validator);
    }

    private sealed class IncludeStep : ICompiledPropertyValidator<T>
    {
        private readonly IValidator<T> _validator;

        internal IncludeStep(IValidator<T> validator)
        {
            _validator = validator;
        }

        public ValueTask<List<PropertyValidationResult>?> AppendAsync(
            T instance,
            List<PropertyValidationResult>? failures,
            CancellationToken cancellationToken,
            int depth)
        {
            var pending = ChildValidation.Run(_validator, instance, cancellationToken, depth);
            if (pending.IsCompletedSuccessfully)
            {
                return new ValueTask<List<PropertyValidationResult>?>(Merge(failures, pending.Result));
            }

            return MergeAsync(pending, failures);
        }

        private static async ValueTask<List<PropertyValidationResult>?> MergeAsync(
            ValueTask<ValidationResult> pending,
            List<PropertyValidationResult>? failures)
        {
            var result = await pending.ConfigureAwait(false);
            return Merge(failures, result);
        }

        private static List<PropertyValidationResult>? Merge(
            List<PropertyValidationResult>? failures,
            ValidationResult result)
        {
            var errors = result.Errors;
            if (errors.Count == 0)
            {
                return failures;
            }

            failures ??= new List<PropertyValidationResult>(errors.Count);
            for (var i = 0; i < errors.Count; i++)
            {
                failures.Add(errors[i]);
            }

            return failures;
        }
    }
}

/// <summary>The compiled steps for one <see cref="Validator{T}"/> configuration.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal sealed class ImmutableValidator<T>
{
    private readonly ICompiledPropertyValidator<T>[] _steps;

    /// <summary>Creates a runner for <paramref name="steps"/>.</summary>
    /// <param name="steps">The compiled steps, in registration order.</param>
    internal ImmutableValidator(ICompiledPropertyValidator<T>[] steps)
    {
        _steps = steps;
    }

    /// <summary>Validates <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate. The caller has already rejected null.</param>
    /// <param name="cancellationToken">Checked between steps.</param>
    /// <param name="depth">The nesting depth. The root is zero.</param>
    /// <returns><see cref="ValidationResult.Success"/> when nothing failed.</returns>
    internal ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken, int depth)
    {
        cancellationToken.ThrowIfCancellationRequested();
        List<PropertyValidationResult>? failures = null;
        var steps = _steps;
        for (var i = 0; i < steps.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = steps[i].AppendAsync(instance, failures, cancellationToken, depth);
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueAsync(pending, steps, i + 1, instance, cancellationToken, depth);
            }

            failures = pending.Result;
        }

        return new ValueTask<ValidationResult>(ToResult(failures));
    }

    private static async ValueTask<ValidationResult> ContinueAsync(
        ValueTask<List<PropertyValidationResult>?> pending,
        ICompiledPropertyValidator<T>[] steps,
        int index,
        T instance,
        CancellationToken cancellationToken,
        int depth)
    {
        var failures = await pending.ConfigureAwait(false);
        for (; index < steps.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            failures = await steps[index].AppendAsync(instance, failures, cancellationToken, depth).ConfigureAwait(false);
        }

        return ToResult(failures);
    }

    private static ValidationResult ToResult(List<PropertyValidationResult>? failures)
        => failures is null ? ValidationResult.Success : new ValidationResult(failures);
}
