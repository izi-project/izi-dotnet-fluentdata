using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation;

/// <summary>
/// Base class for validators: subclass it and, in the constructor, add rules directly via
/// <see cref="AddRule"/>/the fluent extensions in <c>ValidatorExtensions</c>, and/or declare per-property rules
/// with <see cref="RuleFor{TProperty}"/>. Use <c>RuleFor(x =&gt; x)</c> to validate the whole instance
/// (cross-field checks).
/// </summary>
/// <remarks>
/// A validator has two tiers that run in order: its own <see cref="AddRule"/> rules first, then its
/// <see cref="WithDependents(IValidator{T}[])"/> dependents (which includes every <see cref="RuleFor{TProperty}"/>
/// declaration) — but only if every one of its own rules passed. Because <see cref="RuleFor{TProperty}"/> rules
/// live on their own nested validator, they don't gate each other: a failing <c>Name</c> rule never skips the
/// <c>Age</c> rule. Mixing <see cref="AddRule"/> directly on the same validator that also uses
/// <see cref="RuleFor{TProperty}"/> is a sharp edge: a failing whole-instance <see cref="AddRule"/> rule skips
/// <em>every</em> <see cref="RuleFor{TProperty}"/> dependent, not just a related one.
/// </remarks>
/// <typeparam name="T">The type whose instances are validated.</typeparam>
public class Validator<T> : IValidator<T>
{
    // Shared, immutable result so the passing path (no errors) allocates nothing.
    private readonly static IReadOnlyList<string> EmptyMessages = [];

    private readonly List<IValidatorRule<T>> _rules = [];
    private readonly List<IValidator<T>> _dependents = [];

    /// <summary>Adds one or more rules that run directly against this validator's own value.</summary>
    /// <param name="rules">The rules to append, evaluated in order.</param>
    /// <returns>The same validator, for chaining.</returns>
    public Validator<T> AddRule(params IValidatorRule<T>[] rules)
    {
        _rules.AddRange(rules);
        return this;
    }

    /// <summary>Attaches pre-built dependent validators that run only once every rule above has passed.</summary>
    /// <param name="dependents">The dependent validators to append.</param>
    /// <returns>The same validator, for chaining.</returns>
    public Validator<T> WithDependents(params IValidator<T>[] dependents)
    {
        _dependents.AddRange(dependents);
        return this;
    }

    /// <summary>
    /// Creates a nested <see cref="Validator{T}"/>, configures it via <paramref name="configure"/>, and attaches
    /// it as a dependent that runs only once every rule above has passed.
    /// </summary>
    /// <param name="configure">Callback that declares rules on the new nested validator.</param>
    /// <returns>The same validator, for chaining.</returns>
    public Validator<T> WithDependents(Action<Validator<T>> configure)
    {
        var dependent = new Validator<T>();
        configure(dependent);
        return WithDependents(dependent);
    }

    /// <summary>
    /// Declares rules for the value returned by <paramref name="propertySelector"/>. A fresh
    /// <see cref="Validator{TProperty}"/> is created, wired as a dependent of this validator (so it only runs
    /// once this validator's own rules pass), and returned so rules can be chained directly onto it, e.g.
    /// <c>RuleFor(x =&gt; x.Name).NotNull().MaxLength(50)</c>. Pass <c>x =&gt; x</c> to validate the whole
    /// instance (cross-field checks).
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertySelector">Selects the value to validate, e.g. <c>x =&gt; x.Name</c>.</param>
    /// <returns>The nested validator to chain rules onto.</returns>
public Validator<TProperty> RuleFor<TProperty>(Func<T, TProperty> propertySelector)
{
    ArgumentNullException.ThrowIfNull(propertySelector);
    var dependent = new Validator<TProperty>();
    WithDependents(new PropertyAdapter<TProperty>(propertySelector, dependent));
    return dependent;
}

    // Bridges a Validator<TProperty> into the parent's IValidator<T> dependents list by extracting
    // the property value on each call; this is what lets RuleFor erase TProperty back to T.
    private sealed class PropertyAdapter<TProperty>(Func<T, TProperty> propertySelector, IValidator<TProperty> validator) : IValidator<T>
    {
        public ValueTask<IReadOnlyList<string>> ValidateAsync(T instance, CancellationToken cancellationToken = default) =>
            validator.ValidateAsync(propertySelector(instance), cancellationToken);
    }

    /// <summary>
    /// Runs this validator's own rules, then — only if all of them passed — its dependents (including every
    /// <see cref="RuleFor{TProperty}"/> declaration), and aggregates every failure message.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregated failure messages; an empty list means valid.</returns>
    public ValueTask<IReadOnlyList<string>> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        List<string>? messages = null;

        for (int i = 0; i < _rules.Count; i++)
        {
            var pending = _rules[i].ValidateAsync(instance, cancellationToken);
            if (pending.IsCompletedSuccessfully)
            {
                var result = pending.Result;
                if (result is not null)
                {
                    (messages ??= []).Add(result);
                }
            }
            else
            {
                return ValidateRulesSlowAsync(pending, i, messages, instance, cancellationToken);
            }
        }

        // Dependents only run once every rule above has passed — validating a dependent
        // against a value that already failed its own rules is redundant at best, and
        // unsafe at worst if the dependent assumes the value is well-formed.
        if (messages is not null)
        {
            return ValueTask.FromResult((IReadOnlyList<string>)messages);
        }

        for (int i = 0; i < _dependents.Count; i++)
        {
            var pending = _dependents[i].ValidateAsync(instance, cancellationToken);
            if (pending.IsCompletedSuccessfully)
            {
                var result = pending.Result;
                if (result.Count > 0)
                {
                    (messages ??= []).AddRange(result);
                }
            }
            else
            {
                return ValidateDependentsSlowAsync(pending, i, messages, instance, cancellationToken);
            }
        }

        return ValueTask.FromResult(messages ?? EmptyMessages);
    }

    // Async tail resumed once a rule yields; finishes awaiting the current one, drains the rest, then
    // falls through to the dependents loop (mirroring the synchronous path above).
    private async ValueTask<IReadOnlyList<string>> ValidateRulesSlowAsync(ValueTask<string?> pending, int currentRuleIndex, List<string>? messages, T instance, CancellationToken cancellationToken)
    {
        var result = await pending.ConfigureAwait(false);
        if (result is not null)
        {
            (messages ??= []).Add(result);
        }

        for (int i = currentRuleIndex + 1; i < _rules.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result = await _rules[i].ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
            if (result is not null)
            {
                (messages ??= []).Add(result);
            }
        }

        if (messages is not null)
        {
            return messages;
        }

        foreach (var dependent in _dependents)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dependentMessages = await dependent.ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
            if (dependentMessages.Count > 0)
            {
                (messages ??= []).AddRange(dependentMessages);
            }
        }

        return messages ?? EmptyMessages;
    }

    // Async tail resumed once a dependent yields; finishes awaiting the current one then drains the rest.
    private async ValueTask<IReadOnlyList<string>> ValidateDependentsSlowAsync(ValueTask<IReadOnlyList<string>> pending, int currentDependentIndex, List<string>? messages, T instance, CancellationToken cancellationToken)
    {
        var result = await pending.ConfigureAwait(false);
        if (result.Count > 0)
        {
            (messages ??= []).AddRange(result);
        }

        for (int i = currentDependentIndex + 1; i < _dependents.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result = await _dependents[i].ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
            if (result.Count > 0)
            {
                (messages ??= []).AddRange(result);
            }
        }

        return messages ?? EmptyMessages;
    }
}
