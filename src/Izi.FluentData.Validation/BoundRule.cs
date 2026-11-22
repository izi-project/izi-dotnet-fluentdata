namespace Izi.FluentData.Validation;

/// <summary>One rule plus the optional message supplied by <c>WithMessage</c>.</summary>
/// <typeparam name="T">The value type the rule reads.</typeparam>
internal sealed class BoundRule<T>
{
    private readonly IValidatorRule<T> _rule;
    private string? _message;

    /// <summary>Wraps <paramref name="rule"/>.</summary>
    /// <param name="rule">The rule to run.</param>
    /// <param name="message">The replacement message, or null to keep the rule's own message.</param>
    internal BoundRule(IValidatorRule<T> rule, string? message = null)
    {
        _rule = rule;
        _message = message;
    }

    /// <summary>The replacement message, or null to keep the rule's own message.</summary>
    internal string? Message => _message;

    /// <summary>Returns a new binding. A run that already compiled the previous binding keeps the old message.</summary>
    /// <param name="message">The message, used verbatim.</param>
    /// <returns>The new binding.</returns>
    internal BoundRule<T> WithMessage(string message) => new(_rule, message);

    /// <summary>Runs the rule.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="cancellationToken">Passed to the rule.</param>
    /// <returns>The rule result.</returns>
    internal ValueTask<ValidationRuleResult> ValidateAsync(T value, CancellationToken cancellationToken)
        => _rule.ValidateAsync(value, cancellationToken);
}
