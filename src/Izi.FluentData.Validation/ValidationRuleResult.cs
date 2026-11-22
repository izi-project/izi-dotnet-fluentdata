namespace Izi.FluentData.Validation;

/// <summary>The outcome of one rule. Messages do not name a property.</summary>
public sealed class ValidationRuleResult
{
    /// <summary>A rule that passed. Shared by every successful check.</summary>
    public static ValidationRuleResult Success { get; } = new(ValidationError.Empty);

    /// <summary>Creates a result for <paramref name="errors"/>.</summary>
    /// <param name="errors">The messages from the rule.</param>
    internal ValidationRuleResult(ValidationError errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    /// <summary>A failed rule with one <paramref name="message"/>.</summary>
    /// <param name="message">The message. It does not name a property.</param>
    /// <returns>The failed result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static ValidationRuleResult Failed(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return new ValidationRuleResult(new ValidationError(message));
    }

    /// <summary>A failed rule with <paramref name="errors"/>.</summary>
    /// <param name="errors">The messages. An empty error is <see cref="Success"/>.</param>
    /// <returns>The result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="errors"/> is <see langword="null"/>.</exception>
    public static ValidationRuleResult Failed(ValidationError errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return errors.Count == 0 ? Success : new ValidationRuleResult(errors);
    }

    /// <summary>True when the rule produced no messages.</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>The messages from the rule.</summary>
    public ValidationError Errors { get; }

    /// <summary>Joins the messages with a new line.</summary>
    /// <returns>The messages, one per line.</returns>
    public override string ToString() => ToString(Environment.NewLine);

    /// <summary>Joins the messages with <paramref name="separator"/>.</summary>
    /// <param name="separator">The text placed between messages.</param>
    /// <returns>The joined messages.</returns>
    public string ToString(string separator)
    {
        ArgumentNullException.ThrowIfNull(separator);
        if (Errors.Count == 0)
        {
            return string.Empty;
        }

        if (Errors.Count == 1)
        {
            return Errors[0];
        }

        return string.Join(separator, Errors);
    }
}
