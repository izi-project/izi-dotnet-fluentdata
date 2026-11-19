namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a same-type catalogue rule and return the property transformer.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue AddYears rule.</summary>
    public static PropertyTransformer<T, TProperty> AddYears<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int years)
        => rule.Add(TransformerRules.AddYears<TProperty>(years));

    /// <summary>Appends the catalogue AddMonths rule.</summary>
    public static PropertyTransformer<T, TProperty> AddMonths<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int months)
        => rule.Add(TransformerRules.AddMonths<TProperty>(months));

    /// <summary>Appends the catalogue AddDays rule.</summary>
    public static PropertyTransformer<T, TProperty> AddDays<T, TProperty>(this PropertyTransformer<T, TProperty> rule, double days)
        => rule.Add(TransformerRules.AddDays<TProperty>(days));

    /// <summary>Appends the catalogue AddHours rule.</summary>
    public static PropertyTransformer<T, TProperty> AddHours<T, TProperty>(this PropertyTransformer<T, TProperty> rule, double hours)
        => rule.Add(TransformerRules.AddHours<TProperty>(hours));

    /// <summary>Appends the catalogue AddMinutes rule.</summary>
    public static PropertyTransformer<T, TProperty> AddMinutes<T, TProperty>(this PropertyTransformer<T, TProperty> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<TProperty>(minutes));

    /// <summary>Appends the catalogue AddSeconds rule.</summary>
    public static PropertyTransformer<T, TProperty> AddSeconds<T, TProperty>(this PropertyTransformer<T, TProperty> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<TProperty>(seconds));

    /// <summary>Appends the catalogue AddMilliseconds rule.</summary>
    public static PropertyTransformer<T, TProperty> AddMilliseconds<T, TProperty>(this PropertyTransformer<T, TProperty> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<TProperty>(milliseconds));

    /// <summary>Appends the catalogue AddTicks rule.</summary>
    public static PropertyTransformer<T, TProperty> AddTicks<T, TProperty>(this PropertyTransformer<T, TProperty> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<TProperty>(ticks));

    /// <summary>Appends the catalogue date or time Add rule.</summary>
    public static PropertyTransformer<T, TProperty> Add<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<TProperty>(value));

    /// <summary>Appends the catalogue date or time Subtract rule.</summary>
    public static PropertyTransformer<T, TProperty> Subtract<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<TProperty>(value));

    /// <summary>Appends the catalogue StartOfDay rule.</summary>
    public static PropertyTransformer<T, TProperty> StartOfDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.StartOfDay);

    /// <summary>Appends the catalogue EndOfDay rule.</summary>
    public static PropertyTransformer<T, TProperty> EndOfDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.EndOfDay);

    /// <summary>Appends the catalogue StartOfMonth rule.</summary>
    public static PropertyTransformer<T, TProperty> StartOfMonth<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.StartOfMonth);

    /// <summary>Appends the catalogue EndOfMonth rule.</summary>
    public static PropertyTransformer<T, TProperty> EndOfMonth<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.EndOfMonth);

    /// <summary>Appends the catalogue StartOfQuarter rule.</summary>
    public static PropertyTransformer<T, TProperty> StartOfQuarter<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.StartOfQuarter);

    /// <summary>Appends the catalogue EndOfQuarter rule.</summary>
    public static PropertyTransformer<T, TProperty> EndOfQuarter<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.EndOfQuarter);

    /// <summary>Appends the catalogue StartOfYear rule.</summary>
    public static PropertyTransformer<T, TProperty> StartOfYear<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.StartOfYear);

    /// <summary>Appends the catalogue EndOfYear rule.</summary>
    public static PropertyTransformer<T, TProperty> EndOfYear<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.EndOfYear);

    /// <summary>Appends the catalogue ToUniversalTime rule.</summary>
    public static PropertyTransformer<T, TProperty> ToUniversalTime<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.ToUniversalTime);

    /// <summary>Appends the catalogue ToLocalTime rule.</summary>
    public static PropertyTransformer<T, TProperty> ToLocalTime<T, TProperty>(this PropertyTransformer<T, TProperty> rule)
        => rule.Add(DateCache<TProperty>.ToLocalTime);

    /// <summary>Appends the catalogue StartOfWeek rule.</summary>
    public static PropertyTransformer<T, TProperty> StartOfWeek<T, TProperty>(this PropertyTransformer<T, TProperty> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.StartOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue EndOfWeek rule.</summary>
    public static PropertyTransformer<T, TProperty> EndOfWeek<T, TProperty>(this PropertyTransformer<T, TProperty> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.EndOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue RoundTo rule.</summary>
    public static PropertyTransformer<T, TProperty> RoundTo<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<TProperty>(interval));

    /// <summary>Appends the catalogue TruncateTo rule.</summary>
    public static PropertyTransformer<T, TProperty> TruncateTo<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<TProperty>(interval));

    /// <summary>Appends the catalogue CeilingTo rule.</summary>
    public static PropertyTransformer<T, TProperty> CeilingTo<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<TProperty>(interval));

    /// <summary>Appends the catalogue WithYear rule.</summary>
    public static PropertyTransformer<T, TProperty> WithYear<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int year)
        => rule.Add(TransformerRules.WithYear<TProperty>(year));

    /// <summary>Appends the catalogue WithMonth rule.</summary>
    public static PropertyTransformer<T, TProperty> WithMonth<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int month)
        => rule.Add(TransformerRules.WithMonth<TProperty>(month));

    /// <summary>Appends the catalogue WithDay rule.</summary>
    public static PropertyTransformer<T, TProperty> WithDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int day)
        => rule.Add(TransformerRules.WithDay<TProperty>(day));

    /// <summary>Appends the catalogue WithHour rule.</summary>
    public static PropertyTransformer<T, TProperty> WithHour<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int hour)
        => rule.Add(TransformerRules.WithHour<TProperty>(hour));

    /// <summary>Appends the catalogue WithMinute rule.</summary>
    public static PropertyTransformer<T, TProperty> WithMinute<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int minute)
        => rule.Add(TransformerRules.WithMinute<TProperty>(minute));

    /// <summary>Appends the catalogue WithSecond rule.</summary>
    public static PropertyTransformer<T, TProperty> WithSecond<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int second)
        => rule.Add(TransformerRules.WithSecond<TProperty>(second));

    /// <summary>Appends the catalogue WithMillisecond rule.</summary>
    public static PropertyTransformer<T, TProperty> WithMillisecond<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int millisecond)
        => rule.Add(TransformerRules.WithMillisecond<TProperty>(millisecond));

    /// <summary>Appends the catalogue SetTimeOfDay rule.</summary>
    public static PropertyTransformer<T, TProperty> SetTimeOfDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule, TimeOnly timeOfDay)
        => rule.Add(TransformerRules.SetTimeOfDay<TProperty>(timeOfDay));

    /// <summary>Appends the catalogue SetDate rule.</summary>
    public static PropertyTransformer<T, TProperty> SetDate<T, TProperty>(this PropertyTransformer<T, TProperty> rule, DateOnly date)
        => rule.Add(TransformerRules.SetDate<TProperty>(date));

    /// <summary>Appends the catalogue NextDayOfWeek rule.</summary>
    public static PropertyTransformer<T, TProperty> NextDayOfWeek<T, TProperty>(this PropertyTransformer<T, TProperty> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.NextDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue PreviousDayOfWeek rule.</summary>
    public static PropertyTransformer<T, TProperty> PreviousDayOfWeek<T, TProperty>(this PropertyTransformer<T, TProperty> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.PreviousDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue AddBusinessDays rule.</summary>
    public static PropertyTransformer<T, TProperty> AddBusinessDays<T, TProperty>(this PropertyTransformer<T, TProperty> rule, int count, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.AddBusinessDays<TProperty>(count, holidays));

    /// <summary>Appends the catalogue NextBusinessDay rule.</summary>
    public static PropertyTransformer<T, TProperty> NextBusinessDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.NextBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue PreviousBusinessDay rule.</summary>
    public static PropertyTransformer<T, TProperty> PreviousBusinessDay<T, TProperty>(this PropertyTransformer<T, TProperty> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.PreviousBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue Negate rule.</summary>
    public static PropertyTransformer<T, TimeSpan> Negate<T>(this PropertyTransformer<T, TimeSpan> rule)
        => rule.Add(TimeSpanRules.Negate);

    /// <summary>Appends the catalogue Duration rule.</summary>
    public static PropertyTransformer<T, TimeSpan> Duration<T>(this PropertyTransformer<T, TimeSpan> rule)
        => rule.Add(TimeSpanRules.Duration);

    /// <summary>Appends the catalogue DateTime Clamp rule.</summary>
    public static PropertyTransformer<T, DateTime> Clamp<T>(this PropertyTransformer<T, DateTime> rule, DateTime min, DateTime max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateTimeOffset Clamp rule.</summary>
    public static PropertyTransformer<T, DateTimeOffset> Clamp<T>(this PropertyTransformer<T, DateTimeOffset> rule, DateTimeOffset min, DateTimeOffset max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeSpan Clamp rule.</summary>
    public static PropertyTransformer<T, TimeSpan> Clamp<T>(this PropertyTransformer<T, TimeSpan> rule, TimeSpan min, TimeSpan max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateOnly Clamp rule.</summary>
    public static PropertyTransformer<T, DateOnly> Clamp<T>(this PropertyTransformer<T, DateOnly> rule, DateOnly min, DateOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeOnly Clamp rule.</summary>
    public static PropertyTransformer<T, TimeOnly> Clamp<T>(this PropertyTransformer<T, TimeOnly> rule, TimeOnly min, TimeOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue SpecifyKind rule.</summary>
    public static PropertyTransformer<T, DateTime> SpecifyKind<T>(this PropertyTransformer<T, DateTime> rule, DateTimeKind kind)
        => rule.Add(TransformerRules.SpecifyKind(kind));

    /// <summary>Appends the catalogue ToOffset rule.</summary>
    public static PropertyTransformer<T, DateTimeOffset> ToOffset<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeSpan offset)
        => rule.Add(TransformerRules.ToOffset(offset));

    /// <summary>Appends the catalogue ConvertToTimeZone rule.</summary>
    public static PropertyTransformer<T, DateTimeOffset> ConvertToTimeZone<T>(this PropertyTransformer<T, DateTimeOffset> rule, TimeZoneInfo timeZone)
        => rule.Add(TransformerRules.ConvertToTimeZone(timeZone));

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

    private static class TimeSpanRules
    {
        public static readonly TransformerRule<TimeSpan> Negate = TransformerRules.Negate();
        public static readonly TransformerRule<TimeSpan> Duration = TransformerRules.Duration();
    }
}
