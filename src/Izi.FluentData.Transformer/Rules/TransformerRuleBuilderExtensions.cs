using System.Globalization;
using System.Numerics;

namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// Fluent extension methods that append a <see cref="Rules"/> step to a <see cref="TransformerRuleBuilder{TSource, TCurrent}"/>.
/// Each method is constrained by the builder's current type, so a step is only offered while the running
/// value has a compatible type (string steps while it is a string, numeric steps while it is a number, etc.).
/// </summary>
public static class TransformerRuleBuilderExtensions
{
    // =============================
    // String Transforms (require the current value to be a string)
    // =============================

    /// <summary>Appends a step that removes leading and trailing whitespace.</summary>
    public static TransformerRuleBuilder<TSource, string> Trim<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.Trim());

    /// <summary>Appends a step that removes leading whitespace.</summary>
    public static TransformerRuleBuilder<TSource, string> TrimStart<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.TrimStart());

    /// <summary>Appends a step that removes trailing whitespace.</summary>
    public static TransformerRuleBuilder<TSource, string> TrimEnd<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.TrimEnd());

    /// <summary>Appends a step that upper-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> ToUpper<TSource>(this TransformerRuleBuilder<TSource, string> builder, CultureInfo culture) => builder.AddRule(Rules.ToUpper(culture));

    /// <summary>Appends a step that upper-cases the value using the invariant culture.</summary>
    public static TransformerRuleBuilder<TSource, string> ToUpper<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.ToUpper());

    /// <summary>Appends a step that lower-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> ToLower<TSource>(this TransformerRuleBuilder<TSource, string> builder, CultureInfo culture) => builder.AddRule(Rules.ToLower(culture));

    /// <summary>Appends a step that lower-cases the value using the invariant culture.</summary>
    public static TransformerRuleBuilder<TSource, string> ToLower<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.ToLower());

    /// <summary>Appends a step that title-cases the value using <paramref name="culture"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> ToTitleCase<TSource>(this TransformerRuleBuilder<TSource, string> builder, CultureInfo culture) => builder.AddRule(Rules.ToTitleCase(culture));

    /// <summary>Appends a step that title-cases the value using the invariant culture.</summary>
    public static TransformerRuleBuilder<TSource, string> ToTitleCase<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.ToTitleCase());

    /// <summary>Appends a step that replaces every occurrence of <paramref name="oldValue"/> with <paramref name="newValue"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> Replace<TSource>(this TransformerRuleBuilder<TSource, string> builder, string oldValue, string newValue) => builder.AddRule(Rules.Replace(oldValue, newValue));

    /// <summary>Appends a step that extracts <paramref name="length"/> characters starting at <paramref name="startIndex"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> Substring<TSource>(this TransformerRuleBuilder<TSource, string> builder, int startIndex, int length) => builder.AddRule(Rules.Substring(startIndex, length));

    /// <summary>Appends a step that extracts the substring from <paramref name="startIndex"/> to the end.</summary>
    public static TransformerRuleBuilder<TSource, string> Substring<TSource>(this TransformerRuleBuilder<TSource, string> builder, int startIndex) => builder.AddRule(Rules.Substring(startIndex));

    /// <summary>Appends a step that caps the value at <paramref name="maxLength"/> characters.</summary>
    public static TransformerRuleBuilder<TSource, string> Truncate<TSource>(this TransformerRuleBuilder<TSource, string> builder, int maxLength) => builder.AddRule(Rules.Truncate(maxLength));

    /// <summary>Appends a step that left-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> PadLeft<TSource>(this TransformerRuleBuilder<TSource, string> builder, int totalWidth, char paddingChar = ' ') => builder.AddRule(Rules.PadLeft(totalWidth, paddingChar));

    /// <summary>Appends a step that right-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static TransformerRuleBuilder<TSource, string> PadRight<TSource>(this TransformerRuleBuilder<TSource, string> builder, int totalWidth, char paddingChar = ' ') => builder.AddRule(Rules.PadRight(totalWidth, paddingChar));

    /// <summary>Appends a step that prepends <paramref name="prefix"/> to the value.</summary>
    public static TransformerRuleBuilder<TSource, string> Prepend<TSource>(this TransformerRuleBuilder<TSource, string> builder, string prefix) => builder.AddRule(Rules.Prepend(prefix));

    /// <summary>Appends a step that appends <paramref name="suffix"/> to the value.</summary>
    public static TransformerRuleBuilder<TSource, string> Append<TSource>(this TransformerRuleBuilder<TSource, string> builder, string suffix) => builder.AddRule(Rules.Append(suffix));

    // =============================
    // Numeric Transforms
    // =============================

    /// <summary>Appends a step that adds <paramref name="value"/> to the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Add<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber value) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Add(value));

    /// <summary>Appends a step that subtracts <paramref name="value"/> from the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Subtract<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber value) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Subtract(value));

    /// <summary>Appends a step that multiplies the current number by <paramref name="value"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Multiply<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber value) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Multiply(value));

    /// <summary>Appends a step that divides the current number by <paramref name="value"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Divide<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber value) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Divide(value));

    /// <summary>Appends a step that takes the absolute value of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Abs<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Abs<TNumber>());

    /// <summary>Appends a step that clamps the current number to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Clamp<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber min, TNumber max) where TNumber : INumber<TNumber> => builder.AddRule(Rules.Clamp(min, max));

    /// <summary>Appends a step that yields the sign of the current number as <c>-1</c>/<c>0</c>/<c>+1</c>, changing the running type to <see cref="int"/>.</summary>
    public static TransformerRuleBuilder<TSource, int> Sign<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : INumber<TNumber> => builder.AddRule(Rules.Sign<TNumber>());

    /// <summary>Appends a step that negates the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Invert<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : INumberBase<TNumber> => builder.AddRule(Rules.Invert<TNumber>());

    // ---- Rounding (floating-point only) ----

    /// <summary>Appends a step that rounds the current number to the nearest integral value.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Round<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Round<TNumber>());

    /// <summary>Appends a step that rounds the current number to <paramref name="digits"/> fractional digits.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Round<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, int digits) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Round<TNumber>(digits));

    /// <summary>Appends a step that rounds the current number to the nearest integral value using <paramref name="mode"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Round<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Round<TNumber>(mode));

    /// <summary>Appends a step that rounds the current number to <paramref name="digits"/> fractional digits using <paramref name="mode"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Round<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, int digits, MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Round<TNumber>(digits, mode));

    /// <summary>Appends a step that rounds the current number down to the nearest integral value.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Floor<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Floor<TNumber>());

    /// <summary>Appends a step that rounds the current number up to the nearest integral value.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Ceiling<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Ceiling<TNumber>());

    /// <summary>Appends a step that truncates the current number toward zero.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Truncate<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IFloatingPoint<TNumber> => builder.AddRule(Rules.Truncate<TNumber>());

    /// <summary>Appends a step that maps the current number from <c>[<paramref name="min"/>, <paramref name="max"/>]</c> into the <c>[0, 1]</c> range.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Normalize<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber min, TNumber max) where TNumber : INumber<TNumber> => builder.AddRule(Rules.Normalize(min, max));

    // ---- Roots / Powers ----

    /// <summary>Appends a step that takes the square root of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Sqrt<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IRootFunctions<TNumber> => builder.AddRule(Rules.Sqrt<TNumber>());

    /// <summary>Appends a step that takes the cube root of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Cbrt<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IRootFunctions<TNumber> => builder.AddRule(Rules.Cbrt<TNumber>());

    /// <summary>Appends a step that takes the <paramref name="n"/>-th root of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> RootN<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, int n) where TNumber : IRootFunctions<TNumber> => builder.AddRule(Rules.RootN<TNumber>(n));

    /// <summary>Appends a step that raises the current number to the power <paramref name="exponent"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Pow<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber exponent) where TNumber : IPowerFunctions<TNumber> => builder.AddRule(Rules.Pow(exponent));

    // ---- Exponential ----

    /// <summary>Appends a step that computes <c>e</c> raised to the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Exp<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IExponentialFunctions<TNumber> => builder.AddRule(Rules.Exp<TNumber>());

    /// <summary>Appends a step that computes <c>2</c> raised to the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Exp2<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IExponentialFunctions<TNumber> => builder.AddRule(Rules.Exp2<TNumber>());

    /// <summary>Appends a step that computes <c>10</c> raised to the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Exp10<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IExponentialFunctions<TNumber> => builder.AddRule(Rules.Exp10<TNumber>());

    // ---- Logarithmic ----

    /// <summary>Appends a step that takes the natural (base-<c>e</c>) logarithm of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Log<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ILogarithmicFunctions<TNumber> => builder.AddRule(Rules.Log<TNumber>());

    /// <summary>Appends a step that takes the logarithm of the current number in base <paramref name="newBase"/>.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Log<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder, TNumber newBase) where TNumber : ILogarithmicFunctions<TNumber> => builder.AddRule(Rules.Log(newBase));

    /// <summary>Appends a step that takes the base-<c>2</c> logarithm of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Log2<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ILogarithmicFunctions<TNumber> => builder.AddRule(Rules.Log2<TNumber>());

    /// <summary>Appends a step that takes the base-<c>10</c> logarithm of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Log10<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ILogarithmicFunctions<TNumber> => builder.AddRule(Rules.Log10<TNumber>());

    // ---- Trigonometric (radians) ----

    /// <summary>Appends a step that computes the sine of the current number (in radians).</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Sin<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Sin<TNumber>());

    /// <summary>Appends a step that computes the cosine of the current number (in radians).</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Cos<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Cos<TNumber>());

    /// <summary>Appends a step that computes the tangent of the current number (in radians).</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Tan<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Tan<TNumber>());

    /// <summary>Appends a step that computes the arcsine (in radians) of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Asin<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Asin<TNumber>());

    /// <summary>Appends a step that computes the arccosine (in radians) of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Acos<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Acos<TNumber>());

    /// <summary>Appends a step that computes the arctangent (in radians) of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Atan<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.Atan<TNumber>());

    /// <summary>Appends a step that converts the current number from degrees to radians.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> DegreesToRadians<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.DegreesToRadians<TNumber>());

    /// <summary>Appends a step that converts the current number from radians to degrees.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> RadiansToDegrees<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : ITrigonometricFunctions<TNumber> => builder.AddRule(Rules.RadiansToDegrees<TNumber>());

    // ---- Hyperbolic ----

    /// <summary>Appends a step that computes the hyperbolic sine of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Sinh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Sinh<TNumber>());

    /// <summary>Appends a step that computes the hyperbolic cosine of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Cosh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Cosh<TNumber>());

    /// <summary>Appends a step that computes the hyperbolic tangent of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Tanh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Tanh<TNumber>());

    /// <summary>Appends a step that computes the inverse hyperbolic sine of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Asinh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Asinh<TNumber>());

    /// <summary>Appends a step that computes the inverse hyperbolic cosine of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Acosh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Acosh<TNumber>());

    /// <summary>Appends a step that computes the inverse hyperbolic tangent of the current number.</summary>
    public static TransformerRuleBuilder<TSource, TNumber> Atanh<TSource, TNumber>(this TransformerRuleBuilder<TSource, TNumber> builder) where TNumber : IHyperbolicFunctions<TNumber> => builder.AddRule(Rules.Atanh<TNumber>());

    // =============================
    // Default Value
    // =============================

    /// <summary>Appends a step that substitutes <paramref name="defaultValue"/> whenever <paramref name="predicate"/> matches the current value.</summary>
    public static TransformerRuleBuilder<TSource, TCurrent> DefaultIf<TSource, TCurrent>(this TransformerRuleBuilder<TSource, TCurrent> builder, TCurrent defaultValue, Func<TCurrent, bool> predicate) => builder.AddRule(Rules.DefaultIf(defaultValue, predicate));

    /// <summary>Appends a step that substitutes <paramref name="defaultValue"/> when the current value is <see langword="null"/>.</summary>
    public static TransformerRuleBuilder<TSource, TCurrent> DefaultIfNull<TSource, TCurrent>(this TransformerRuleBuilder<TSource, TCurrent> builder, TCurrent defaultValue) => builder.AddRule(Rules.DefaultIfNull(defaultValue));

    /// <summary>Appends a step that substitutes <paramref name="defaultValue"/> when the current value is null, an empty string, or an empty collection/sequence.</summary>
    public static TransformerRuleBuilder<TSource, TCurrent> DefaultIfEmpty<TSource, TCurrent>(this TransformerRuleBuilder<TSource, TCurrent> builder, TCurrent defaultValue) => builder.AddRule(Rules.DefaultIfEmpty(defaultValue));

    /// <summary>Appends a step that substitutes <paramref name="defaultValue"/> when the current string is null, empty, or whitespace.</summary>
    public static TransformerRuleBuilder<TSource, string> DefaultIfNullOrWhitespace<TSource>(this TransformerRuleBuilder<TSource, string> builder, string defaultValue) => builder.AddRule(Rules.DefaultIfNullOrWhitespace(defaultValue));

    // =============================
    // Generic Transforms (TCurrent -> TDestination)
    // =============================

    /// <summary>Appends an arbitrary (optionally asynchronous) <paramref name="converter"/>, changing the running type to <typeparamref name="TDestination"/>.</summary>
    public static TransformerRuleBuilder<TSource, TDestination> Convert<TSource, TCurrent, TDestination>(this TransformerRuleBuilder<TSource, TCurrent> builder, Func<TCurrent, CancellationToken, ValueTask<TDestination>> converter) => builder.AddRule(Rules.Convert(converter));

    // =============================
    // Primitive Conversions (TCurrent -> TDestination)
    // =============================
    // ---- Boolean -> * ----
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> BooleanToByte<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToByte());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> BooleanToSByte<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToSByte());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> BooleanToInt16<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToInt16());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> BooleanToUInt16<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToUInt16());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> BooleanToInt32<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToInt32());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> BooleanToUInt32<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToUInt32());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> BooleanToInt64<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToInt64());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> BooleanToUInt64<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToUInt64());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> BooleanToSingle<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToSingle());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> BooleanToDouble<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToDouble());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> BooleanToDecimal<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToDecimal());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> BooleanToChar<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToChar());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> BooleanToDateTime<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToDateTime());
    /// <summary>Appends a step that converts the current <c>bool</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> BooleanToString<TSource>(this TransformerRuleBuilder<TSource, bool> builder) => builder.AddRule(Rules.BooleanToString());

    // ---- Byte -> * ----
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> ByteToBoolean<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToBoolean());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> ByteToSByte<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToSByte());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> ByteToInt16<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToInt16());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> ByteToUInt16<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToUInt16());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> ByteToInt32<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToInt32());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> ByteToUInt32<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToUInt32());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> ByteToInt64<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToInt64());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> ByteToUInt64<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToUInt64());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> ByteToSingle<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToSingle());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> ByteToDouble<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToDouble());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> ByteToDecimal<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToDecimal());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> ByteToChar<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToChar());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> ByteToDateTime<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToDateTime());
    /// <summary>Appends a step that converts the current <c>byte</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> ByteToString<TSource>(this TransformerRuleBuilder<TSource, byte> builder) => builder.AddRule(Rules.ByteToString());

    // ---- SByte -> * ----
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> SByteToBoolean<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToBoolean());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> SByteToByte<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToByte());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> SByteToInt16<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToInt16());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> SByteToUInt16<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToUInt16());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> SByteToInt32<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToInt32());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> SByteToUInt32<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToUInt32());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> SByteToInt64<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToInt64());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> SByteToUInt64<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToUInt64());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> SByteToSingle<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToSingle());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> SByteToDouble<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToDouble());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> SByteToDecimal<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToDecimal());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> SByteToChar<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToChar());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> SByteToDateTime<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToDateTime());
    /// <summary>Appends a step that converts the current <c>sbyte</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> SByteToString<TSource>(this TransformerRuleBuilder<TSource, sbyte> builder) => builder.AddRule(Rules.SByteToString());

    // ---- Int16 -> * ----
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> Int16ToBoolean<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToBoolean());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> Int16ToByte<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToByte());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> Int16ToSByte<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToSByte());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> Int16ToUInt16<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToUInt16());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> Int16ToInt32<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToInt32());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> Int16ToUInt32<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToUInt32());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> Int16ToInt64<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToInt64());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> Int16ToUInt64<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToUInt64());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> Int16ToSingle<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToSingle());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> Int16ToDouble<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToDouble());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> Int16ToDecimal<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToDecimal());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> Int16ToChar<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToChar());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> Int16ToDateTime<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToDateTime());
    /// <summary>Appends a step that converts the current <c>short</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> Int16ToString<TSource>(this TransformerRuleBuilder<TSource, short> builder) => builder.AddRule(Rules.Int16ToString());

    // ---- UInt16 -> * ----
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> UInt16ToBoolean<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToBoolean());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> UInt16ToByte<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToByte());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> UInt16ToSByte<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToSByte());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> UInt16ToInt16<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToInt16());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> UInt16ToInt32<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToInt32());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> UInt16ToUInt32<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToUInt32());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> UInt16ToInt64<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToInt64());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> UInt16ToUInt64<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToUInt64());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> UInt16ToSingle<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToSingle());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> UInt16ToDouble<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToDouble());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> UInt16ToDecimal<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToDecimal());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> UInt16ToChar<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToChar());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> UInt16ToDateTime<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToDateTime());
    /// <summary>Appends a step that converts the current <c>ushort</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> UInt16ToString<TSource>(this TransformerRuleBuilder<TSource, ushort> builder) => builder.AddRule(Rules.UInt16ToString());

    // ---- Int32 -> * ----
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> Int32ToBoolean<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToBoolean());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> Int32ToByte<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToByte());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> Int32ToSByte<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToSByte());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> Int32ToInt16<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToInt16());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> Int32ToUInt16<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToUInt16());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> Int32ToUInt32<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToUInt32());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> Int32ToInt64<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToInt64());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> Int32ToUInt64<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToUInt64());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> Int32ToSingle<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToSingle());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> Int32ToDouble<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToDouble());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> Int32ToDecimal<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToDecimal());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> Int32ToChar<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToChar());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> Int32ToDateTime<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToDateTime());
    /// <summary>Appends a step that converts the current <c>int</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> Int32ToString<TSource>(this TransformerRuleBuilder<TSource, int> builder) => builder.AddRule(Rules.Int32ToString());

    // ---- UInt32 -> * ----
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> UInt32ToBoolean<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToBoolean());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> UInt32ToByte<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToByte());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> UInt32ToSByte<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToSByte());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> UInt32ToInt16<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToInt16());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> UInt32ToUInt16<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToUInt16());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> UInt32ToInt32<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToInt32());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> UInt32ToInt64<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToInt64());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> UInt32ToUInt64<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToUInt64());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> UInt32ToSingle<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToSingle());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> UInt32ToDouble<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToDouble());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> UInt32ToDecimal<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToDecimal());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> UInt32ToChar<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToChar());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> UInt32ToDateTime<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToDateTime());
    /// <summary>Appends a step that converts the current <c>uint</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> UInt32ToString<TSource>(this TransformerRuleBuilder<TSource, uint> builder) => builder.AddRule(Rules.UInt32ToString());

    // ---- Int64 -> * ----
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> Int64ToBoolean<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToBoolean());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> Int64ToByte<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToByte());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> Int64ToSByte<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToSByte());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> Int64ToInt16<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToInt16());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> Int64ToUInt16<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToUInt16());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> Int64ToInt32<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToInt32());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> Int64ToUInt32<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToUInt32());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> Int64ToUInt64<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToUInt64());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> Int64ToSingle<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToSingle());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> Int64ToDouble<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToDouble());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> Int64ToDecimal<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToDecimal());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> Int64ToChar<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToChar());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> Int64ToDateTime<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToDateTime());
    /// <summary>Appends a step that converts the current <c>long</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> Int64ToString<TSource>(this TransformerRuleBuilder<TSource, long> builder) => builder.AddRule(Rules.Int64ToString());

    // ---- UInt64 -> * ----
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> UInt64ToBoolean<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToBoolean());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> UInt64ToByte<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToByte());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> UInt64ToSByte<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToSByte());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> UInt64ToInt16<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToInt16());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> UInt64ToUInt16<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToUInt16());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> UInt64ToInt32<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToInt32());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> UInt64ToUInt32<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToUInt32());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> UInt64ToInt64<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToInt64());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> UInt64ToSingle<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToSingle());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> UInt64ToDouble<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToDouble());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> UInt64ToDecimal<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToDecimal());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> UInt64ToChar<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToChar());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> UInt64ToDateTime<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToDateTime());
    /// <summary>Appends a step that converts the current <c>ulong</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> UInt64ToString<TSource>(this TransformerRuleBuilder<TSource, ulong> builder) => builder.AddRule(Rules.UInt64ToString());

    // ---- Single -> * ----
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> SingleToBoolean<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToBoolean());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> SingleToByte<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToByte());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> SingleToSByte<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToSByte());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> SingleToInt16<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToInt16());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> SingleToUInt16<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToUInt16());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> SingleToInt32<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToInt32());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> SingleToUInt32<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToUInt32());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> SingleToInt64<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToInt64());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> SingleToUInt64<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToUInt64());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> SingleToDouble<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToDouble());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> SingleToDecimal<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToDecimal());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> SingleToChar<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToChar());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> SingleToDateTime<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToDateTime());
    /// <summary>Appends a step that converts the current <c>float</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> SingleToString<TSource>(this TransformerRuleBuilder<TSource, float> builder) => builder.AddRule(Rules.SingleToString());

    // ---- Double -> * ----
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> DoubleToBoolean<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToBoolean());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> DoubleToByte<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToByte());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> DoubleToSByte<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToSByte());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> DoubleToInt16<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToInt16());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> DoubleToUInt16<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToUInt16());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> DoubleToInt32<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToInt32());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> DoubleToUInt32<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToUInt32());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> DoubleToInt64<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToInt64());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> DoubleToUInt64<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToUInt64());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> DoubleToSingle<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToSingle());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> DoubleToDecimal<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToDecimal());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> DoubleToChar<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToChar());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> DoubleToDateTime<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToDateTime());
    /// <summary>Appends a step that converts the current <c>double</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> DoubleToString<TSource>(this TransformerRuleBuilder<TSource, double> builder) => builder.AddRule(Rules.DoubleToString());

    // ---- Decimal -> * ----
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> DecimalToBoolean<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToBoolean());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> DecimalToByte<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToByte());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> DecimalToSByte<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToSByte());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> DecimalToInt16<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToInt16());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> DecimalToUInt16<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToUInt16());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> DecimalToInt32<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToInt32());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> DecimalToUInt32<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToUInt32());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> DecimalToInt64<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToInt64());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> DecimalToUInt64<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToUInt64());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> DecimalToSingle<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToSingle());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> DecimalToDouble<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToDouble());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> DecimalToChar<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToChar());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> DecimalToDateTime<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToDateTime());
    /// <summary>Appends a step that converts the current <c>decimal</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> DecimalToString<TSource>(this TransformerRuleBuilder<TSource, decimal> builder) => builder.AddRule(Rules.DecimalToString());

    // ---- Char -> * ----
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> CharToBoolean<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToBoolean());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> CharToByte<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToByte());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> CharToSByte<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToSByte());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> CharToInt16<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToInt16());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> CharToUInt16<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToUInt16());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> CharToInt32<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToInt32());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> CharToUInt32<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToUInt32());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> CharToInt64<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToInt64());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> CharToUInt64<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToUInt64());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> CharToSingle<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToSingle());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> CharToDouble<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToDouble());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> CharToDecimal<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToDecimal());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> CharToDateTime<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToDateTime());
    /// <summary>Appends a step that converts the current <c>char</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> CharToString<TSource>(this TransformerRuleBuilder<TSource, char> builder) => builder.AddRule(Rules.CharToString());

    // ---- DateTime -> * ----
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> DateTimeToBoolean<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToBoolean());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> DateTimeToByte<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToByte());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> DateTimeToSByte<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToSByte());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> DateTimeToInt16<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToInt16());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> DateTimeToUInt16<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToUInt16());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> DateTimeToInt32<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToInt32());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> DateTimeToUInt32<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToUInt32());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> DateTimeToInt64<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToInt64());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> DateTimeToUInt64<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToUInt64());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> DateTimeToSingle<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToSingle());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> DateTimeToDouble<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToDouble());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> DateTimeToDecimal<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToDecimal());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> DateTimeToChar<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToChar());
    /// <summary>Appends a step that converts the current <c>DateTime</c> value to <c>string</c>.</summary>
    public static TransformerRuleBuilder<TSource, string> DateTimeToString<TSource>(this TransformerRuleBuilder<TSource, DateTime> builder) => builder.AddRule(Rules.DateTimeToString());

    // ---- String -> * ----
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>bool</c>.</summary>
    public static TransformerRuleBuilder<TSource, bool> StringToBoolean<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToBoolean());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>byte</c>.</summary>
    public static TransformerRuleBuilder<TSource, byte> StringToByte<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToByte());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>sbyte</c>.</summary>
    public static TransformerRuleBuilder<TSource, sbyte> StringToSByte<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToSByte());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>short</c>.</summary>
    public static TransformerRuleBuilder<TSource, short> StringToInt16<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToInt16());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>ushort</c>.</summary>
    public static TransformerRuleBuilder<TSource, ushort> StringToUInt16<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToUInt16());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>int</c>.</summary>
    public static TransformerRuleBuilder<TSource, int> StringToInt32<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToInt32());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>uint</c>.</summary>
    public static TransformerRuleBuilder<TSource, uint> StringToUInt32<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToUInt32());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>long</c>.</summary>
    public static TransformerRuleBuilder<TSource, long> StringToInt64<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToInt64());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>ulong</c>.</summary>
    public static TransformerRuleBuilder<TSource, ulong> StringToUInt64<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToUInt64());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>float</c>.</summary>
    public static TransformerRuleBuilder<TSource, float> StringToSingle<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToSingle());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>double</c>.</summary>
    public static TransformerRuleBuilder<TSource, double> StringToDouble<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToDouble());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>decimal</c>.</summary>
    public static TransformerRuleBuilder<TSource, decimal> StringToDecimal<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToDecimal());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>char</c>.</summary>
    public static TransformerRuleBuilder<TSource, char> StringToChar<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToChar());
    /// <summary>Appends a step that converts the current <c>string</c> value to <c>DateTime</c>.</summary>
    public static TransformerRuleBuilder<TSource, DateTime> StringToDateTime<TSource>(this TransformerRuleBuilder<TSource, string> builder) => builder.AddRule(Rules.StringToDateTime());
}

