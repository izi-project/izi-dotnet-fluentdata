namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a catalogue rule and return the builder.
/// Same-type steps call <see cref="PropertyRule{T, TProperty}.Add(ITransformerRule{TProperty, TProperty})"/>
/// or <see cref="PropertyConversion{T, TIn, TOut}.Then{TNext}(ITransformerRule{TOut, TNext})"/>.
/// Steps that change type call <see cref="PropertyRule{T, TProperty}.Convert{TOut}(ITransformerRule{TProperty, TOut})"/>
/// or <c>Then</c>.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue AddYears rule.</summary>
    public static PropertyRule<T, TProperty> AddYears<T, TProperty>(this PropertyRule<T, TProperty> rule, int years)
        => rule.Add(TransformerRules.AddYears<TProperty>(years));

    /// <summary>Appends the catalogue AddYears rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddYears<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int years)
        => conversion.Then(TransformerRules.AddYears<TProperty>(years));

    /// <summary>Appends the catalogue AddMonths rule.</summary>
    public static PropertyRule<T, TProperty> AddMonths<T, TProperty>(this PropertyRule<T, TProperty> rule, int months)
        => rule.Add(TransformerRules.AddMonths<TProperty>(months));

    /// <summary>Appends the catalogue AddMonths rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddMonths<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int months)
        => conversion.Then(TransformerRules.AddMonths<TProperty>(months));

    /// <summary>Appends the catalogue AddDays rule.</summary>
    public static PropertyRule<T, TProperty> AddDays<T, TProperty>(this PropertyRule<T, TProperty> rule, double days)
        => rule.Add(TransformerRules.AddDays<TProperty>(days));

    /// <summary>Appends the catalogue AddDays rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddDays<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, double days)
        => conversion.Then(TransformerRules.AddDays<TProperty>(days));

    /// <summary>Appends the catalogue AddHours rule.</summary>
    public static PropertyRule<T, TProperty> AddHours<T, TProperty>(this PropertyRule<T, TProperty> rule, double hours)
        => rule.Add(TransformerRules.AddHours<TProperty>(hours));

    /// <summary>Appends the catalogue AddHours rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddHours<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, double hours)
        => conversion.Then(TransformerRules.AddHours<TProperty>(hours));

    /// <summary>Appends the catalogue AddMinutes rule.</summary>
    public static PropertyRule<T, TProperty> AddMinutes<T, TProperty>(this PropertyRule<T, TProperty> rule, double minutes)
        => rule.Add(TransformerRules.AddMinutes<TProperty>(minutes));

    /// <summary>Appends the catalogue AddMinutes rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddMinutes<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, double minutes)
        => conversion.Then(TransformerRules.AddMinutes<TProperty>(minutes));

    /// <summary>Appends the catalogue AddSeconds rule.</summary>
    public static PropertyRule<T, TProperty> AddSeconds<T, TProperty>(this PropertyRule<T, TProperty> rule, double seconds)
        => rule.Add(TransformerRules.AddSeconds<TProperty>(seconds));

    /// <summary>Appends the catalogue AddSeconds rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddSeconds<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, double seconds)
        => conversion.Then(TransformerRules.AddSeconds<TProperty>(seconds));

    /// <summary>Appends the catalogue AddMilliseconds rule.</summary>
    public static PropertyRule<T, TProperty> AddMilliseconds<T, TProperty>(this PropertyRule<T, TProperty> rule, double milliseconds)
        => rule.Add(TransformerRules.AddMilliseconds<TProperty>(milliseconds));

    /// <summary>Appends the catalogue AddMilliseconds rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddMilliseconds<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, double milliseconds)
        => conversion.Then(TransformerRules.AddMilliseconds<TProperty>(milliseconds));

    /// <summary>Appends the catalogue AddTicks rule.</summary>
    public static PropertyRule<T, TProperty> AddTicks<T, TProperty>(this PropertyRule<T, TProperty> rule, long ticks)
        => rule.Add(TransformerRules.AddTicks<TProperty>(ticks));

    /// <summary>Appends the catalogue AddTicks rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddTicks<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, long ticks)
        => conversion.Then(TransformerRules.AddTicks<TProperty>(ticks));

    /// <summary>Appends the catalogue date or time Add rule.</summary>
    public static PropertyRule<T, TProperty> Add<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeSpan value)
        => rule.Add(TransformerRules.Add<TProperty>(value));

    /// <summary>Appends the catalogue date or time Add rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> Add<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeSpan value)
        => conversion.Then(TransformerRules.Add<TProperty>(value));

    /// <summary>Appends the catalogue date or time Subtract rule.</summary>
    public static PropertyRule<T, TProperty> Subtract<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeSpan value)
        => rule.Add(TransformerRules.Subtract<TProperty>(value));

    /// <summary>Appends the catalogue date or time Subtract rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> Subtract<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeSpan value)
        => conversion.Then(TransformerRules.Subtract<TProperty>(value));

    /// <summary>Appends the catalogue StartOfDay rule.</summary>
    public static PropertyRule<T, TProperty> StartOfDay<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.StartOfDay<TProperty>());

    /// <summary>Appends the catalogue StartOfDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> StartOfDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.StartOfDay<TProperty>());

    /// <summary>Appends the catalogue EndOfDay rule.</summary>
    public static PropertyRule<T, TProperty> EndOfDay<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.EndOfDay<TProperty>());

    /// <summary>Appends the catalogue EndOfDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> EndOfDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.EndOfDay<TProperty>());

    /// <summary>Appends the catalogue StartOfMonth rule.</summary>
    public static PropertyRule<T, TProperty> StartOfMonth<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.StartOfMonth<TProperty>());

    /// <summary>Appends the catalogue StartOfMonth rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> StartOfMonth<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.StartOfMonth<TProperty>());

    /// <summary>Appends the catalogue EndOfMonth rule.</summary>
    public static PropertyRule<T, TProperty> EndOfMonth<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.EndOfMonth<TProperty>());

    /// <summary>Appends the catalogue EndOfMonth rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> EndOfMonth<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.EndOfMonth<TProperty>());

    /// <summary>Appends the catalogue StartOfQuarter rule.</summary>
    public static PropertyRule<T, TProperty> StartOfQuarter<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.StartOfQuarter<TProperty>());

    /// <summary>Appends the catalogue StartOfQuarter rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> StartOfQuarter<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.StartOfQuarter<TProperty>());

    /// <summary>Appends the catalogue EndOfQuarter rule.</summary>
    public static PropertyRule<T, TProperty> EndOfQuarter<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.EndOfQuarter<TProperty>());

    /// <summary>Appends the catalogue EndOfQuarter rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> EndOfQuarter<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.EndOfQuarter<TProperty>());

    /// <summary>Appends the catalogue StartOfYear rule.</summary>
    public static PropertyRule<T, TProperty> StartOfYear<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.StartOfYear<TProperty>());

    /// <summary>Appends the catalogue StartOfYear rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> StartOfYear<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.StartOfYear<TProperty>());

    /// <summary>Appends the catalogue EndOfYear rule.</summary>
    public static PropertyRule<T, TProperty> EndOfYear<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.EndOfYear<TProperty>());

    /// <summary>Appends the catalogue EndOfYear rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> EndOfYear<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.EndOfYear<TProperty>());

    /// <summary>Appends the catalogue ToUniversalTime rule.</summary>
    public static PropertyRule<T, TProperty> ToUniversalTime<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.ToUniversalTime<TProperty>());

    /// <summary>Appends the catalogue ToUniversalTime rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> ToUniversalTime<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.ToUniversalTime<TProperty>());

    /// <summary>Appends the catalogue ToLocalTime rule.</summary>
    public static PropertyRule<T, TProperty> ToLocalTime<T, TProperty>(this PropertyRule<T, TProperty> rule)
        => rule.Add(TransformerRules.ToLocalTime<TProperty>());

    /// <summary>Appends the catalogue ToLocalTime rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> ToLocalTime<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion)
        => conversion.Then(TransformerRules.ToLocalTime<TProperty>());

    /// <summary>Appends the catalogue StartOfWeek rule.</summary>
    public static PropertyRule<T, TProperty> StartOfWeek<T, TProperty>(this PropertyRule<T, TProperty> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.StartOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue StartOfWeek rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> StartOfWeek<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, DayOfWeek firstDayOfWeek)
        => conversion.Then(TransformerRules.StartOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue EndOfWeek rule.</summary>
    public static PropertyRule<T, TProperty> EndOfWeek<T, TProperty>(this PropertyRule<T, TProperty> rule, DayOfWeek firstDayOfWeek)
        => rule.Add(TransformerRules.EndOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue EndOfWeek rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> EndOfWeek<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, DayOfWeek firstDayOfWeek)
        => conversion.Then(TransformerRules.EndOfWeek<TProperty>(firstDayOfWeek));

    /// <summary>Appends the catalogue RoundTo rule.</summary>
    public static PropertyRule<T, TProperty> RoundTo<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.RoundTo<TProperty>(interval));

    /// <summary>Appends the catalogue RoundTo rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> RoundTo<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeSpan interval)
        => conversion.Then(TransformerRules.RoundTo<TProperty>(interval));

    /// <summary>Appends the catalogue TruncateTo rule.</summary>
    public static PropertyRule<T, TProperty> TruncateTo<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.TruncateTo<TProperty>(interval));

    /// <summary>Appends the catalogue TruncateTo rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> TruncateTo<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeSpan interval)
        => conversion.Then(TransformerRules.TruncateTo<TProperty>(interval));

    /// <summary>Appends the catalogue CeilingTo rule.</summary>
    public static PropertyRule<T, TProperty> CeilingTo<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeSpan interval)
        => rule.Add(TransformerRules.CeilingTo<TProperty>(interval));

    /// <summary>Appends the catalogue CeilingTo rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> CeilingTo<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeSpan interval)
        => conversion.Then(TransformerRules.CeilingTo<TProperty>(interval));

    /// <summary>Appends the catalogue WithYear rule.</summary>
    public static PropertyRule<T, TProperty> WithYear<T, TProperty>(this PropertyRule<T, TProperty> rule, int year)
        => rule.Add(TransformerRules.WithYear<TProperty>(year));

    /// <summary>Appends the catalogue WithYear rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithYear<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int year)
        => conversion.Then(TransformerRules.WithYear<TProperty>(year));

    /// <summary>Appends the catalogue WithMonth rule.</summary>
    public static PropertyRule<T, TProperty> WithMonth<T, TProperty>(this PropertyRule<T, TProperty> rule, int month)
        => rule.Add(TransformerRules.WithMonth<TProperty>(month));

    /// <summary>Appends the catalogue WithMonth rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithMonth<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int month)
        => conversion.Then(TransformerRules.WithMonth<TProperty>(month));

    /// <summary>Appends the catalogue WithDay rule.</summary>
    public static PropertyRule<T, TProperty> WithDay<T, TProperty>(this PropertyRule<T, TProperty> rule, int day)
        => rule.Add(TransformerRules.WithDay<TProperty>(day));

    /// <summary>Appends the catalogue WithDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int day)
        => conversion.Then(TransformerRules.WithDay<TProperty>(day));

    /// <summary>Appends the catalogue WithHour rule.</summary>
    public static PropertyRule<T, TProperty> WithHour<T, TProperty>(this PropertyRule<T, TProperty> rule, int hour)
        => rule.Add(TransformerRules.WithHour<TProperty>(hour));

    /// <summary>Appends the catalogue WithHour rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithHour<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int hour)
        => conversion.Then(TransformerRules.WithHour<TProperty>(hour));

    /// <summary>Appends the catalogue WithMinute rule.</summary>
    public static PropertyRule<T, TProperty> WithMinute<T, TProperty>(this PropertyRule<T, TProperty> rule, int minute)
        => rule.Add(TransformerRules.WithMinute<TProperty>(minute));

    /// <summary>Appends the catalogue WithMinute rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithMinute<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int minute)
        => conversion.Then(TransformerRules.WithMinute<TProperty>(minute));

    /// <summary>Appends the catalogue WithSecond rule.</summary>
    public static PropertyRule<T, TProperty> WithSecond<T, TProperty>(this PropertyRule<T, TProperty> rule, int second)
        => rule.Add(TransformerRules.WithSecond<TProperty>(second));

    /// <summary>Appends the catalogue WithSecond rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithSecond<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int second)
        => conversion.Then(TransformerRules.WithSecond<TProperty>(second));

    /// <summary>Appends the catalogue WithMillisecond rule.</summary>
    public static PropertyRule<T, TProperty> WithMillisecond<T, TProperty>(this PropertyRule<T, TProperty> rule, int millisecond)
        => rule.Add(TransformerRules.WithMillisecond<TProperty>(millisecond));

    /// <summary>Appends the catalogue WithMillisecond rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> WithMillisecond<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int millisecond)
        => conversion.Then(TransformerRules.WithMillisecond<TProperty>(millisecond));

    /// <summary>Appends the catalogue SetTimeOfDay rule.</summary>
    public static PropertyRule<T, TProperty> SetTimeOfDay<T, TProperty>(this PropertyRule<T, TProperty> rule, TimeOnly timeOfDay)
        => rule.Add(TransformerRules.SetTimeOfDay<TProperty>(timeOfDay));

    /// <summary>Appends the catalogue SetTimeOfDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> SetTimeOfDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TimeOnly timeOfDay)
        => conversion.Then(TransformerRules.SetTimeOfDay<TProperty>(timeOfDay));

    /// <summary>Appends the catalogue SetDate rule.</summary>
    public static PropertyRule<T, TProperty> SetDate<T, TProperty>(this PropertyRule<T, TProperty> rule, DateOnly date)
        => rule.Add(TransformerRules.SetDate<TProperty>(date));

    /// <summary>Appends the catalogue SetDate rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> SetDate<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, DateOnly date)
        => conversion.Then(TransformerRules.SetDate<TProperty>(date));

    /// <summary>Appends the catalogue NextDayOfWeek rule.</summary>
    public static PropertyRule<T, TProperty> NextDayOfWeek<T, TProperty>(this PropertyRule<T, TProperty> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.NextDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue NextDayOfWeek rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> NextDayOfWeek<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, DayOfWeek dayOfWeek)
        => conversion.Then(TransformerRules.NextDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue PreviousDayOfWeek rule.</summary>
    public static PropertyRule<T, TProperty> PreviousDayOfWeek<T, TProperty>(this PropertyRule<T, TProperty> rule, DayOfWeek dayOfWeek)
        => rule.Add(TransformerRules.PreviousDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue PreviousDayOfWeek rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> PreviousDayOfWeek<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, DayOfWeek dayOfWeek)
        => conversion.Then(TransformerRules.PreviousDayOfWeek<TProperty>(dayOfWeek));

    /// <summary>Appends the catalogue AddBusinessDays rule.</summary>
    public static PropertyRule<T, TProperty> AddBusinessDays<T, TProperty>(this PropertyRule<T, TProperty> rule, int count, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.AddBusinessDays<TProperty>(count, holidays));

    /// <summary>Appends the catalogue AddBusinessDays rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> AddBusinessDays<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, int count, IEnumerable<DateOnly>? holidays = null)
        => conversion.Then(TransformerRules.AddBusinessDays<TProperty>(count, holidays));

    /// <summary>Appends the catalogue NextBusinessDay rule.</summary>
    public static PropertyRule<T, TProperty> NextBusinessDay<T, TProperty>(this PropertyRule<T, TProperty> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.NextBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue NextBusinessDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> NextBusinessDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, IEnumerable<DateOnly>? holidays = null)
        => conversion.Then(TransformerRules.NextBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue PreviousBusinessDay rule.</summary>
    public static PropertyRule<T, TProperty> PreviousBusinessDay<T, TProperty>(this PropertyRule<T, TProperty> rule, IEnumerable<DateOnly>? holidays = null)
        => rule.Add(TransformerRules.PreviousBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue PreviousBusinessDay rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> PreviousBusinessDay<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, IEnumerable<DateOnly>? holidays = null)
        => conversion.Then(TransformerRules.PreviousBusinessDay<TProperty>(holidays));

    /// <summary>Appends the catalogue Negate rule.</summary>
    public static PropertyRule<T, TimeSpan> Negate<T>(this PropertyRule<T, TimeSpan> rule)
        => rule.Add(TransformerRules.Negate());

    /// <summary>Appends the catalogue Negate rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> Negate<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion)
        => conversion.Then(TransformerRules.Negate());

    /// <summary>Appends the catalogue Duration rule.</summary>
    public static PropertyRule<T, TimeSpan> Duration<T>(this PropertyRule<T, TimeSpan> rule)
        => rule.Add(TransformerRules.Duration());

    /// <summary>Appends the catalogue Duration rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> Duration<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion)
        => conversion.Then(TransformerRules.Duration());

    /// <summary>Appends the catalogue DateTime Clamp rule.</summary>
    public static PropertyRule<T, DateTime> Clamp<T>(this PropertyRule<T, DateTime> rule, DateTime min, DateTime max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateTime Clamp rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> Clamp<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion, DateTime min, DateTime max)
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateTimeOffset Clamp rule.</summary>
    public static PropertyRule<T, DateTimeOffset> Clamp<T>(this PropertyRule<T, DateTimeOffset> rule, DateTimeOffset min, DateTimeOffset max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateTimeOffset Clamp rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> Clamp<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion, DateTimeOffset min, DateTimeOffset max)
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeSpan Clamp rule.</summary>
    public static PropertyRule<T, TimeSpan> Clamp<T>(this PropertyRule<T, TimeSpan> rule, TimeSpan min, TimeSpan max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeSpan Clamp rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> Clamp<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion, TimeSpan min, TimeSpan max)
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateOnly Clamp rule.</summary>
    public static PropertyRule<T, DateOnly> Clamp<T>(this PropertyRule<T, DateOnly> rule, DateOnly min, DateOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue DateOnly Clamp rule.</summary>
    public static PropertyConversion<T, TIn, DateOnly> Clamp<T, TIn>(this PropertyConversion<T, TIn, DateOnly> conversion, DateOnly min, DateOnly max)
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeOnly Clamp rule.</summary>
    public static PropertyRule<T, TimeOnly> Clamp<T>(this PropertyRule<T, TimeOnly> rule, TimeOnly min, TimeOnly max)
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue TimeOnly Clamp rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> Clamp<T, TIn>(this PropertyConversion<T, TIn, TimeOnly> conversion, TimeOnly min, TimeOnly max)
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue SpecifyKind rule.</summary>
    public static PropertyRule<T, DateTime> SpecifyKind<T>(this PropertyRule<T, DateTime> rule, DateTimeKind kind)
        => rule.Add(TransformerRules.SpecifyKind(kind));

    /// <summary>Appends the catalogue SpecifyKind rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> SpecifyKind<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion, DateTimeKind kind)
        => conversion.Then(TransformerRules.SpecifyKind(kind));

    /// <summary>Appends the catalogue ToOffset rule.</summary>
    public static PropertyRule<T, DateTimeOffset> ToOffset<T>(this PropertyRule<T, DateTimeOffset> rule, TimeSpan offset)
        => rule.Add(TransformerRules.ToOffset(offset));

    /// <summary>Appends the catalogue ToOffset rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ToOffset<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion, TimeSpan offset)
        => conversion.Then(TransformerRules.ToOffset(offset));

    /// <summary>Appends the catalogue ConvertToTimeZone rule.</summary>
    public static PropertyRule<T, DateTimeOffset> ConvertToTimeZone<T>(this PropertyRule<T, DateTimeOffset> rule, TimeZoneInfo timeZone)
        => rule.Add(TransformerRules.ConvertToTimeZone(timeZone));

    /// <summary>Appends the catalogue ConvertToTimeZone rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ConvertToTimeZone<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion, TimeZoneInfo timeZone)
        => conversion.Then(TransformerRules.ConvertToTimeZone(timeZone));
}
