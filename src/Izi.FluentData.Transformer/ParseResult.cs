namespace Izi.FluentData.Transformer;

/// <summary>
/// The outcome of <see cref="TransformerRules.TryParse{T}"/>.
/// Check <see cref="Success"/> before using <see cref="Value"/>:
/// failure stores <c>default</c>, which is also a legal successful value for some types.
/// </summary>
/// <typeparam name="T">The parsed type.</typeparam>
/// <param name="Success"><see langword="true"/> when the text parsed.</param>
/// <param name="Value">
/// The parsed value, or <c>default</c> when <paramref name="Success"/> is <see langword="false"/>.
/// </param>
public readonly record struct ParseResult<T>(bool Success, T Value)
{
    /// <summary>Creates a successful parse of <paramref name="value"/>.</summary>
    /// <param name="value">The parsed value.</param>
    /// <returns> A result with <see cref="Success"/> set and <see cref="Value"/> equal to
    /// <paramref name="value"/>.</returns>
    public static ParseResult<T> Succeeded(T value) => new(true, value);

    /// <summary>Creates a failed parse. <see cref="Value"/> is <c>default</c>.</summary>
    /// <returns>A result whose <see cref="Success"/> is <see langword="false"/>.</returns>
    public static ParseResult<T> Failed() => new(false, default!);
}
