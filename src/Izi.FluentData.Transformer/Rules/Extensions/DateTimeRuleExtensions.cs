namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a same-type catalogue rule and return the property transformer.
/// </summary>
public static partial class TransformerRuleExtensions
{
    /// <summary>Appends AddYears for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="years">Years to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> AddYears<T>(this PropertyTransformer<T, DateTime> rule, int years)
        => rule.Add(TransformerRules.AddYears<DateTime>(years));

    /// <summary>Appends AddYears for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="years">Years to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> AddYears<T>(this PropertyTransformer<T, DateTimeOffset> rule, int years)
        => rule.Add(TransformerRules.AddYears<DateTimeOffset>(years));

    /// <summary>Appends AddYears for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="years">Years to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> AddYears<T>(this PropertyTransformer<T, DateOnly> rule, int years)
        => rule.Add(TransformerRules.AddYears<DateOnly>(years));

    /// <summary>Appends AddMonths for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="months">Months to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> AddMonths<T>(this PropertyTransformer<T, DateTime> rule, int months)
        => rule.Add(TransformerRules.AddMonths<DateTime>(months));

    /// <summary>Appends AddMonths for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="months">Months to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> AddMonths<T>(this PropertyTransformer<T, DateTimeOffset> rule, int months)
        => rule.Add(TransformerRules.AddMonths<DateTimeOffset>(months));

    /// <summary>Appends AddMonths for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="months">Months to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> AddMonths<T>(this PropertyTransformer<T, DateOnly> rule, int months)
        => rule.Add(TransformerRules.AddMonths<DateOnly>(months));

    /// <summary>Appends AddDays for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="days">Days to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="days"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTime> AddDays<T>(this PropertyTransformer<T, DateTime> rule, double days)
        => rule.Add(TransformerRules.AddDays<DateTime>(days));

    /// <summary>Appends AddDays for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="days">Days to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="days"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTimeOffset> AddDays<T>(this PropertyTransformer<T, DateTimeOffset> rule, double days)
        => rule.Add(TransformerRules.AddDays<DateTimeOffset>(days));

    /// <summary>Appends AddDays for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="days">Days to add. A negative count moves backward. The fraction is truncated toward zero.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="days"/> is not finite.</exception>
    public static PropertyTransformer<T, DateOnly> AddDays<T>(this PropertyTransformer<T, DateOnly> rule, double days)
        => rule.Add(TransformerRules.AddDays<DateOnly>(days));

    /// <summary>Appends AddDays for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="days">Days to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="days"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeSpan> AddDays<T>(this PropertyTransformer<T, TimeSpan> rule, double days)
        => rule.Add(TransformerRules.AddDays<TimeSpan>(days));

    /// <summary>Appends AddHours for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hours">Hours to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hours"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTime> AddHours<T>(this PropertyTransformer<T, DateTime> rule, double hours)
        => rule.Add(TransformerRules.AddHours<DateTime>(hours));

    /// <summary>Appends AddHours for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hours">Hours to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hours"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTimeOffset> AddHours<T>(this PropertyTransformer<T, DateTimeOffset> rule, double hours)
        => rule.Add(TransformerRules.AddHours<DateTimeOffset>(hours));

    /// <summary>Appends AddHours for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hours">Hours to add. A negative count moves backward. TimeOnly wraps past midnight.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hours"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeOnly> AddHours<T>(this PropertyTransformer<T, TimeOnly> rule, double hours)
        => rule.Add(TransformerRules.AddHours<TimeOnly>(hours));

    /// <summary>Appends AddHours for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hours">Hours to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hours"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeSpan> AddHours<T>(this PropertyTransformer<T, TimeSpan> rule, double hours)
        => rule.Add(TransformerRules.AddHours<TimeSpan>(hours));

    /// <summary>Appends AddMinutes for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minutes">Minutes to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minutes"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTime> AddMinutes<T>(this PropertyTransformer<T, DateTime> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<DateTime>(minutes));

    /// <summary>Appends AddMinutes for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minutes">Minutes to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minutes"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTimeOffset> AddMinutes<T>(this PropertyTransformer<T, DateTimeOffset> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<DateTimeOffset>(minutes));

