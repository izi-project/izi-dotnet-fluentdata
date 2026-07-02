namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// A fluent builder that accumulates <see cref="ValidatorRule{T}"/> instances for a single value of type
/// <typeparamref name="T"/>. Because every rule shares the same type, the builder is mutable and each method
/// returns the same instance, so rules and their refinements chain directly:
/// <c>builder.NotNull().MaxLength(50).WithMessage("…")</c>. The most recently added rule is the implicit
/// target for <c>WithMessage</c>/<c>WithDependent</c>. The built-in rules are exposed as extension methods
/// (see <c>ValidatorRuleBuilderExtensions</c>).
/// </summary>
/// <typeparam name="T">The type of value the accumulated rules validate.</typeparam>
public class ValidatorRuleBuilder<T>
{
    private readonly List<ValidatorRule<T>> _rules = [];
    // The last rule added; the target that WithMessage/WithDependent refinements apply to.
    private ValidatorRule<T>? _current;


    /// <summary>Adds a rule from a predicate.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <returns>The same builder, for chaining.</returns>
    public ValidatorRuleBuilder<T> AddRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc) => AddRule(new ValidatorRule<T>(evaluateFunc));
    /// <summary>Adds a rule from a predicate and a failure message.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message reported on failure.</param>
    /// <returns>The same builder, for chaining.</returns>
    public ValidatorRuleBuilder<T> AddRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, string message) => AddRule(new ValidatorRule<T>(evaluateFunc, message));
    /// <summary>Adds a rule from a predicate and several failure messages.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="messages">The messages reported on failure.</param>
    /// <returns>The same builder, for chaining.</returns>
    public ValidatorRuleBuilder<T> AddRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, IEnumerable<string> messages) => AddRule(new ValidatorRule<T>(evaluateFunc, messages));
    /// <summary>Adds a pre-built rule and makes it the target for subsequent refinements.</summary>
    /// <param name="rule">The rule to append.</param>
    /// <returns>The same builder, for chaining.</returns>
    public ValidatorRuleBuilder<T> AddRule(ValidatorRule<T> rule)
    {
        _rules.Add(rule);
        _current = rule;
        return this;
    }


    /// <summary>Appends a failure message to the most recently added rule.</summary>
    /// <param name="message">The message to append.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithMessage(string message)
    {
        if (_current is null) throw new InvalidOperationException("No rule to add message to. Call AddRule first.");
        _current.WithMessage(message);
        return this;
    }

    /// <summary>Appends several failure messages to the most recently added rule.</summary>
    /// <param name="messages">The messages to append.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithMessages(IEnumerable<string> messages)
    {
        if (_current is null) throw new InvalidOperationException("No rule to add messages to. Call AddRule first.");
        _current.WithMessages(messages);
        return this;
    }


    /// <summary>Attaches a dependent rule (built from a predicate) to the most recently added rule.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithDependent(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, string message)
    {
        return WithDependent(new ValidatorRule<T>(evaluateFunc, message));
    }

    /// <summary>Attaches a dependent rule (built from a predicate) to the most recently added rule.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithDependent(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, IEnumerable<string> messages)
    {
        return WithDependent(new ValidatorRule<T>(evaluateFunc, messages));
    }

    /// <summary>Attaches a dependent rule to the most recently added rule (runs only if the parent passes).</summary>
    /// <param name="dependent">The dependent rule.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithDependent(ValidatorRule<T> dependent)
    {
        if (_current is null) throw new InvalidOperationException("No rule to add dependent to. Call AddRule first.");
        _current.WithDependent(dependent);
        return this;
    }

    /// <summary>Attaches several dependent rules to the most recently added rule.</summary>
    /// <param name="dependents">The dependent rules.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">No rule has been added yet.</exception>
    public ValidatorRuleBuilder<T> WithDependents(IEnumerable<ValidatorRule<T>> dependents)
    {
        if (_current is null) throw new InvalidOperationException("No rule to add dependents to. Call AddRule first.");
        _current.WithDependents(dependents);
        return this;
    }


    /// <summary>Finalises the accumulated rules into a single composite rule.</summary>
    /// <returns>
    /// A <see cref="CompositeValidatorRule{T}"/> over the accumulated rules. When none were added, an
    /// always-passing rule is substituted so the result is still a usable (never-failing) validator.
    /// </returns>
    public CompositeValidatorRule<T> Build()
    {
        return _rules.Count == 0 ?
            new CompositeValidatorRule<T>(new ValidatorRule<T>((_, _) => new ValueTask<bool>(true))) :
            new CompositeValidatorRule<T>(_rules);
    }
}
