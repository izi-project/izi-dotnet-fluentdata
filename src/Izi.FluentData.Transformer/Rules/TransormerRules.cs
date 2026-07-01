using System.Collections;
using System.Globalization;
using System.Numerics;

namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// Factory methods for the built-in transformation steps (<see cref="TransformerRule{TSource}"/> and
/// <see cref="TransformerRule{TSource, TDestination}"/>) used to compose a pipeline. String rules coalesce
/// a <see langword="null"/> source to <see cref="string.Empty"/>; numeric rules build on .NET generic
/// math; primitive conversions delegate to <see cref="System.Convert"/> using the invariant culture.
/// </summary>
public static class Rules
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

    /// <summary>Creates a step that yields the sign of the current number as <c>-1</c>, <c>0</c>, or <c>+1</c> (changing the running type to <see cref="int"/>).</summary>
    public static TransformerRule<TNumber, int> Sign<TNumber>() where TNumber : INumber<TNumber> => Convert<TNumber, int>((source, _) => ValueTask.FromResult(TNumber.Sign(source)));

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

    // =============================
    // Generic Transforms (TSource -> TDestination)
    // =============================

    /// <summary>Creates a step from an arbitrary (optionally asynchronous) <paramref name="converter"/>, changing the running type from <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.</summary>
    public static TransformerRule<TSource, TDestination> Convert<TSource, TDestination>(Func<TSource, CancellationToken, ValueTask<TDestination>> converter) => new(converter);

    // =============================
    // Primitive Conversions (TSource -> TDestination)
    //
    // Full matrix between bool, the integral/floating-point types, char, DateTime and string.
    // Each {Source}To{Target} factory delegates to System.Convert; string-facing conversions use
    // the invariant culture. Pairs System.Convert cannot perform (e.g. char <-> bool, any numeric
    // <-> DateTime) compile but throw InvalidCastException at runtime.
    // =============================
    // ---- Boolean -> * ----
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>byte</c>.</summary>
    public static TransformerRule<bool, byte> BooleanToByte() => Convert<bool, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<bool, sbyte> BooleanToSByte() => Convert<bool, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>short</c>.</summary>
    public static TransformerRule<bool, short> BooleanToInt16() => Convert<bool, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<bool, ushort> BooleanToUInt16() => Convert<bool, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>int</c>.</summary>
    public static TransformerRule<bool, int> BooleanToInt32() => Convert<bool, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>uint</c>.</summary>
    public static TransformerRule<bool, uint> BooleanToUInt32() => Convert<bool, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>long</c>.</summary>
    public static TransformerRule<bool, long> BooleanToInt64() => Convert<bool, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<bool, ulong> BooleanToUInt64() => Convert<bool, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>float</c>.</summary>
    public static TransformerRule<bool, float> BooleanToSingle() => Convert<bool, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>double</c>.</summary>
    public static TransformerRule<bool, double> BooleanToDouble() => Convert<bool, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<bool, decimal> BooleanToDecimal() => Convert<bool, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>char</c>.</summary>
    public static TransformerRule<bool, char> BooleanToChar() => Convert<bool, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<bool, DateTime> BooleanToDateTime() => Convert<bool, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>bool</c> value to <c>string</c>.</summary>
    public static TransformerRule<bool, string> BooleanToString() => Convert<bool, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Byte -> * ----
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>bool</c>.</summary>
    public static TransformerRule<byte, bool> ByteToBoolean() => Convert<byte, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<byte, sbyte> ByteToSByte() => Convert<byte, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>short</c>.</summary>
    public static TransformerRule<byte, short> ByteToInt16() => Convert<byte, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<byte, ushort> ByteToUInt16() => Convert<byte, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>int</c>.</summary>
    public static TransformerRule<byte, int> ByteToInt32() => Convert<byte, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>uint</c>.</summary>
    public static TransformerRule<byte, uint> ByteToUInt32() => Convert<byte, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>long</c>.</summary>
    public static TransformerRule<byte, long> ByteToInt64() => Convert<byte, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<byte, ulong> ByteToUInt64() => Convert<byte, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>float</c>.</summary>
    public static TransformerRule<byte, float> ByteToSingle() => Convert<byte, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>double</c>.</summary>
    public static TransformerRule<byte, double> ByteToDouble() => Convert<byte, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<byte, decimal> ByteToDecimal() => Convert<byte, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>char</c>.</summary>
    public static TransformerRule<byte, char> ByteToChar() => Convert<byte, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<byte, DateTime> ByteToDateTime() => Convert<byte, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>byte</c> value to <c>string</c>.</summary>
    public static TransformerRule<byte, string> ByteToString() => Convert<byte, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- SByte -> * ----
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>bool</c>.</summary>
    public static TransformerRule<sbyte, bool> SByteToBoolean() => Convert<sbyte, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>byte</c>.</summary>
    public static TransformerRule<sbyte, byte> SByteToByte() => Convert<sbyte, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>short</c>.</summary>
    public static TransformerRule<sbyte, short> SByteToInt16() => Convert<sbyte, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<sbyte, ushort> SByteToUInt16() => Convert<sbyte, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>int</c>.</summary>
    public static TransformerRule<sbyte, int> SByteToInt32() => Convert<sbyte, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>uint</c>.</summary>
    public static TransformerRule<sbyte, uint> SByteToUInt32() => Convert<sbyte, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>long</c>.</summary>
    public static TransformerRule<sbyte, long> SByteToInt64() => Convert<sbyte, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<sbyte, ulong> SByteToUInt64() => Convert<sbyte, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>float</c>.</summary>
    public static TransformerRule<sbyte, float> SByteToSingle() => Convert<sbyte, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>double</c>.</summary>
    public static TransformerRule<sbyte, double> SByteToDouble() => Convert<sbyte, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<sbyte, decimal> SByteToDecimal() => Convert<sbyte, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>char</c>.</summary>
    public static TransformerRule<sbyte, char> SByteToChar() => Convert<sbyte, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<sbyte, DateTime> SByteToDateTime() => Convert<sbyte, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>sbyte</c> value to <c>string</c>.</summary>
    public static TransformerRule<sbyte, string> SByteToString() => Convert<sbyte, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Int16 -> * ----
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>bool</c>.</summary>
    public static TransformerRule<short, bool> Int16ToBoolean() => Convert<short, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>byte</c>.</summary>
    public static TransformerRule<short, byte> Int16ToByte() => Convert<short, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<short, sbyte> Int16ToSByte() => Convert<short, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<short, ushort> Int16ToUInt16() => Convert<short, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>int</c>.</summary>
    public static TransformerRule<short, int> Int16ToInt32() => Convert<short, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>uint</c>.</summary>
    public static TransformerRule<short, uint> Int16ToUInt32() => Convert<short, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>long</c>.</summary>
    public static TransformerRule<short, long> Int16ToInt64() => Convert<short, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<short, ulong> Int16ToUInt64() => Convert<short, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>float</c>.</summary>
    public static TransformerRule<short, float> Int16ToSingle() => Convert<short, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>double</c>.</summary>
    public static TransformerRule<short, double> Int16ToDouble() => Convert<short, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<short, decimal> Int16ToDecimal() => Convert<short, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>char</c>.</summary>
    public static TransformerRule<short, char> Int16ToChar() => Convert<short, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<short, DateTime> Int16ToDateTime() => Convert<short, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>short</c> value to <c>string</c>.</summary>
    public static TransformerRule<short, string> Int16ToString() => Convert<short, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- UInt16 -> * ----
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>bool</c>.</summary>
    public static TransformerRule<ushort, bool> UInt16ToBoolean() => Convert<ushort, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>byte</c>.</summary>
    public static TransformerRule<ushort, byte> UInt16ToByte() => Convert<ushort, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<ushort, sbyte> UInt16ToSByte() => Convert<ushort, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>short</c>.</summary>
    public static TransformerRule<ushort, short> UInt16ToInt16() => Convert<ushort, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>int</c>.</summary>
    public static TransformerRule<ushort, int> UInt16ToInt32() => Convert<ushort, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>uint</c>.</summary>
    public static TransformerRule<ushort, uint> UInt16ToUInt32() => Convert<ushort, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>long</c>.</summary>
    public static TransformerRule<ushort, long> UInt16ToInt64() => Convert<ushort, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<ushort, ulong> UInt16ToUInt64() => Convert<ushort, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>float</c>.</summary>
    public static TransformerRule<ushort, float> UInt16ToSingle() => Convert<ushort, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>double</c>.</summary>
    public static TransformerRule<ushort, double> UInt16ToDouble() => Convert<ushort, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<ushort, decimal> UInt16ToDecimal() => Convert<ushort, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>char</c>.</summary>
    public static TransformerRule<ushort, char> UInt16ToChar() => Convert<ushort, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<ushort, DateTime> UInt16ToDateTime() => Convert<ushort, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>ushort</c> value to <c>string</c>.</summary>
    public static TransformerRule<ushort, string> UInt16ToString() => Convert<ushort, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Int32 -> * ----
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>bool</c>.</summary>
    public static TransformerRule<int, bool> Int32ToBoolean() => Convert<int, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>byte</c>.</summary>
    public static TransformerRule<int, byte> Int32ToByte() => Convert<int, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<int, sbyte> Int32ToSByte() => Convert<int, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>short</c>.</summary>
    public static TransformerRule<int, short> Int32ToInt16() => Convert<int, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<int, ushort> Int32ToUInt16() => Convert<int, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>uint</c>.</summary>
    public static TransformerRule<int, uint> Int32ToUInt32() => Convert<int, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>long</c>.</summary>
    public static TransformerRule<int, long> Int32ToInt64() => Convert<int, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<int, ulong> Int32ToUInt64() => Convert<int, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>float</c>.</summary>
    public static TransformerRule<int, float> Int32ToSingle() => Convert<int, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>double</c>.</summary>
    public static TransformerRule<int, double> Int32ToDouble() => Convert<int, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<int, decimal> Int32ToDecimal() => Convert<int, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>char</c>.</summary>
    public static TransformerRule<int, char> Int32ToChar() => Convert<int, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<int, DateTime> Int32ToDateTime() => Convert<int, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>int</c> value to <c>string</c>.</summary>
    public static TransformerRule<int, string> Int32ToString() => Convert<int, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- UInt32 -> * ----
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>bool</c>.</summary>
    public static TransformerRule<uint, bool> UInt32ToBoolean() => Convert<uint, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>byte</c>.</summary>
    public static TransformerRule<uint, byte> UInt32ToByte() => Convert<uint, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<uint, sbyte> UInt32ToSByte() => Convert<uint, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>short</c>.</summary>
    public static TransformerRule<uint, short> UInt32ToInt16() => Convert<uint, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<uint, ushort> UInt32ToUInt16() => Convert<uint, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>int</c>.</summary>
    public static TransformerRule<uint, int> UInt32ToInt32() => Convert<uint, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>long</c>.</summary>
    public static TransformerRule<uint, long> UInt32ToInt64() => Convert<uint, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<uint, ulong> UInt32ToUInt64() => Convert<uint, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>float</c>.</summary>
    public static TransformerRule<uint, float> UInt32ToSingle() => Convert<uint, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>double</c>.</summary>
    public static TransformerRule<uint, double> UInt32ToDouble() => Convert<uint, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<uint, decimal> UInt32ToDecimal() => Convert<uint, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>char</c>.</summary>
    public static TransformerRule<uint, char> UInt32ToChar() => Convert<uint, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<uint, DateTime> UInt32ToDateTime() => Convert<uint, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>uint</c> value to <c>string</c>.</summary>
    public static TransformerRule<uint, string> UInt32ToString() => Convert<uint, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Int64 -> * ----
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>bool</c>.</summary>
    public static TransformerRule<long, bool> Int64ToBoolean() => Convert<long, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>byte</c>.</summary>
    public static TransformerRule<long, byte> Int64ToByte() => Convert<long, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<long, sbyte> Int64ToSByte() => Convert<long, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>short</c>.</summary>
    public static TransformerRule<long, short> Int64ToInt16() => Convert<long, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<long, ushort> Int64ToUInt16() => Convert<long, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>int</c>.</summary>
    public static TransformerRule<long, int> Int64ToInt32() => Convert<long, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>uint</c>.</summary>
    public static TransformerRule<long, uint> Int64ToUInt32() => Convert<long, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<long, ulong> Int64ToUInt64() => Convert<long, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>float</c>.</summary>
    public static TransformerRule<long, float> Int64ToSingle() => Convert<long, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>double</c>.</summary>
    public static TransformerRule<long, double> Int64ToDouble() => Convert<long, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<long, decimal> Int64ToDecimal() => Convert<long, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>char</c>.</summary>
    public static TransformerRule<long, char> Int64ToChar() => Convert<long, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<long, DateTime> Int64ToDateTime() => Convert<long, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>long</c> value to <c>string</c>.</summary>
    public static TransformerRule<long, string> Int64ToString() => Convert<long, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- UInt64 -> * ----
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>bool</c>.</summary>
    public static TransformerRule<ulong, bool> UInt64ToBoolean() => Convert<ulong, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>byte</c>.</summary>
    public static TransformerRule<ulong, byte> UInt64ToByte() => Convert<ulong, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<ulong, sbyte> UInt64ToSByte() => Convert<ulong, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>short</c>.</summary>
    public static TransformerRule<ulong, short> UInt64ToInt16() => Convert<ulong, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<ulong, ushort> UInt64ToUInt16() => Convert<ulong, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>int</c>.</summary>
    public static TransformerRule<ulong, int> UInt64ToInt32() => Convert<ulong, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>uint</c>.</summary>
    public static TransformerRule<ulong, uint> UInt64ToUInt32() => Convert<ulong, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>long</c>.</summary>
    public static TransformerRule<ulong, long> UInt64ToInt64() => Convert<ulong, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>float</c>.</summary>
    public static TransformerRule<ulong, float> UInt64ToSingle() => Convert<ulong, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>double</c>.</summary>
    public static TransformerRule<ulong, double> UInt64ToDouble() => Convert<ulong, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<ulong, decimal> UInt64ToDecimal() => Convert<ulong, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>char</c>.</summary>
    public static TransformerRule<ulong, char> UInt64ToChar() => Convert<ulong, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<ulong, DateTime> UInt64ToDateTime() => Convert<ulong, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>ulong</c> value to <c>string</c>.</summary>
    public static TransformerRule<ulong, string> UInt64ToString() => Convert<ulong, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Single -> * ----
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>bool</c>.</summary>
    public static TransformerRule<float, bool> SingleToBoolean() => Convert<float, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>byte</c>.</summary>
    public static TransformerRule<float, byte> SingleToByte() => Convert<float, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<float, sbyte> SingleToSByte() => Convert<float, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>short</c>.</summary>
    public static TransformerRule<float, short> SingleToInt16() => Convert<float, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<float, ushort> SingleToUInt16() => Convert<float, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>int</c>.</summary>
    public static TransformerRule<float, int> SingleToInt32() => Convert<float, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>uint</c>.</summary>
    public static TransformerRule<float, uint> SingleToUInt32() => Convert<float, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>long</c>.</summary>
    public static TransformerRule<float, long> SingleToInt64() => Convert<float, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<float, ulong> SingleToUInt64() => Convert<float, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>double</c>.</summary>
    public static TransformerRule<float, double> SingleToDouble() => Convert<float, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<float, decimal> SingleToDecimal() => Convert<float, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>char</c>.</summary>
    public static TransformerRule<float, char> SingleToChar() => Convert<float, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<float, DateTime> SingleToDateTime() => Convert<float, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>float</c> value to <c>string</c>.</summary>
    public static TransformerRule<float, string> SingleToString() => Convert<float, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Double -> * ----
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>bool</c>.</summary>
    public static TransformerRule<double, bool> DoubleToBoolean() => Convert<double, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>byte</c>.</summary>
    public static TransformerRule<double, byte> DoubleToByte() => Convert<double, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<double, sbyte> DoubleToSByte() => Convert<double, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>short</c>.</summary>
    public static TransformerRule<double, short> DoubleToInt16() => Convert<double, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<double, ushort> DoubleToUInt16() => Convert<double, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>int</c>.</summary>
    public static TransformerRule<double, int> DoubleToInt32() => Convert<double, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>uint</c>.</summary>
    public static TransformerRule<double, uint> DoubleToUInt32() => Convert<double, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>long</c>.</summary>
    public static TransformerRule<double, long> DoubleToInt64() => Convert<double, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<double, ulong> DoubleToUInt64() => Convert<double, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>float</c>.</summary>
    public static TransformerRule<double, float> DoubleToSingle() => Convert<double, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<double, decimal> DoubleToDecimal() => Convert<double, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>char</c>.</summary>
    public static TransformerRule<double, char> DoubleToChar() => Convert<double, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<double, DateTime> DoubleToDateTime() => Convert<double, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>double</c> value to <c>string</c>.</summary>
    public static TransformerRule<double, string> DoubleToString() => Convert<double, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Decimal -> * ----
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>bool</c>.</summary>
    public static TransformerRule<decimal, bool> DecimalToBoolean() => Convert<decimal, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>byte</c>.</summary>
    public static TransformerRule<decimal, byte> DecimalToByte() => Convert<decimal, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<decimal, sbyte> DecimalToSByte() => Convert<decimal, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>short</c>.</summary>
    public static TransformerRule<decimal, short> DecimalToInt16() => Convert<decimal, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<decimal, ushort> DecimalToUInt16() => Convert<decimal, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>int</c>.</summary>
    public static TransformerRule<decimal, int> DecimalToInt32() => Convert<decimal, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>uint</c>.</summary>
    public static TransformerRule<decimal, uint> DecimalToUInt32() => Convert<decimal, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>long</c>.</summary>
    public static TransformerRule<decimal, long> DecimalToInt64() => Convert<decimal, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<decimal, ulong> DecimalToUInt64() => Convert<decimal, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>float</c>.</summary>
    public static TransformerRule<decimal, float> DecimalToSingle() => Convert<decimal, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>double</c>.</summary>
    public static TransformerRule<decimal, double> DecimalToDouble() => Convert<decimal, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>char</c>.</summary>
    public static TransformerRule<decimal, char> DecimalToChar() => Convert<decimal, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<decimal, DateTime> DecimalToDateTime() => Convert<decimal, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>decimal</c> value to <c>string</c>.</summary>
    public static TransformerRule<decimal, string> DecimalToString() => Convert<decimal, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- Char -> * ----
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>bool</c>.</summary>
    public static TransformerRule<char, bool> CharToBoolean() => Convert<char, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>byte</c>.</summary>
    public static TransformerRule<char, byte> CharToByte() => Convert<char, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<char, sbyte> CharToSByte() => Convert<char, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>short</c>.</summary>
    public static TransformerRule<char, short> CharToInt16() => Convert<char, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<char, ushort> CharToUInt16() => Convert<char, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>int</c>.</summary>
    public static TransformerRule<char, int> CharToInt32() => Convert<char, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>uint</c>.</summary>
    public static TransformerRule<char, uint> CharToUInt32() => Convert<char, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>long</c>.</summary>
    public static TransformerRule<char, long> CharToInt64() => Convert<char, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<char, ulong> CharToUInt64() => Convert<char, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>float</c>.</summary>
    public static TransformerRule<char, float> CharToSingle() => Convert<char, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>double</c>.</summary>
    public static TransformerRule<char, double> CharToDouble() => Convert<char, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<char, decimal> CharToDecimal() => Convert<char, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<char, DateTime> CharToDateTime() => Convert<char, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source)));
    /// <summary>Creates a step that converts the current <c>char</c> value to <c>string</c>.</summary>
    public static TransformerRule<char, string> CharToString() => Convert<char, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- DateTime -> * ----
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>bool</c>.</summary>
    public static TransformerRule<DateTime, bool> DateTimeToBoolean() => Convert<DateTime, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>byte</c>.</summary>
    public static TransformerRule<DateTime, byte> DateTimeToByte() => Convert<DateTime, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<DateTime, sbyte> DateTimeToSByte() => Convert<DateTime, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>short</c>.</summary>
    public static TransformerRule<DateTime, short> DateTimeToInt16() => Convert<DateTime, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<DateTime, ushort> DateTimeToUInt16() => Convert<DateTime, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>int</c>.</summary>
    public static TransformerRule<DateTime, int> DateTimeToInt32() => Convert<DateTime, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>uint</c>.</summary>
    public static TransformerRule<DateTime, uint> DateTimeToUInt32() => Convert<DateTime, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>long</c>.</summary>
    public static TransformerRule<DateTime, long> DateTimeToInt64() => Convert<DateTime, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<DateTime, ulong> DateTimeToUInt64() => Convert<DateTime, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>float</c>.</summary>
    public static TransformerRule<DateTime, float> DateTimeToSingle() => Convert<DateTime, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>double</c>.</summary>
    public static TransformerRule<DateTime, double> DateTimeToDouble() => Convert<DateTime, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<DateTime, decimal> DateTimeToDecimal() => Convert<DateTime, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>char</c>.</summary>
    public static TransformerRule<DateTime, char> DateTimeToChar() => Convert<DateTime, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source)));
    /// <summary>Creates a step that converts the current <c>DateTime</c> value to <c>string</c>.</summary>
    public static TransformerRule<DateTime, string> DateTimeToString() => Convert<DateTime, string>((source, _) => ValueTask.FromResult(System.Convert.ToString(source, CultureInfo.InvariantCulture) ?? string.Empty));

    // ---- String -> * ----
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>bool</c>.</summary>
    public static TransformerRule<string, bool> StringToBoolean() => Convert<string, bool>((source, _) => ValueTask.FromResult(System.Convert.ToBoolean(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>byte</c>.</summary>
    public static TransformerRule<string, byte> StringToByte() => Convert<string, byte>((source, _) => ValueTask.FromResult(System.Convert.ToByte(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>sbyte</c>.</summary>
    public static TransformerRule<string, sbyte> StringToSByte() => Convert<string, sbyte>((source, _) => ValueTask.FromResult(System.Convert.ToSByte(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>short</c>.</summary>
    public static TransformerRule<string, short> StringToInt16() => Convert<string, short>((source, _) => ValueTask.FromResult(System.Convert.ToInt16(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>ushort</c>.</summary>
    public static TransformerRule<string, ushort> StringToUInt16() => Convert<string, ushort>((source, _) => ValueTask.FromResult(System.Convert.ToUInt16(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>int</c>.</summary>
    public static TransformerRule<string, int> StringToInt32() => Convert<string, int>((source, _) => ValueTask.FromResult(System.Convert.ToInt32(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>uint</c>.</summary>
    public static TransformerRule<string, uint> StringToUInt32() => Convert<string, uint>((source, _) => ValueTask.FromResult(System.Convert.ToUInt32(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>long</c>.</summary>
    public static TransformerRule<string, long> StringToInt64() => Convert<string, long>((source, _) => ValueTask.FromResult(System.Convert.ToInt64(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>ulong</c>.</summary>
    public static TransformerRule<string, ulong> StringToUInt64() => Convert<string, ulong>((source, _) => ValueTask.FromResult(System.Convert.ToUInt64(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>float</c>.</summary>
    public static TransformerRule<string, float> StringToSingle() => Convert<string, float>((source, _) => ValueTask.FromResult(System.Convert.ToSingle(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>double</c>.</summary>
    public static TransformerRule<string, double> StringToDouble() => Convert<string, double>((source, _) => ValueTask.FromResult(System.Convert.ToDouble(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>decimal</c>.</summary>
    public static TransformerRule<string, decimal> StringToDecimal() => Convert<string, decimal>((source, _) => ValueTask.FromResult(System.Convert.ToDecimal(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>char</c>.</summary>
    public static TransformerRule<string, char> StringToChar() => Convert<string, char>((source, _) => ValueTask.FromResult(System.Convert.ToChar(source, CultureInfo.InvariantCulture)));
    /// <summary>Creates a step that converts the current <c>string</c> value to <c>DateTime</c>.</summary>
    public static TransformerRule<string, DateTime> StringToDateTime() => Convert<string, DateTime>((source, _) => ValueTask.FromResult(System.Convert.ToDateTime(source, CultureInfo.InvariantCulture)));
}
