namespace Izi.FluentData.Validation;

/// <summary>The outcome of one rule. Messages do not name a property.</summary>
public sealed class ValidationRuleResult
{
    /// <summary>A rule that passed.</summary>
    public static ValidationRuleResult Success => throw new NotImplementedException();

    /// <summary>Creates a result for <paramref name="errors"/>.</summary>
    /// <param name="errors">The messages from the rule.</param>
    internal ValidationRuleResult(ValidationError errors)
    {
        _ = errors;
        throw new NotImplementedException();
    }

    /// <summary>True when the rule produced no messages.</summary>
    public bool IsValid => throw new NotImplementedException();

    /// <summary>The messages from the rule.</summary>
    public ValidationError Errors => throw new NotImplementedException();

    /// <summary>Joins the messages with a new line.</summary>
    /// <returns>The messages, one per line.</returns>
    public override string ToString() => throw new NotImplementedException();

    /// <summary>Joins the messages with <paramref name="separator"/>.</summary>
    /// <param name="separator">The text placed between messages.</param>
    /// <returns>The joined messages.</returns>
    public string ToString(string separator)
    {
        _ = separator;
        throw new NotImplementedException();
    }
}
