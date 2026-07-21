using System.Collections;
using System.Globalization;
using System.Numerics;

namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// Factory methods for the built-in transformation steps (<see cref="TransformerRule{T}"/>) used to compose
/// a pipeline. String rules coalesce a <see langword="null"/> source to <see cref="string.Empty"/>; numeric
/// rules build on .NET generic math.
/// </summary>
public static partial class TransformerRules
{
    // =============================
    // String Transforms (string -> string)
    //
    // All string rules coalesce a null source to an empty string so the rest of the
    // pipeline never has to guard against null.
    // =============================

    /// <summary>Creates a step that removes leading and trailing whitespace.</summary>
    public static TransformerRule<string> Trim() => new((source, _) => ValueTask.FromResult(source?.Trim() ?? string.Empty));

    /// <summary>Creates a step that removes leading whitespace.</summary>
    public static TransformerRule<string> TrimStart() => new((source, _) => ValueTask.FromResult(source?.TrimStart() ?? string.Empty));

    /// <summary>Creates a step that removes trailing whitespace.</summary>
    public static TransformerRule<string> TrimEnd() => new((source, _) => ValueTask.FromResult(source?.TrimEnd() ?? string.Empty));

    /// <summary>Creates a step that upper-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRule<string> ToUpper(CultureInfo culture) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).ToUpper(culture)));

    /// <summary>Creates a step that upper-cases the value using the invariant culture.</summary>
    public static TransformerRule<string> ToUpper() => ToUpper(CultureInfo.InvariantCulture);

    /// <summary>Creates a step that lower-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRule<string> ToLower(CultureInfo culture) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).ToLower(culture)));

    /// <summary>Creates a step that lower-cases the value using the invariant culture.</summary>
    public static TransformerRule<string> ToLower() => ToLower(CultureInfo.InvariantCulture);

    /// <summary>Creates a step that title-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRule<string> ToTitleCase(CultureInfo culture) => new((source, _) => ValueTask.FromResult(culture.TextInfo.ToTitleCase(source ?? string.Empty)));

    /// <summary>Creates a step that title-cases the value using the invariant culture.</summary>
    public static TransformerRule<string> ToTitleCase() => ToTitleCase(CultureInfo.InvariantCulture);