    /// <summary>Appends AddMinutes for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minutes">Minutes to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minutes"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeOnly> AddMinutes<T>(this PropertyTransformer<T, TimeOnly> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<TimeOnly>(minutes));

    /// <summary>Appends AddMinutes for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minutes">Minutes to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minutes"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeSpan> AddMinutes<T>(this PropertyTransformer<T, TimeSpan> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<TimeSpan>(minutes));

    /// <summary>Appends AddSeconds for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="seconds">Seconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="seconds"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTime> AddSeconds<T>(this PropertyTransformer<T, DateTime> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<DateTime>(seconds));

    /// <summary>Appends AddSeconds for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="seconds">Seconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="seconds"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTimeOffset> AddSeconds<T>(this PropertyTransformer<T, DateTimeOffset> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<DateTimeOffset>(seconds));

    /// <summary>Appends AddSeconds for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="seconds">Seconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="seconds"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeOnly> AddSeconds<T>(this PropertyTransformer<T, TimeOnly> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<TimeOnly>(seconds));

    /// <summary>Appends AddSeconds for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="seconds">Seconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="seconds"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeSpan> AddSeconds<T>(this PropertyTransformer<T, TimeSpan> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<TimeSpan>(seconds));

    /// <summary>Appends AddMilliseconds for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="milliseconds">Milliseconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTime> AddMilliseconds<T>(this PropertyTransformer<T, DateTime> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<DateTime>(milliseconds));

    /// <summary>Appends AddMilliseconds for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="milliseconds">Milliseconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is not finite.</exception>
    public static PropertyTransformer<T, DateTimeOffset> AddMilliseconds<T>(this PropertyTransformer<T, DateTimeOffset> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<DateTimeOffset>(milliseconds));

    /// <summary>Appends AddMilliseconds for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="milliseconds">Milliseconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeOnly> AddMilliseconds<T>(this PropertyTransformer<T, TimeOnly> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<TimeOnly>(milliseconds));

    /// <summary>Appends AddMilliseconds for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="milliseconds">Milliseconds to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is not finite.</exception>
    public static PropertyTransformer<T, TimeSpan> AddMilliseconds<T>(this PropertyTransformer<T, TimeSpan> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<TimeSpan>(milliseconds));

    /// <summary>Appends AddTicks for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="ticks">Ticks to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> AddTicks<T>(this PropertyTransformer<T, DateTime> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<DateTime>(ticks));

    /// <summary>Appends AddTicks for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="ticks">Ticks to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> AddTicks<T>(this PropertyTransformer<T, DateTimeOffset> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<DateTimeOffset>(ticks));

    /// <summary>Appends AddTicks for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="ticks">Ticks to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeOnly> AddTicks<T>(this PropertyTransformer<T, TimeOnly> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<TimeOnly>(ticks));

    /// <summary>Appends AddTicks for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="ticks">Ticks to add. A negative count moves backward.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> AddTicks<T>(this PropertyTransformer<T, TimeSpan> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<TimeSpan>(ticks));

    /// <summary>Appends Add for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to add.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> Add<T>(this PropertyTransformer<T, DateTime> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<DateTime>(value));

    /// <summary>Appends Add for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to add.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> Add<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<DateTimeOffset>(value));

    /// <summary>Appends Add for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to add.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeOnly> Add<T>(this PropertyTransformer<T, TimeOnly> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<TimeOnly>(value));

    /// <summary>Appends Add for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to add.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> Add<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<TimeSpan>(value));

    /// <summary>Appends Subtract for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to subtract.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> Subtract<T>(this PropertyTransformer<T, DateTime> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<DateTime>(value));

    /// <summary>Appends Subtract for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to subtract.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> Subtract<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<DateTimeOffset>(value));

    /// <summary>Appends Subtract for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to subtract.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeOnly> Subtract<T>(this PropertyTransformer<T, TimeOnly> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<TimeOnly>(value));

    /// <summary>Appends Subtract for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="value">Duration to subtract.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> Subtract<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<TimeSpan>(value));

    /// <summary>Appends StartOfDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> StartOfDay<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.StartOfDay);

    /// <summary>Appends StartOfDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> StartOfDay<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.StartOfDay);

