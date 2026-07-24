using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a built-in <see cref="TransformerRules"/> step onto a <see cref="Transformer{T}"/>
/// and return it, so per-property pipelines read naturally, e.g. <c>RuleFor(x =&gt; x.Name).Trim().ToUpper()</c>.
/// Every method here is a one-liner over the matching factory; the sections mirror <see cref="TransformerRules"/>.
/// </summary>
public static class TransformerExtensions
{
    // =============================
    // String (a null source is coalesced to "" by the underlying rule)
    // =============================

    /// <summary>Removes leading and trailing whitespace.</summary>
    public static Transformer<string> Trim(this Transformer<string> t) => t.AddTransformer(TransformerRules.Trim());

    /// <summary>Removes leading whitespace.</summary>
    public static Transformer<string> TrimStart(this Transformer<string> t) => t.AddTransformer(TransformerRules.TrimStart());

    /// <summary>Removes trailing whitespace.</summary>
    public static Transformer<string> TrimEnd(this Transformer<string> t) => t.AddTransformer(TransformerRules.TrimEnd());

    /// <summary>Upper-cases the value using the invariant culture.</summary>
    public static Transformer<string> ToUpper(this Transformer<string> t) => t.AddTransformer(TransformerRules.ToUpper());

    /// <summary>Upper-cases the value using <paramref name="culture"/>.</summary>
    public static Transformer<string> ToUpper(this Transformer<string> t, CultureInfo culture) => t.AddTransformer(TransformerRules.ToUpper(culture));

    /// <summary>Lower-cases the value using the invariant culture.</summary>
    public static Transformer<string> ToLower(this Transformer<string> t) => t.AddTransformer(TransformerRules.ToLower());

    /// <summary>Lower-cases the value using <paramref name="culture"/>.</summary>
    public static Transformer<string> ToLower(this Transformer<string> t, CultureInfo culture) => t.AddTransformer(TransformerRules.ToLower(culture));

    /// <summary>Title-cases the value using the invariant culture.</summary>
    public static Transformer<string> ToTitleCase(this Transformer<string> t) => t.AddTransformer(TransformerRules.ToTitleCase());

    /// <summary>Title-cases the value using <paramref name="culture"/>.</summary>
    public static Transformer<string> ToTitleCase(this Transformer<string> t, CultureInfo culture) => t.AddTransformer(TransformerRules.ToTitleCase(culture));

    /// <summary>Replaces every occurrence of <paramref name="oldValue"/> with <paramref name="newValue"/>.</summary>
    public static Transformer<string> Replace(this Transformer<string> t, string oldValue, string newValue) => t.AddTransformer(TransformerRules.Replace(oldValue, newValue));

    /// <summary>Replaces every match of <paramref name="regex"/> with <paramref name="replacement"/>. Pass a cached or <c>[GeneratedRegex]</c> instance to reuse it across rules without recompiling.</summary>
    public static Transformer<string> ReplaceRegex(this Transformer<string> t, Regex regex, string replacement) => t.AddTransformer(TransformerRules.ReplaceRegex(regex, replacement));

    /// <summary>Replaces every substring matching <paramref name="pattern"/> with <paramref name="replacement"/>.</summary>
    public static Transformer<string> ReplaceRegex(this Transformer<string> t, string pattern, string replacement) => t.AddTransformer(TransformerRules.ReplaceRegex(pattern, replacement));

    /// <summary>Replaces every substring matching <paramref name="pattern"/> with <paramref name="replacement"/> using <paramref name="options"/>.</summary>
    public static Transformer<string> ReplaceRegex(this Transformer<string> t, string pattern, string replacement, RegexOptions options) => t.AddTransformer(TransformerRules.ReplaceRegex(pattern, replacement, options));

    /// <summary>Removes all whitespace characters from the value (anywhere in the string, not just the ends).</summary>
    public static Transformer<string> RemoveWhitespace(this Transformer<string> t) => t.AddTransformer(TransformerRules.RemoveWhitespace());

