using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation;

/// <summary>
/// Base class for validators: subclass it and declare rules with <see cref="RuleFor{TProperty}"/> in the
/// constructor. Rules chain directly off the returned builder, e.g.
/// <c>RuleFor(x =&gt; x.Name).NotNull().MaxLength(50)</c>; use <c>RuleFor(x =&gt; x)</c> to validate the
/// whole instance (cross-field checks).
/// </summary>
/// <typeparam name="T">The type whose instances are validated.</typeparam>
public class Validator<T> : IValidator<T>
{
    // Shared, immutable result for the valid path so success allocates nothing.
    private static readonly IReadOnlyList<string> NoErrors = [];

    // Each entry extracts a value and runs its rule builder, erasing TProperty.
    private readonly List<Func<T, CancellationToken, ValueTask<IReadOnlyList<string>>>> _propertyValidators = [];

    /// <summary>
    /// Begins configuring rules for the value returned by <paramref name="selector"/>. Chain rules onto
    /// the returned builder; pass <c>x =&gt; x</c> to validate the whole instance.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="selector">Selects the value to validate, e.g. <c>x =&gt; x.Name</c>.</param>
    /// <returns>A builder to chain rules onto.</returns>
    public ValidatorRuleBuilder<TProperty> RuleFor<TProperty>(Func<T, TProperty> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var builder = new ValidatorRuleBuilder<TProperty>();

        // The fluent chain (e.g. .NotNull().MaxLength(50)) appends rules to the builder *after* this method
        // returns, so the collection can only be built once configuration has finished. Build it lazily on the
        // first validation and cache it, so we don't allocate a fresh collection on every call. A concurrent
        // first call may build twice; the result is equivalent and reference assignment is atomic, so it's benign.
        CompositeValidatorRule<TProperty>? rules = null;
        _propertyValidators.Add((instance, ct) => (rules ??= builder.Build()).ValidateAsync(selector(instance), ct));

        return builder;
    }

    /// <summary>Runs every configured rule against <paramref name="instance"/> and aggregates the resulting errors.</summary>
    public ValueTask<IReadOnlyList<string>> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var validators = _propertyValidators;
        if (validators.Count == 0)
        {
            return ValueTask.FromResult(NoErrors);
        }

        List<string>? errors = null;

        for (int i = 0; i < validators.Count; i++)
        {
            var pending = validators[i](instance, cancellationToken);

            if (pending.IsCompletedSuccessfully)
            {
                var result = pending.Result;
                if (result.Count > 0)
                {
                    (errors ??= []).AddRange(result);
                }
            }
            else
            {
                return ValidateSlowAsync(validators, i, pending, errors, instance, cancellationToken);
            }
        }

        return ValueTask.FromResult(errors ?? NoErrors);
    }

    /// <summary>Resumes the validator loop asynchronously once one has yielded.</summary>
    private static async ValueTask<IReadOnlyList<string>> ValidateSlowAsync(List<Func<T, CancellationToken, ValueTask<IReadOnlyList<string>>>> validators, int currentIndex, ValueTask<IReadOnlyList<string>> currentPendingTask, List<string>? errors, T instance, CancellationToken cancellationToken)
    {
        var currentResult = await currentPendingTask;
        if (currentResult.Count > 0)
        {
            (errors ??= []).AddRange(currentResult);
        }

        for (int i = currentIndex + 1; i < validators.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await validators[i](instance, cancellationToken);
            if (result.Count > 0)
            {
                (errors ??= []).AddRange(result);
            }
        }

        return errors ?? NoErrors;
    }
}
