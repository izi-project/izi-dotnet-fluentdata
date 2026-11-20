namespace Izi.FluentData.Validation;

/// <summary>One check for a value of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The value type the rule reads.</typeparam>
public interface IValidatorRule<in T>
{
    /// <summary>Checks <paramref name="value"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="cancellationToken">Checked while the rule runs.</param>
    /// <returns>The rule result. Its messages do not name a property.</returns>
    ValueTask<ValidationRuleResult> ValidateAsync(T value, CancellationToken cancellationToken = default);
}