    /// <summary>Collapses every run of whitespace into a single space.</summary>
    public static Transformer<string> CollapseWhitespace(this Transformer<string> t) => t.AddTransformer(TransformerRules.CollapseWhitespace());

    /// <summary>Extracts <paramref name="length"/> characters starting at <paramref name="startIndex"/>.</summary>
    public static Transformer<string> Substring(this Transformer<string> t, int startIndex, int length) => t.AddTransformer(TransformerRules.Substring(startIndex, length));

    /// <summary>Extracts the substring from <paramref name="startIndex"/> to the end.</summary>
    public static Transformer<string> Substring(this Transformer<string> t, int startIndex) => t.AddTransformer(TransformerRules.Substring(startIndex));

    /// <summary>Caps the value at <paramref name="maxLength"/> characters.</summary>
    public static Transformer<string> Truncate(this Transformer<string> t, int maxLength) => t.AddTransformer(TransformerRules.Truncate(maxLength));

    /// <summary>Left-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static Transformer<string> PadLeft(this Transformer<string> t, int totalWidth, char paddingChar) => t.AddTransformer(TransformerRules.PadLeft(totalWidth, paddingChar));

    /// <summary>Right-pads the value to <paramref name="totalWidth"/> using <paramref name="paddingChar"/>.</summary>
    public static Transformer<string> PadRight(this Transformer<string> t, int totalWidth, char paddingChar) => t.AddTransformer(TransformerRules.PadRight(totalWidth, paddingChar));

    /// <summary>Prepends <paramref name="prefix"/> to the value.</summary>
    public static Transformer<string> Prepend(this Transformer<string> t, string prefix) => t.AddTransformer(TransformerRules.Prepend(prefix));

    /// <summary>Appends <paramref name="suffix"/> to the value.</summary>
    public static Transformer<string> Append(this Transformer<string> t, string suffix) => t.AddTransformer(TransformerRules.Append(suffix));

    // =============================
    // Numeric — arithmetic
    // =============================

