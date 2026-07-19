namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// The atomic unit of validation: a predicate over a <typeparamref name="T"/> value plus the single message
/// reported when it fails.
/// </summary>
/// <typeparam name="T">
/// The type of value this rule validates. Contravariant (<see langword="in"/>) so a rule for a base type can
/// stand in for a rule over a derived type.
/// </typeparam>
public interface IValidatorRule<in T>
{
    /// <summary>Evaluates the rule against <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The failure message, or <see langword="null"/> when the value is valid.</returns>
    ValueTask<string?> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