    /// <summary>Creates a step that replaces every occurrence of <paramref name="oldValue"/> with <paramref name="newValue"/>.</summary>
    public static TransformerRule<string> Replace(string oldValue, string newValue) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).Replace(oldValue, newValue)));

    /// <summary>Creates a step that extracts <paramref name="length"/> characters starting at <paramref name="startIndex"/>.</summary>
    public static TransformerRule<string> Substring(int startIndex, int length) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).Substring(startIndex, length)));

    /// <summary>Creates a step that extracts the substring from <paramref name="startIndex"/> to the end.</summary>
    public static TransformerRule<string> Substring(int startIndex) => new((source, _) => ValueTask.FromResult((source ?? string.Empty)[startIndex..]));

    /// <summary>Creates a step that caps the value at <paramref name="maxLength"/> characters (empty when the source is null/empty or <paramref name="maxLength"/> is negative).</summary>
    public static TransformerRule<string> Truncate(int maxLength) => new((source, _) => ValueTask.FromResult(string.IsNullOrEmpty(source) || maxLength < 0 ? string.Empty : source.Length <= maxLength ? source : source[..maxLength]));

    /// <summary>Creates a step that left-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static TransformerRule<string> PadLeft(int totalWidth, char paddingChar) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).PadLeft(totalWidth, paddingChar)));

    /// <summary>Creates a step that right-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static TransformerRule<string> PadRight(int totalWidth, char paddingChar) => new((source, _) => ValueTask.FromResult((source ?? string.Empty).PadRight(totalWidth, paddingChar)));

    /// <summary>Creates a step that prepends <paramref name="prefix"/> to the value.</summary>
    public static TransformerRule<string> Prepend(string prefix) => new((source, _) => ValueTask.FromResult((prefix ?? string.Empty) + (source ?? string.Empty)));

    /// <summary>Creates a step that appends <paramref name="suffix"/> to the value.</summary>
    public static TransformerRule<string> Append(string suffix) => new((source, _) => ValueTask.FromResult((source ?? string.Empty) + (suffix ?? string.Empty)));

    // =============================
    // Numeric Transforms
    // =============================

    /// <summary>Creates a step that adds <paramref name="value"/> to the current number.</summary>
    public static TransformerRule<TNumber> Add<TNumber>(TNumber value) where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(source + value));

    /// <summary>Creates a step that subtracts <paramref name="value"/> from the current number.</summary>
    public static TransformerRule<TNumber> Subtract<TNumber>(TNumber value) where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(source - value));

    /// <summary>Creates a step that multiplies the current number by <paramref name="value"/>.</summary>
    public static TransformerRule<TNumber> Multiply<TNumber>(TNumber value) where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(source * value));

    /// <summary>Creates a step that divides the current number by <paramref name="value"/>.</summary>
    public static TransformerRule<TNumber> Divide<TNumber>(TNumber value) where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(source / value));

    /// <summary>Creates a step that takes the absolute value of the current number.</summary>
    public static TransformerRule<TNumber> Abs<TNumber>() where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Abs(source)));

    /// <summary>Creates a step that clamps the current number to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<TNumber> Clamp<TNumber>(TNumber min, TNumber max) where TNumber : INumber<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Clamp(source, min, max)));

    /// <summary>Creates a step that negates the current number (unary minus).</summary>
    public static TransformerRule<TNumber> Invert<TNumber>() where TNumber : INumberBase<TNumber> => new((source, _) => ValueTask.FromResult(-source));

    // ---- Rounding (floating-point only) ----

    /// <summary>Creates a step that rounds the current number to the nearest integral value.</summary>
    public static TransformerRule<TNumber> Round<TNumber>() where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Round(source)));

    /// <summary>Creates a step that rounds the current number to <paramref name="digits"/> fractional digits.</summary>
    public static TransformerRule<TNumber> Round<TNumber>(int digits) where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Round(source, digits)));

    /// <summary>Creates a step that rounds the current number to the nearest integral value using <paramref name="mode"/>.</summary>
    public static TransformerRule<TNumber> Round<TNumber>(MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Round(source, mode)));

    /// <summary>Creates a step that rounds the current number to <paramref name="digits"/> fractional digits using <paramref name="mode"/>.</summary>
    public static TransformerRule<TNumber> Round<TNumber>(int digits, MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Round(source, digits, mode)));

    /// <summary>Creates a step that rounds the current number down to the nearest integral value.</summary>
    public static TransformerRule<TNumber> Floor<TNumber>() where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Floor(source)));

    /// <summary>Creates a step that rounds the current number up to the nearest integral value.</summary>
    public static TransformerRule<TNumber> Ceiling<TNumber>() where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Ceiling(source)));

    /// <summary>Creates a step that truncates the current number toward zero.</summary>
    public static TransformerRule<TNumber> Truncate<TNumber>() where TNumber : IFloatingPoint<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Truncate(source)));

    /// <summary>Creates a step that maps the current number from <c>[<paramref name="min"/>, <paramref name="max"/>]</c> into the <c>[0, 1]</c> range as <c>(source - min) / (max - min)</c>.</summary>
    public static TransformerRule<TNumber> Normalize<TNumber>(TNumber min, TNumber max) where TNumber : INumber<TNumber> => new((source, _) => ValueTask.FromResult((source - min) / (max - min)));

    // ---- Roots / Powers ----

    /// <summary>Creates a step that takes the square root of the current number.</summary>
    public static TransformerRule<TNumber> Sqrt<TNumber>() where TNumber : IRootFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Sqrt(source)));

    /// <summary>Creates a step that takes the cube root of the current number.</summary>
    public static TransformerRule<TNumber> Cbrt<TNumber>() where TNumber : IRootFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Cbrt(source)));

    /// <summary>Creates a step that takes the <paramref name="n"/>-th root of the current number.</summary>
    public static TransformerRule<TNumber> RootN<TNumber>(int n) where TNumber : IRootFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.RootN(source, n)));

    /// <summary>Creates a step that raises the current number to the power <paramref name="exponent"/>.</summary>
    public static TransformerRule<TNumber> Pow<TNumber>(TNumber exponent) where TNumber : IPowerFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Pow(source, exponent)));

    // ---- Exponential ----

    /// <summary>Creates a step that computes <c>e</c> raised to the current number.</summary>
    public static TransformerRule<TNumber> Exp<TNumber>() where TNumber : IExponentialFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Exp(source)));

    /// <summary>Creates a step that computes <c>2</c> raised to the current number.</summary>
    public static TransformerRule<TNumber> Exp2<TNumber>() where TNumber : IExponentialFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Exp2(source)));

    /// <summary>Creates a step that computes <c>10</c> raised to the current number.</summary>
    public static TransformerRule<TNumber> Exp10<TNumber>() where TNumber : IExponentialFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Exp10(source)));

    // ---- Logarithmic ----

    /// <summary>Creates a step that takes the natural (base-<c>e</c>) logarithm of the current number.</summary>
    public static TransformerRule<TNumber> Log<TNumber>() where TNumber : ILogarithmicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Log(source)));

    /// <summary>Creates a step that takes the logarithm of the current number in base <paramref name="newBase"/>.</summary>
    public static TransformerRule<TNumber> Log<TNumber>(TNumber newBase) where TNumber : ILogarithmicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Log(source, newBase)));

    /// <summary>Creates a step that takes the base-<c>2</c> logarithm of the current number.</summary>
    public static TransformerRule<TNumber> Log2<TNumber>() where TNumber : ILogarithmicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Log2(source)));

    /// <summary>Creates a step that takes the base-<c>10</c> logarithm of the current number.</summary>
    public static TransformerRule<TNumber> Log10<TNumber>() where TNumber : ILogarithmicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Log10(source)));

    // ---- Trigonometric (radians) ----

    /// <summary>Creates a step that computes the sine of the current number (in radians).</summary>
    public static TransformerRule<TNumber> Sin<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Sin(source)));

    /// <summary>Creates a step that computes the cosine of the current number (in radians).</summary>
    public static TransformerRule<TNumber> Cos<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Cos(source)));

    /// <summary>Creates a step that computes the tangent of the current number (in radians).</summary>
    public static TransformerRule<TNumber> Tan<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Tan(source)));

    /// <summary>Creates a step that computes the arcsine (in radians) of the current number.</summary>
    public static TransformerRule<TNumber> Asin<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Asin(source)));

    /// <summary>Creates a step that computes the arccosine (in radians) of the current number.</summary>
    public static TransformerRule<TNumber> Acos<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Acos(source)));

    /// <summary>Creates a step that computes the arctangent (in radians) of the current number.</summary>
    public static TransformerRule<TNumber> Atan<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Atan(source)));

    /// <summary>Creates a step that converts the current number from degrees to radians.</summary>
    public static TransformerRule<TNumber> DegreesToRadians<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.DegreesToRadians(source)));

    /// <summary>Creates a step that converts the current number from radians to degrees.</summary>
    public static TransformerRule<TNumber> RadiansToDegrees<TNumber>() where TNumber : ITrigonometricFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.RadiansToDegrees(source)));

    // ---- Hyperbolic ----

    /// <summary>Creates a step that computes the hyperbolic sine of the current number.</summary>
    public static TransformerRule<TNumber> Sinh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Sinh(source)));

    /// <summary>Creates a step that computes the hyperbolic cosine of the current number.</summary>
    public static TransformerRule<TNumber> Cosh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Cosh(source)));

    /// <summary>Creates a step that computes the hyperbolic tangent of the current number.</summary>
    public static TransformerRule<TNumber> Tanh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Tanh(source)));

    /// <summary>Creates a step that computes the inverse hyperbolic sine of the current number.</summary>
    public static TransformerRule<TNumber> Asinh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Asinh(source)));

    /// <summary>Creates a step that computes the inverse hyperbolic cosine of the current number.</summary>
    public static TransformerRule<TNumber> Acosh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Acosh(source)));

    /// <summary>Creates a step that computes the inverse hyperbolic tangent of the current number.</summary>
    public static TransformerRule<TNumber> Atanh<TNumber>() where TNumber : IHyperbolicFunctions<TNumber> => new((source, _) => ValueTask.FromResult(TNumber.Atanh(source)));

    // =============================
    // Default Value
    // =============================

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> whenever <paramref name="predicate"/> matches the current value.</summary>
    public static TransformerRule<TSource> DefaultIf<TSource>(TSource defaultValue, Func<TSource, bool> predicate) => new((source, _) => ValueTask.FromResult(predicate(source) ? defaultValue : source));

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see langword="null"/>.</summary>
    public static TransformerRule<TSource> DefaultIfNull<TSource>(TSource defaultValue) => DefaultIf(defaultValue, source => source is null);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is null, an empty string, or an empty collection/sequence.</summary>
    public static TransformerRule<TSource> DefaultIfEmpty<TSource>(TSource defaultValue) => DefaultIf(defaultValue, source =>
    {
        if (source is null) return true;
        if (source is string str) return string.IsNullOrEmpty(str);
        if (source is ICollection collection) return collection.Count == 0;
        if (source is IEnumerable enumerable) return !enumerable.GetEnumerator().MoveNext();
        return false;
    });

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current string is null, empty, or whitespace.</summary>
    public static TransformerRule<string> DefaultIfNullOrWhitespace(string defaultValue) => DefaultIf(defaultValue, source => string.IsNullOrWhiteSpace(source));

    public static TransformerRule<T> DefaultIfMinValue<T>(T defaultValue) where T : IComparable<T>, IMinMaxValue<T> => DefaultIfMinValue(T.MinValue, defaultValue);
    public static TransformerRule<T> DefaultIfMaxValue<T>(T defaultValue) where T : IComparable<T>, IMinMaxValue<T> => DefaultIfMaxValue(T.MaxValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is at or below <paramref name="minimum"/>. Works for any comparable type (dates, times, numbers, strings). Pass <paramref name="minimum"/> as the default to floor the value instead.</summary>
    public static TransformerRule<T> DefaultIfMinValue<T>(T minimum, T defaultValue) where T : IComparable<T> => DefaultIf(defaultValue, source => source is not null && source.CompareTo(minimum) <= 0);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is at or above <paramref name="maximum"/>. Works for any comparable type (dates, times, numbers, strings). Pass <paramref name="maximum"/> as the default to cap the value instead.</summary>
    public static TransformerRule<T> DefaultIfMaxValue<T>(T maximum, T defaultValue) where T : IComparable<T> => DefaultIf(defaultValue, source => source is not null && source.CompareTo(maximum) >= 0);
}