    /// <summary>Appends EndOfDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> EndOfDay<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.EndOfDay);

    /// <summary>Appends EndOfDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> EndOfDay<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.EndOfDay);

    /// <summary>Appends StartOfMonth for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> StartOfMonth<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.StartOfMonth);

    /// <summary>Appends StartOfMonth for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> StartOfMonth<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.StartOfMonth);

    /// <summary>Appends StartOfMonth for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> StartOfMonth<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.StartOfMonth);

    /// <summary>Appends EndOfMonth for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> EndOfMonth<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.EndOfMonth);

    /// <summary>Appends EndOfMonth for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> EndOfMonth<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.EndOfMonth);

    /// <summary>Appends EndOfMonth for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> EndOfMonth<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.EndOfMonth);

    /// <summary>Appends StartOfQuarter for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> StartOfQuarter<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.StartOfQuarter);

    /// <summary>Appends StartOfQuarter for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> StartOfQuarter<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.StartOfQuarter);

    /// <summary>Appends StartOfQuarter for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> StartOfQuarter<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.StartOfQuarter);

    /// <summary>Appends EndOfQuarter for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> EndOfQuarter<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.EndOfQuarter);

    /// <summary>Appends EndOfQuarter for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> EndOfQuarter<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.EndOfQuarter);

    /// <summary>Appends EndOfQuarter for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> EndOfQuarter<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.EndOfQuarter);

    /// <summary>Appends StartOfYear for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> StartOfYear<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.StartOfYear);

    /// <summary>Appends StartOfYear for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> StartOfYear<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.StartOfYear);

    /// <summary>Appends StartOfYear for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> StartOfYear<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.StartOfYear);

    /// <summary>Appends EndOfYear for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> EndOfYear<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.EndOfYear);

    /// <summary>Appends EndOfYear for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> EndOfYear<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.EndOfYear);

    /// <summary>Appends EndOfYear for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> EndOfYear<T>(this PropertyTransformer<T, DateOnly> rule)
        => rule.Add(DateCache<DateOnly>.EndOfYear);

    /// <summary>Appends ToUniversalTime for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> ToUniversalTime<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.ToUniversalTime);

    /// <summary>Appends ToUniversalTime for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> ToUniversalTime<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.ToUniversalTime);

    /// <summary>Appends ToLocalTime for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> ToLocalTime<T>(this PropertyTransformer<T, DateTime> rule)
        => rule.Add(DateCache<DateTime>.ToLocalTime);

    /// <summary>Appends ToLocalTime for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> ToLocalTime<T>(this PropertyTransformer<T, DateTimeOffset> rule)
        => rule.Add(DateCache<DateTimeOffset>.ToLocalTime);

    /// <summary>Appends StartOfWeek for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> StartOfWeek<T>(this PropertyTransformer<T, DateTime> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.StartOfWeek<DateTime>(firstDayOfWeek));

    /// <summary>Appends StartOfWeek for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> StartOfWeek<T>(this PropertyTransformer<T, DateTimeOffset> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.StartOfWeek<DateTimeOffset>(firstDayOfWeek));

    /// <summary>Appends StartOfWeek for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> StartOfWeek<T>(this PropertyTransformer<T, DateOnly> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.StartOfWeek<DateOnly>(firstDayOfWeek));

    /// <summary>Appends EndOfWeek for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> EndOfWeek<T>(this PropertyTransformer<T, DateTime> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.EndOfWeek<DateTime>(firstDayOfWeek));

    /// <summary>Appends EndOfWeek for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> EndOfWeek<T>(this PropertyTransformer<T, DateTimeOffset> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.EndOfWeek<DateTimeOffset>(firstDayOfWeek));

    /// <summary>Appends EndOfWeek for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="firstDayOfWeek">Day the week starts on.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> EndOfWeek<T>(this PropertyTransformer<T, DateOnly> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.EndOfWeek<DateOnly>(firstDayOfWeek));

    /// <summary>Appends RoundTo for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTime> RoundTo<T>(this PropertyTransformer<T, DateTime> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<DateTime>(interval));

    /// <summary>Appends RoundTo for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTimeOffset> RoundTo<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<DateTimeOffset>(interval));

