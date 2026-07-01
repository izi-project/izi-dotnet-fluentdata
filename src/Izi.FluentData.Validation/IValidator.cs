namespace Izi.FluentData.Validation;

/// <summary>
/// Validates an instance of <typeparamref name="T"/>, returning the messages for any rules
/// that failed.
/// </summary>
/// <typeparam name="T">The type whose instances are validated.</typeparam>
public interface IValidator<T>
{
    /// <summary>Runs every configured rule against <paramref name="instance"/>.</summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The error messages for failed rules; an empty list means valid.</returns>
    ValueTask<IReadOnlyList<string>> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
