namespace Izi.FluentData.Validation;

/// <summary>A compiled property validator for <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal interface ICompiledPropertyValidator<T>
{
    /// <summary>Validates one property of <paramref name="instance"/>.</summary>
    /// <param name="instance">The object that owns the property.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The property result.</returns>
    ValueTask<PropertyValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken);
}
