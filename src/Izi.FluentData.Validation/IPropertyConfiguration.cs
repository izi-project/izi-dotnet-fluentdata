namespace Izi.FluentData.Validation;

/// <summary>Mutable configuration for one property on <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal interface IPropertyConfiguration<T>
{
    /// <summary>Compiles the current property rules.</summary>
    /// <returns>The compiled property validator.</returns>
    ICompiledPropertyValidator<T> Compile();
}
