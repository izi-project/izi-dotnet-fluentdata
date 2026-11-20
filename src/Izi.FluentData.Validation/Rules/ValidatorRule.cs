namespace Izi.FluentData.Validation;

/// <summary>A rule backed by a predicate and a fixed message.</summary>
/// <typeparam name="T">The value type the rule reads.</typeparam>
public sealed class ValidatorRule<T> : IValidatorRule<T>
{
    /// <summary>Creates a synchronous rule.</summary>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the value fails. It does not name a property.</param>
    public ValidatorRule(Func<T, bool> predicate, string message)
    {
        throw new NotImplementedException();
    }

    /// <summary>Creates an asynchronous rule.</summary>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the value fails. It does not name a property.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> predicate, string message)
    {
        throw new NotImplementedException();
    }

    /// <summary>Checks <paramref name="value"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="cancellationToken">Checked while the rule runs.</param>
    /// <returns>The rule result.</returns>
    public ValueTask<ValidationRuleResult> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
