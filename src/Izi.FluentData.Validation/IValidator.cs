namespace Izi.FluentData.Validation;

/// <summary>Validates an instance of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type being validated.</typeparam>
public interface IValidator<in T>
{
    /// <summary>Validates <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The validation result.</returns>
    ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