    /// <summary>Appends RoundTo for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeOnly> RoundTo<T>(this PropertyTransformer<T, TimeOnly> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<TimeOnly>(interval));

    /// <summary>Appends RoundTo for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeSpan> RoundTo<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<TimeSpan>(interval));

    /// <summary>Appends TruncateTo for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTime> TruncateTo<T>(this PropertyTransformer<T, DateTime> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<DateTime>(interval));

    /// <summary>Appends TruncateTo for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTimeOffset> TruncateTo<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<DateTimeOffset>(interval));

    /// <summary>Appends TruncateTo for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeOnly> TruncateTo<T>(this PropertyTransformer<T, TimeOnly> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<TimeOnly>(interval));

    /// <summary>Appends TruncateTo for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeSpan> TruncateTo<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<TimeSpan>(interval));

    /// <summary>Appends CeilingTo for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTime> CeilingTo<T>(this PropertyTransformer<T, DateTime> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<DateTime>(interval));

    /// <summary>Appends CeilingTo for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, DateTimeOffset> CeilingTo<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<DateTimeOffset>(interval));

    /// <summary>Appends CeilingTo for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeOnly> CeilingTo<T>(this PropertyTransformer<T, TimeOnly> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<TimeOnly>(interval));

    /// <summary>Appends CeilingTo for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="interval">Positive duration the result must be a multiple of.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is not positive.</exception>
    public static PropertyTransformer<T, TimeSpan> CeilingTo<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<TimeSpan>(interval));

    /// <summary>Appends WithYear for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="year">Year to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> WithYear<T>(this PropertyTransformer<T, DateTime> rule, int year)
        => rule.Add(TransformerRules.WithYear<DateTime>(year));

    /// <summary>Appends WithYear for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="year">Year to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> WithYear<T>(this PropertyTransformer<T, DateTimeOffset> rule, int year)
        => rule.Add(TransformerRules.WithYear<DateTimeOffset>(year));

    /// <summary>Appends WithYear for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="year">Year to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> WithYear<T>(this PropertyTransformer<T, DateOnly> rule, int year)
        => rule.Add(TransformerRules.WithYear<DateOnly>(year));

    /// <summary>Appends WithMonth for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="month">Month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> WithMonth<T>(this PropertyTransformer<T, DateTime> rule, int month)
        => rule.Add(TransformerRules.WithMonth<DateTime>(month));

    /// <summary>Appends WithMonth for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="month">Month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> WithMonth<T>(this PropertyTransformer<T, DateTimeOffset> rule, int month)
        => rule.Add(TransformerRules.WithMonth<DateTimeOffset>(month));

    /// <summary>Appends WithMonth for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="month">Month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> WithMonth<T>(this PropertyTransformer<T, DateOnly> rule, int month)
        => rule.Add(TransformerRules.WithMonth<DateOnly>(month));

    /// <summary>Appends WithDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="day">Day of the month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> WithDay<T>(this PropertyTransformer<T, DateTime> rule, int day)
        => rule.Add(TransformerRules.WithDay<DateTime>(day));

    /// <summary>Appends WithDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="day">Day of the month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> WithDay<T>(this PropertyTransformer<T, DateTimeOffset> rule, int day)
        => rule.Add(TransformerRules.WithDay<DateTimeOffset>(day));

    /// <summary>Appends WithDay for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="day">Day of the month to write. The other components are kept.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> WithDay<T>(this PropertyTransformer<T, DateOnly> rule, int day)
        => rule.Add(TransformerRules.WithDay<DateOnly>(day));

    /// <summary>Appends WithHour for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hour">Hour to write, from 0 through 23.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hour"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTime> WithHour<T>(this PropertyTransformer<T, DateTime> rule, int hour)
        => rule.Add(TransformerRules.WithHour<DateTime>(hour));

    /// <summary>Appends WithHour for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hour">Hour to write, from 0 through 23.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hour"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTimeOffset> WithHour<T>(this PropertyTransformer<T, DateTimeOffset> rule, int hour)
        => rule.Add(TransformerRules.WithHour<DateTimeOffset>(hour));

    /// <summary>Appends WithHour for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="hour">Hour to write, from 0 through 23.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hour"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, TimeOnly> WithHour<T>(this PropertyTransformer<T, TimeOnly> rule, int hour)
        => rule.Add(TransformerRules.WithHour<TimeOnly>(hour));

    /// <summary>Appends WithMinute for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minute">Minute to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minute"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTime> WithMinute<T>(this PropertyTransformer<T, DateTime> rule, int minute)
        => rule.Add(TransformerRules.WithMinute<DateTime>(minute));

    /// <summary>Appends WithMinute for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minute">Minute to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minute"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTimeOffset> WithMinute<T>(this PropertyTransformer<T, DateTimeOffset> rule, int minute)
        => rule.Add(TransformerRules.WithMinute<DateTimeOffset>(minute));

    /// <summary>Appends WithMinute for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="minute">Minute to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minute"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, TimeOnly> WithMinute<T>(this PropertyTransformer<T, TimeOnly> rule, int minute)
        => rule.Add(TransformerRules.WithMinute<TimeOnly>(minute));

    /// <summary>Appends WithSecond for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="second">Second to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="second"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTime> WithSecond<T>(this PropertyTransformer<T, DateTime> rule, int second)
        => rule.Add(TransformerRules.WithSecond<DateTime>(second));

    /// <summary>Appends WithSecond for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="second">Second to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="second"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTimeOffset> WithSecond<T>(this PropertyTransformer<T, DateTimeOffset> rule, int second)
        => rule.Add(TransformerRules.WithSecond<DateTimeOffset>(second));

    /// <summary>Appends WithSecond for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="second">Second to write, from 0 through 59.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="second"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, TimeOnly> WithSecond<T>(this PropertyTransformer<T, TimeOnly> rule, int second)
        => rule.Add(TransformerRules.WithSecond<TimeOnly>(second));

    /// <summary>Appends WithMillisecond for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="millisecond">Millisecond to write, from 0 through 999.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecond"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTime> WithMillisecond<T>(this PropertyTransformer<T, DateTime> rule, int millisecond)
        => rule.Add(TransformerRules.WithMillisecond<DateTime>(millisecond));

    /// <summary>Appends WithMillisecond for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="millisecond">Millisecond to write, from 0 through 999.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecond"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, DateTimeOffset> WithMillisecond<T>(this PropertyTransformer<T, DateTimeOffset> rule, int millisecond)
        => rule.Add(TransformerRules.WithMillisecond<DateTimeOffset>(millisecond));

    /// <summary>Appends WithMillisecond for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="millisecond">Millisecond to write, from 0 through 999.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecond"/> is outside the accepted range.</exception>
    public static PropertyTransformer<T, TimeOnly> WithMillisecond<T>(this PropertyTransformer<T, TimeOnly> rule, int millisecond)
        => rule.Add(TransformerRules.WithMillisecond<TimeOnly>(millisecond));

    /// <summary>Appends SetTimeOfDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="timeOfDay">Time of day that replaces the current time.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> SetTimeOfDay<T>(this PropertyTransformer<T, DateTime> rule, TimeOnly timeOfDay)
        => rule.Add(TransformerRules.SetTimeOfDay<DateTime>(timeOfDay));

    /// <summary>Appends SetTimeOfDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="timeOfDay">Time of day that replaces the current time.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> SetTimeOfDay<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeOnly timeOfDay)
        => rule.Add(TransformerRules.SetTimeOfDay<DateTimeOffset>(timeOfDay));

    /// <summary>Appends SetDate for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="date">Date that replaces the current date.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> SetDate<T>(this PropertyTransformer<T, DateTime> rule, DateOnly date)
        => rule.Add(TransformerRules.SetDate<DateTime>(date));

    /// <summary>Appends SetDate for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="date">Date that replaces the current date.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> SetDate<T>(this PropertyTransformer<T, DateTimeOffset> rule, DateOnly date)
        => rule.Add(TransformerRules.SetDate<DateTimeOffset>(date));

    /// <summary>Appends NextDayOfWeek for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> NextDayOfWeek<T>(this PropertyTransformer<T, DateTime> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.NextDayOfWeek<DateTime>(dayOfWeek));

    /// <summary>Appends NextDayOfWeek for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> NextDayOfWeek<T>(this PropertyTransformer<T, DateTimeOffset> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.NextDayOfWeek<DateTimeOffset>(dayOfWeek));

    /// <summary>Appends NextDayOfWeek for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> NextDayOfWeek<T>(this PropertyTransformer<T, DateOnly> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.NextDayOfWeek<DateOnly>(dayOfWeek));

    /// <summary>Appends PreviousDayOfWeek for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> PreviousDayOfWeek<T>(this PropertyTransformer<T, DateTime> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.PreviousDayOfWeek<DateTime>(dayOfWeek));

    /// <summary>Appends PreviousDayOfWeek for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> PreviousDayOfWeek<T>(this PropertyTransformer<T, DateTimeOffset> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.PreviousDayOfWeek<DateTimeOffset>(dayOfWeek));

    /// <summary>Appends PreviousDayOfWeek for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="dayOfWeek">Target day of the week. The current day is not left in place.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> PreviousDayOfWeek<T>(this PropertyTransformer<T, DateOnly> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.PreviousDayOfWeek<DateOnly>(dayOfWeek));

    /// <summary>Appends AddBusinessDays for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="count">Business days to move. Positive moves forward and negative moves backward.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> AddBusinessDays<T>(this PropertyTransformer<T, DateTime> rule, int count, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.AddBusinessDays<DateTime>(count, holidays));

    /// <summary>Appends AddBusinessDays for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="count">Business days to move. Positive moves forward and negative moves backward.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> AddBusinessDays<T>(this PropertyTransformer<T, DateTimeOffset> rule, int count, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.AddBusinessDays<DateTimeOffset>(count, holidays));

    /// <summary>Appends AddBusinessDays for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="count">Business days to move. Positive moves forward and negative moves backward.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> AddBusinessDays<T>(this PropertyTransformer<T, DateOnly> rule, int count, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.AddBusinessDays<DateOnly>(count, holidays));

    /// <summary>Appends NextBusinessDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> NextBusinessDay<T>(this PropertyTransformer<T, DateTime> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.NextBusinessDay<DateTime>(holidays));

    /// <summary>Appends NextBusinessDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> NextBusinessDay<T>(this PropertyTransformer<T, DateTimeOffset> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.NextBusinessDay<DateTimeOffset>(holidays));

    /// <summary>Appends NextBusinessDay for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> NextBusinessDay<T>(this PropertyTransformer<T, DateOnly> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.NextBusinessDay<DateOnly>(holidays));

    /// <summary>Appends PreviousBusinessDay for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> PreviousBusinessDay<T>(this PropertyTransformer<T, DateTime> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.PreviousBusinessDay<DateTime>(holidays));

    /// <summary>Appends PreviousBusinessDay for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> PreviousBusinessDay<T>(this PropertyTransformer<T, DateTimeOffset> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.PreviousBusinessDay<DateTimeOffset>(holidays));

    /// <summary>Appends PreviousBusinessDay for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="holidays">Extra dates to skip, besides weekends. Null means weekends only.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> PreviousBusinessDay<T>(this PropertyTransformer<T, DateOnly> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.PreviousBusinessDay<DateOnly>(holidays));

    /// <summary>Appends Negate for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> Negate<T>(this PropertyTransformer<T, TimeSpan> rule)
        => rule.Add(TimeSpanRules.Negate);

    /// <summary>Appends Duration for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> Duration<T>(this PropertyTransformer<T, TimeSpan> rule)
        => rule.Add(TimeSpanRules.Duration);

    /// <summary>Appends Clamp for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="min">Inclusive lower bound.</param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> Clamp<T>(this PropertyTransformer<T, DateTime> rule, DateTime min, DateTime max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends Clamp for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="min">Inclusive lower bound.</param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTimeOffset> Clamp<T>(this PropertyTransformer<T, DateTimeOffset> rule, DateTimeOffset min, DateTimeOffset max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends Clamp for a <see cref="TimeSpan"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="min">Inclusive lower bound.</param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeSpan> Clamp<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan min, TimeSpan max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends Clamp for a <see cref="DateOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="min">Inclusive lower bound.</param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateOnly> Clamp<T>(this PropertyTransformer<T, DateOnly> rule, DateOnly min, DateOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends Clamp for a <see cref="TimeOnly"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="min">Inclusive lower bound.</param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, TimeOnly> Clamp<T>(this PropertyTransformer<T, TimeOnly> rule, TimeOnly min, TimeOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends SpecifyKind for a <see cref="DateTime"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="kind">Kind stored on the result. The clock value is not shifted.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    public static PropertyTransformer<T, DateTime> SpecifyKind<T>(this PropertyTransformer<T, DateTime> rule, DateTimeKind kind)
        => rule.Add(TransformerRules.SpecifyKind(kind));

    /// <summary>Appends ToOffset for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="offset">Offset applied to the value.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentException"><paramref name="offset"/> is not a whole number of minutes.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is outside plus or minus 14 hours.</exception>
    public static PropertyTransformer<T, DateTimeOffset> ToOffset<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan offset)
        => rule.Add(TransformerRules.ToOffset(offset));

    /// <summary>Appends ConvertToTimeZone for a <see cref="DateTimeOffset"/> property.</summary>
    /// <typeparam name="T">The object that owns the property.</typeparam>
    /// <param name="rule">The property transformer the rule is appended to.</param>
    /// <param name="timeZone">Destination time zone. The instant is preserved.</param>
    /// <returns>The same property transformer, so another rule can be appended.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="timeZone"/> is <see langword="null"/>.</exception>
    public static PropertyTransformer<T, DateTimeOffset> ConvertToTimeZone<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeZoneInfo timeZone)
        => rule.Add(TransformerRules.ConvertToTimeZone(timeZone));

    // The closed property type is not known until the first call, so the rule is created then and reused.
    private static class DateCache<TProperty>
    {
        private static TransformerRule<TProperty>? _startOfDay;
        private static TransformerRule<TProperty>? _endOfDay;
        private static TransformerRule<TProperty>? _startOfMonth;
        private static TransformerRule<TProperty>? _endOfMonth;
        private static TransformerRule<TProperty>? _startOfQuarter;
        private static TransformerRule<TProperty>? _endOfQuarter;
        private static TransformerRule<TProperty>? _startOfYear;
        private static TransformerRule<TProperty>? _endOfYear;
        private static TransformerRule<TProperty>? _toUniversalTime;
        private static TransformerRule<TProperty>? _toLocalTime;

        public static TransformerRule<TProperty> StartOfDay =>
            _startOfDay ??= TransformerRules.StartOfDay<TProperty>();

        public static TransformerRule<TProperty> EndOfDay =>
            _endOfDay ??= TransformerRules.EndOfDay<TProperty>();

        public static TransformerRule<TProperty> StartOfMonth =>
            _startOfMonth ??= TransformerRules.StartOfMonth<TProperty>();

        public static TransformerRule<TProperty> EndOfMonth =>
            _endOfMonth ??= TransformerRules.EndOfMonth<TProperty>();

        public static TransformerRule<TProperty> StartOfQuarter =>
            _startOfQuarter ??= TransformerRules.StartOfQuarter<TProperty>();

        public static TransformerRule<TProperty> EndOfQuarter =>
            _endOfQuarter ??= TransformerRules.EndOfQuarter<TProperty>();

        public static TransformerRule<TProperty> StartOfYear =>
            _startOfYear ??= TransformerRules.StartOfYear<TProperty>();

        public static TransformerRule<TProperty> EndOfYear =>
            _endOfYear ??= TransformerRules.EndOfYear<TProperty>();

        public static TransformerRule<TProperty> ToUniversalTime =>
            _toUniversalTime ??= TransformerRules.ToUniversalTime<TProperty>();

        public static TransformerRule<TProperty> ToLocalTime =>
            _toLocalTime ??= TransformerRules.ToLocalTime<TProperty>();
    }

    // Negate and Duration take no arguments, so one instance serves every TimeSpan property.
    private static class TimeSpanRules
    {
        public static readonly TransformerRule<TimeSpan> Negate = TransformerRules.Negate();
        public static readonly TransformerRule<TimeSpan> Duration = TransformerRules.Duration();
    }
}
