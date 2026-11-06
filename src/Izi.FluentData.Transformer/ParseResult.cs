namespace Izi.FluentData.Transformer;

/// <summary>
/// The outcome of <see cref="TransformerRules.TryParse{T}"/>. Check <see cref="Success"/> before using <see cref="Value"/>:
/// failure stores <c>default</c>, which is also a legal successful value for some types.
/// </summary>
/// <typeparam name="T">The parsed type.</typeparam>
public readonly record struct ParseResult<T>(bool Success, T Value)
{
    /// <summary>A successful parse of <paramref name="value"/>.</summary>
    public static ParseResult<T> Succeeded(T value) => new(true, value);

    /// <summary>A null or invalid input. <see cref="Value"/> is <c>default</c>.</summary>
    public static ParseResult<T> Failed() => new(false, default!);
}