    /// <summary>Adds <paramref name="value"/> to the current number.</summary>
    public static Transformer<TNumber> Add<TNumber>(this Transformer<TNumber> t, TNumber value) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Add(value));

    /// <summary>Subtracts <paramref name="value"/> from the current number.</summary>
    public static Transformer<TNumber> Subtract<TNumber>(this Transformer<TNumber> t, TNumber value) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Subtract(value));

    /// <summary>Multiplies the current number by <paramref name="value"/>.</summary>
    public static Transformer<TNumber> Multiply<TNumber>(this Transformer<TNumber> t, TNumber value) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Multiply(value));

    /// <summary>Divides the current number by <paramref name="value"/>.</summary>
    public static Transformer<TNumber> Divide<TNumber>(this Transformer<TNumber> t, TNumber value) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Divide(value));

    /// <summary>Takes the absolute value of the current number.</summary>
    public static Transformer<TNumber> Abs<TNumber>(this Transformer<TNumber> t) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Abs<TNumber>());

    /// <summary>Clamps the current number to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<TNumber> Clamp<TNumber>(this Transformer<TNumber> t, TNumber min, TNumber max) where TNumber : INumber<TNumber> => t.AddTransformer(TransformerRules.Clamp(min, max));

    /// <summary>Negates the current number (unary minus).</summary>
    public static Transformer<TNumber> Invert<TNumber>(this Transformer<TNumber> t) where TNumber : INumberBase<TNumber> => t.AddTransformer(TransformerRules.Invert<TNumber>());

    /// <summary>Maps the current number from <c>[<paramref name="min"/>, <paramref name="max"/>]</c> into <c>[0, 1]</c>.</summary>
    public static Transformer<TNumber> Normalize<TNumber>(this Transformer<TNumber> t, TNumber min, TNumber max) where TNumber : INumber<TNumber> => t.AddTransformer(TransformerRules.Normalize(min, max));

    // =============================
    // Numeric — rounding (floating-point only)
    // =============================

    /// <summary>Rounds the current number to the nearest integral value.</summary>
    public static Transformer<TNumber> Round<TNumber>(this Transformer<TNumber> t) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Round<TNumber>());

    /// <summary>Rounds the current number to <paramref name="digits"/> fractional digits.</summary>
    public static Transformer<TNumber> Round<TNumber>(this Transformer<TNumber> t, int digits) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Round<TNumber>(digits));

    /// <summary>Rounds the current number to the nearest integral value using <paramref name="mode"/>.</summary>
    public static Transformer<TNumber> Round<TNumber>(this Transformer<TNumber> t, MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Round<TNumber>(mode));

    /// <summary>Rounds the current number to <paramref name="digits"/> fractional digits using <paramref name="mode"/>.</summary>
    public static Transformer<TNumber> Round<TNumber>(this Transformer<TNumber> t, int digits, MidpointRounding mode) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Round<TNumber>(digits, mode));

    /// <summary>Rounds the current number down to the nearest integral value.</summary>
    public static Transformer<TNumber> Floor<TNumber>(this Transformer<TNumber> t) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Floor<TNumber>());

    /// <summary>Rounds the current number up to the nearest integral value.</summary>
    public static Transformer<TNumber> Ceiling<TNumber>(this Transformer<TNumber> t) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Ceiling<TNumber>());

    /// <summary>Truncates the current number toward zero.</summary>
    public static Transformer<TNumber> Truncate<TNumber>(this Transformer<TNumber> t) where TNumber : IFloatingPoint<TNumber> => t.AddTransformer(TransformerRules.Truncate<TNumber>());

    // =============================
    // Numeric — roots / powers
    // =============================

    /// <summary>Takes the square root of the current number.</summary>
    public static Transformer<TNumber> Sqrt<TNumber>(this Transformer<TNumber> t) where TNumber : IRootFunctions<TNumber> => t.AddTransformer(TransformerRules.Sqrt<TNumber>());

    /// <summary>Takes the cube root of the current number.</summary>
    public static Transformer<TNumber> Cbrt<TNumber>(this Transformer<TNumber> t) where TNumber : IRootFunctions<TNumber> => t.AddTransformer(TransformerRules.Cbrt<TNumber>());

    /// <summary>Takes the <paramref name="n"/>-th root of the current number.</summary>
    public static Transformer<TNumber> RootN<TNumber>(this Transformer<TNumber> t, int n) where TNumber : IRootFunctions<TNumber> => t.AddTransformer(TransformerRules.RootN<TNumber>(n));

    /// <summary>Raises the current number to the power <paramref name="exponent"/>.</summary>
    public static Transformer<TNumber> Pow<TNumber>(this Transformer<TNumber> t, TNumber exponent) where TNumber : IPowerFunctions<TNumber> => t.AddTransformer(TransformerRules.Pow(exponent));

    // =============================
    // Numeric — exponential
    // =============================

    /// <summary>Computes <c>e</c> raised to the current number.</summary>
    public static Transformer<TNumber> Exp<TNumber>(this Transformer<TNumber> t) where TNumber : IExponentialFunctions<TNumber> => t.AddTransformer(TransformerRules.Exp<TNumber>());

    /// <summary>Computes <c>2</c> raised to the current number.</summary>
    public static Transformer<TNumber> Exp2<TNumber>(this Transformer<TNumber> t) where TNumber : IExponentialFunctions<TNumber> => t.AddTransformer(TransformerRules.Exp2<TNumber>());

    /// <summary>Computes <c>10</c> raised to the current number.</summary>
    public static Transformer<TNumber> Exp10<TNumber>(this Transformer<TNumber> t) where TNumber : IExponentialFunctions<TNumber> => t.AddTransformer(TransformerRules.Exp10<TNumber>());

    // =============================
    // Numeric — logarithmic
    // =============================

    /// <summary>Takes the natural (base-<c>e</c>) logarithm of the current number.</summary>
    public static Transformer<TNumber> Log<TNumber>(this Transformer<TNumber> t) where TNumber : ILogarithmicFunctions<TNumber> => t.AddTransformer(TransformerRules.Log<TNumber>());

    /// <summary>Takes the logarithm of the current number in base <paramref name="newBase"/>.</summary>
    public static Transformer<TNumber> Log<TNumber>(this Transformer<TNumber> t, TNumber newBase) where TNumber : ILogarithmicFunctions<TNumber> => t.AddTransformer(TransformerRules.Log(newBase));

    /// <summary>Takes the base-<c>2</c> logarithm of the current number.</summary>
    public static Transformer<TNumber> Log2<TNumber>(this Transformer<TNumber> t) where TNumber : ILogarithmicFunctions<TNumber> => t.AddTransformer(TransformerRules.Log2<TNumber>());

    /// <summary>Takes the base-<c>10</c> logarithm of the current number.</summary>
    public static Transformer<TNumber> Log10<TNumber>(this Transformer<TNumber> t) where TNumber : ILogarithmicFunctions<TNumber> => t.AddTransformer(TransformerRules.Log10<TNumber>());

    // =============================
    // Numeric — trigonometric (radians)
    // =============================

    /// <summary>Computes the sine of the current number (in radians).</summary>
    public static Transformer<TNumber> Sin<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Sin<TNumber>());

    /// <summary>Computes the cosine of the current number (in radians).</summary>
    public static Transformer<TNumber> Cos<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Cos<TNumber>());

    /// <summary>Computes the tangent of the current number (in radians).</summary>
    public static Transformer<TNumber> Tan<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Tan<TNumber>());

    /// <summary>Computes the arcsine (in radians) of the current number.</summary>
    public static Transformer<TNumber> Asin<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Asin<TNumber>());

    /// <summary>Computes the arccosine (in radians) of the current number.</summary>
    public static Transformer<TNumber> Acos<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Acos<TNumber>());

    /// <summary>Computes the arctangent (in radians) of the current number.</summary>
    public static Transformer<TNumber> Atan<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.Atan<TNumber>());

    /// <summary>Converts the current number from degrees to radians.</summary>
    public static Transformer<TNumber> DegreesToRadians<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.DegreesToRadians<TNumber>());

    /// <summary>Converts the current number from radians to degrees.</summary>
    public static Transformer<TNumber> RadiansToDegrees<TNumber>(this Transformer<TNumber> t) where TNumber : ITrigonometricFunctions<TNumber> => t.AddTransformer(TransformerRules.RadiansToDegrees<TNumber>());

    // =============================
    // Numeric — hyperbolic
    // =============================

    /// <summary>Computes the hyperbolic sine of the current number.</summary>
    public static Transformer<TNumber> Sinh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Sinh<TNumber>());

    /// <summary>Computes the hyperbolic cosine of the current number.</summary>
    public static Transformer<TNumber> Cosh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Cosh<TNumber>());

    /// <summary>Computes the hyperbolic tangent of the current number.</summary>
    public static Transformer<TNumber> Tanh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Tanh<TNumber>());

    /// <summary>Computes the inverse hyperbolic sine of the current number.</summary>
    public static Transformer<TNumber> Asinh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Asinh<TNumber>());

    /// <summary>Computes the inverse hyperbolic cosine of the current number.</summary>
    public static Transformer<TNumber> Acosh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Acosh<TNumber>());

    /// <summary>Computes the inverse hyperbolic tangent of the current number.</summary>
    public static Transformer<TNumber> Atanh<TNumber>(this Transformer<TNumber> t) where TNumber : IHyperbolicFunctions<TNumber> => t.AddTransformer(TransformerRules.Atanh<TNumber>());

    // =============================
    // Default value
    // =============================

    /// <summary>Substitutes <paramref name="defaultValue"/> whenever <paramref name="predicate"/> matches the current value.</summary>
    public static Transformer<TSource> DefaultIf<TSource>(this Transformer<TSource> t, TSource defaultValue, Func<TSource, bool> predicate) => t.AddTransformer(TransformerRules.DefaultIf(defaultValue, predicate));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see langword="null"/>.</summary>
    public static Transformer<TSource> DefaultIfNull<TSource>(this Transformer<TSource> t, TSource defaultValue) => t.AddTransformer(TransformerRules.DefaultIfNull(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is null, an empty string, or an empty collection/sequence.</summary>
    public static Transformer<TSource> DefaultIfEmpty<TSource>(this Transformer<TSource> t, TSource defaultValue) => t.AddTransformer(TransformerRules.DefaultIfEmpty(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current string is null, empty, or whitespace.</summary>
    public static Transformer<string> DefaultIfNullOrWhitespace(this Transformer<string> t, string defaultValue) => t.AddTransformer(TransformerRules.DefaultIfNullOrWhitespace(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is at its type's <c>MinValue</c>.</summary>
    public static Transformer<TSource> DefaultIfMinValue<TSource>(this Transformer<TSource> t, TSource defaultValue) where TSource : IComparable<TSource>, IMinMaxValue<TSource> => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is at its type's <c>MaxValue</c>.</summary>
    public static Transformer<TSource> DefaultIfMaxValue<TSource>(this Transformer<TSource> t, TSource defaultValue) where TSource : IComparable<TSource>, IMinMaxValue<TSource> => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is at or below <paramref name="minimum"/>.</summary>
    public static Transformer<TSource> DefaultIfMinValue<TSource>(this Transformer<TSource> t, TSource minimum, TSource defaultValue) where TSource : IComparable<TSource> => t.AddTransformer(TransformerRules.DefaultIfMinValue(minimum, defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is at or above <paramref name="maximum"/>.</summary>
    public static Transformer<TSource> DefaultIfMaxValue<TSource>(this Transformer<TSource> t, TSource maximum, TSource defaultValue) where TSource : IComparable<TSource> => t.AddTransformer(TransformerRules.DefaultIfMaxValue(maximum, defaultValue));

    // =============================
    // Date/time — arithmetic / offset
    // (generic over T: DateTime, DateTimeOffset, DateOnly, TimeOnly, TimeSpan as each rule supports)
    // =============================

    /// <summary>Adds <paramref name="years"/> years.</summary>
    public static Transformer<T> AddYears<T>(this Transformer<T> t, int years) => t.AddTransformer(TransformerRules.AddYears<T>(years));

    /// <summary>Adds <paramref name="months"/> months.</summary>
    public static Transformer<T> AddMonths<T>(this Transformer<T> t, int months) => t.AddTransformer(TransformerRules.AddMonths<T>(months));

    /// <summary>Adds <paramref name="days"/> days.</summary>
    public static Transformer<T> AddDays<T>(this Transformer<T> t, double days) => t.AddTransformer(TransformerRules.AddDays<T>(days));

    /// <summary>Adds <paramref name="hours"/> hours.</summary>
    public static Transformer<T> AddHours<T>(this Transformer<T> t, double hours) => t.AddTransformer(TransformerRules.AddHours<T>(hours));

    /// <summary>Adds <paramref name="minutes"/> minutes.</summary>
    public static Transformer<T> AddMinutes<T>(this Transformer<T> t, double minutes) => t.AddTransformer(TransformerRules.AddMinutes<T>(minutes));

    /// <summary>Adds <paramref name="seconds"/> seconds.</summary>
    public static Transformer<T> AddSeconds<T>(this Transformer<T> t, double seconds) => t.AddTransformer(TransformerRules.AddSeconds<T>(seconds));

    /// <summary>Adds <paramref name="milliseconds"/> milliseconds.</summary>
    public static Transformer<T> AddMilliseconds<T>(this Transformer<T> t, double milliseconds) => t.AddTransformer(TransformerRules.AddMilliseconds<T>(milliseconds));

    /// <summary>Adds <paramref name="ticks"/> ticks.</summary>
    public static Transformer<T> AddTicks<T>(this Transformer<T> t, long ticks) => t.AddTransformer(TransformerRules.AddTicks<T>(ticks));

    /// <summary>Adds the <see cref="TimeSpan"/> <paramref name="value"/> to the current value.</summary>
    public static Transformer<T> Add<T>(this Transformer<T> t, TimeSpan value) => t.AddTransformer(TransformerRules.Add<T>(value));

    /// <summary>Subtracts the <see cref="TimeSpan"/> <paramref name="value"/> from the current value.</summary>
    public static Transformer<T> Subtract<T>(this Transformer<T> t, TimeSpan value) => t.AddTransformer(TransformerRules.Subtract<T>(value));

    /// <summary>Negates the current <see cref="TimeSpan"/> (flips its sign).</summary>
    public static Transformer<TimeSpan> Negate(this Transformer<TimeSpan> t) => t.AddTransformer(TransformerRules.Negate());

    /// <summary>Takes the absolute duration of the current <see cref="TimeSpan"/>.</summary>
    public static Transformer<TimeSpan> Duration(this Transformer<TimeSpan> t) => t.AddTransformer(TransformerRules.Duration());

    // =============================
    // Date/time — clamp
    // =============================

    /// <summary>Clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<DateTime> Clamp(this Transformer<DateTime> t, DateTime min, DateTime max) => t.AddTransformer(TransformerRules.Clamp(min, max));

    /// <summary>Clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<DateTimeOffset> Clamp(this Transformer<DateTimeOffset> t, DateTimeOffset min, DateTimeOffset max) => t.AddTransformer(TransformerRules.Clamp(min, max));

    /// <summary>Clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<TimeSpan> Clamp(this Transformer<TimeSpan> t, TimeSpan min, TimeSpan max) => t.AddTransformer(TransformerRules.Clamp(min, max));

    /// <summary>Clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<DateOnly> Clamp(this Transformer<DateOnly> t, DateOnly min, DateOnly max) => t.AddTransformer(TransformerRules.Clamp(min, max));

    /// <summary>Clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static Transformer<TimeOnly> Clamp(this Transformer<TimeOnly> t, TimeOnly min, TimeOnly max) => t.AddTransformer(TransformerRules.Clamp(min, max));

    // =============================
    // Date/time — start / end of a calendar period
    // =============================

    /// <summary>Snaps to the start of the day (midnight).</summary>
    public static Transformer<T> StartOfDay<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.StartOfDay<T>());

    /// <summary>Snaps to the last instant of the day.</summary>
    public static Transformer<T> EndOfDay<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.EndOfDay<T>());

    /// <summary>Snaps back to <paramref name="firstDayOfWeek"/>.</summary>
    public static Transformer<T> StartOfWeek<T>(this Transformer<T> t, DayOfWeek firstDayOfWeek) => t.AddTransformer(TransformerRules.StartOfWeek<T>(firstDayOfWeek));

    /// <summary>Snaps forward to the end of the week beginning on <paramref name="firstDayOfWeek"/>.</summary>
    public static Transformer<T> EndOfWeek<T>(this Transformer<T> t, DayOfWeek firstDayOfWeek) => t.AddTransformer(TransformerRules.EndOfWeek<T>(firstDayOfWeek));

    /// <summary>Snaps to the first day of the month.</summary>
    public static Transformer<T> StartOfMonth<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.StartOfMonth<T>());

    /// <summary>Snaps to the end of the month.</summary>
    public static Transformer<T> EndOfMonth<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.EndOfMonth<T>());

    /// <summary>Snaps to the first day of the calendar quarter.</summary>
    public static Transformer<T> StartOfQuarter<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.StartOfQuarter<T>());

    /// <summary>Snaps to the end of the calendar quarter.</summary>
    public static Transformer<T> EndOfQuarter<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.EndOfQuarter<T>());

    /// <summary>Snaps to the first day of the year.</summary>
    public static Transformer<T> StartOfYear<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.StartOfYear<T>());

    /// <summary>Snaps to the end of the year.</summary>
    public static Transformer<T> EndOfYear<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.EndOfYear<T>());

    // =============================
    // Date/time — rounding to an interval
    // =============================

    /// <summary>Snaps to the nearest multiple of <paramref name="interval"/> (halves round up).</summary>
    public static Transformer<T> RoundTo<T>(this Transformer<T> t, TimeSpan interval) => t.AddTransformer(TransformerRules.RoundTo<T>(interval));

    /// <summary>Snaps down to a multiple of <paramref name="interval"/> (toward zero).</summary>
    public static Transformer<T> TruncateTo<T>(this Transformer<T> t, TimeSpan interval) => t.AddTransformer(TransformerRules.TruncateTo<T>(interval));

    /// <summary>Snaps up to a multiple of <paramref name="interval"/>.</summary>
    public static Transformer<T> CeilingTo<T>(this Transformer<T> t, TimeSpan interval) => t.AddTransformer(TransformerRules.CeilingTo<T>(interval));

    // =============================
    // Date/time — component replacement ("with" setters)
    // =============================

    /// <summary>Replaces the year.</summary>
    public static Transformer<T> WithYear<T>(this Transformer<T> t, int year) => t.AddTransformer(TransformerRules.WithYear<T>(year));

    /// <summary>Replaces the month (1-12).</summary>
    public static Transformer<T> WithMonth<T>(this Transformer<T> t, int month) => t.AddTransformer(TransformerRules.WithMonth<T>(month));

    /// <summary>Replaces the day of the month.</summary>
    public static Transformer<T> WithDay<T>(this Transformer<T> t, int day) => t.AddTransformer(TransformerRules.WithDay<T>(day));

    /// <summary>Replaces the hour (0-23).</summary>
    public static Transformer<T> WithHour<T>(this Transformer<T> t, int hour) => t.AddTransformer(TransformerRules.WithHour<T>(hour));

    /// <summary>Replaces the minute (0-59).</summary>
    public static Transformer<T> WithMinute<T>(this Transformer<T> t, int minute) => t.AddTransformer(TransformerRules.WithMinute<T>(minute));

    /// <summary>Replaces the second (0-59).</summary>
    public static Transformer<T> WithSecond<T>(this Transformer<T> t, int second) => t.AddTransformer(TransformerRules.WithSecond<T>(second));

    /// <summary>Replaces the millisecond (0-999).</summary>
    public static Transformer<T> WithMillisecond<T>(this Transformer<T> t, int millisecond) => t.AddTransformer(TransformerRules.WithMillisecond<T>(millisecond));

    /// <summary>Replaces the time of day, keeping the date.</summary>
    public static Transformer<T> SetTimeOfDay<T>(this Transformer<T> t, TimeOnly timeOfDay) => t.AddTransformer(TransformerRules.SetTimeOfDay<T>(timeOfDay));

    /// <summary>Replaces the date, keeping the time of day.</summary>
    public static Transformer<T> SetDate<T>(this Transformer<T> t, DateOnly date) => t.AddTransformer(TransformerRules.SetDate<T>(date));

    // =============================
    // Date/time — calendar navigation
    // =============================

    /// <summary>Moves forward to the next <paramref name="dayOfWeek"/> (strictly after the current day).</summary>
    public static Transformer<T> NextDayOfWeek<T>(this Transformer<T> t, DayOfWeek dayOfWeek) => t.AddTransformer(TransformerRules.NextDayOfWeek<T>(dayOfWeek));

    /// <summary>Moves back to the previous <paramref name="dayOfWeek"/> (strictly before the current day).</summary>
    public static Transformer<T> PreviousDayOfWeek<T>(this Transformer<T> t, DayOfWeek dayOfWeek) => t.AddTransformer(TransformerRules.PreviousDayOfWeek<T>(dayOfWeek));

    /// <summary>Adds <paramref name="count"/> business days, skipping weekends and any <paramref name="holidays"/>.</summary>
    public static Transformer<T> AddBusinessDays<T>(this Transformer<T> t, int count, IEnumerable<DateOnly>? holidays = null) => t.AddTransformer(TransformerRules.AddBusinessDays<T>(count, holidays));

    /// <summary>Moves to the next business day, skipping weekends and any <paramref name="holidays"/>.</summary>
    public static Transformer<T> NextBusinessDay<T>(this Transformer<T> t, IEnumerable<DateOnly>? holidays = null) => t.AddTransformer(TransformerRules.NextBusinessDay<T>(holidays));

    /// <summary>Moves to the previous business day, skipping weekends and any <paramref name="holidays"/>.</summary>
    public static Transformer<T> PreviousBusinessDay<T>(this Transformer<T> t, IEnumerable<DateOnly>? holidays = null) => t.AddTransformer(TransformerRules.PreviousBusinessDay<T>(holidays));

    // =============================
    // Date/time — kind / time zone
    // =============================

    /// <summary>Converts to Coordinated Universal Time.</summary>
    public static Transformer<T> ToUniversalTime<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.ToUniversalTime<T>());

    /// <summary>Converts to local time.</summary>
    public static Transformer<T> ToLocalTime<T>(this Transformer<T> t) => t.AddTransformer(TransformerRules.ToLocalTime<T>());

    /// <summary>Stamps the current <see cref="DateTime"/> with <paramref name="kind"/> without shifting the clock value.</summary>
    public static Transformer<DateTime> SpecifyKind(this Transformer<DateTime> t, DateTimeKind kind) => t.AddTransformer(TransformerRules.SpecifyKind(kind));

    /// <summary>Adjusts the current <see cref="DateTimeOffset"/> to <paramref name="offset"/>, preserving the instant.</summary>
    public static Transformer<DateTimeOffset> ToOffset(this Transformer<DateTimeOffset> t, TimeSpan offset) => t.AddTransformer(TransformerRules.ToOffset(offset));

    /// <summary>Converts the current <see cref="DateTimeOffset"/> into <paramref name="timeZone"/>, preserving the instant.</summary>
    public static Transformer<DateTimeOffset> ConvertToTimeZone(this Transformer<DateTimeOffset> t, TimeZoneInfo timeZone) => t.AddTransformer(TransformerRules.ConvertToTimeZone(timeZone));

    // =============================
    // Date/time — sentinel defaults
    // =============================

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTime.MinValue"/>.</summary>
    public static Transformer<DateTime> DefaultIfMinValue(this Transformer<DateTime> t, DateTime defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTimeOffset.MinValue"/>.</summary>
    public static Transformer<DateTimeOffset> DefaultIfMinValue(this Transformer<DateTimeOffset> t, DateTimeOffset defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeSpan.MinValue"/>.</summary>
    public static Transformer<TimeSpan> DefaultIfMinValue(this Transformer<TimeSpan> t, TimeSpan defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateOnly.MinValue"/>.</summary>
    public static Transformer<DateOnly> DefaultIfMinValue(this Transformer<DateOnly> t, DateOnly defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeOnly.MinValue"/>.</summary>
    public static Transformer<TimeOnly> DefaultIfMinValue(this Transformer<TimeOnly> t, TimeOnly defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTime.MaxValue"/>.</summary>
    public static Transformer<DateTime> DefaultIfMaxValue(this Transformer<DateTime> t, DateTime defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTimeOffset.MaxValue"/>.</summary>
    public static Transformer<DateTimeOffset> DefaultIfMaxValue(this Transformer<DateTimeOffset> t, DateTimeOffset defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeSpan.MaxValue"/>.</summary>
    public static Transformer<TimeSpan> DefaultIfMaxValue(this Transformer<TimeSpan> t, TimeSpan defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateOnly.MaxValue"/>.</summary>
    public static Transformer<DateOnly> DefaultIfMaxValue(this Transformer<DateOnly> t, DateOnly defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeOnly.MaxValue"/>.</summary>
    public static Transformer<TimeOnly> DefaultIfMaxValue(this Transformer<TimeOnly> t, TimeOnly defaultValue) => t.AddTransformer(TransformerRules.DefaultIfMaxValue(defaultValue));
}
