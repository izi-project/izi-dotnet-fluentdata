namespace Izi.FluentData.Transformer;

public static partial class TransformerRules
{
    /// <summary>Adds years. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>. An unsupported type throws when the rule is created.</summary>
    public static TransformerRule<T, T> AddYears<T>(int years)
        => Pick<T>(nameof(AddYears),
            dateTime: value => value.AddYears(years),
            dateTimeOffset: value => value.AddYears(years),
            dateOnly: value => value.AddYears(years));

    /// <summary>Adds months. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> AddMonths<T>(int months)
        => Pick<T>(nameof(AddMonths),
            dateTime: value => value.AddMonths(months),
            dateTimeOffset: value => value.AddMonths(months),
            dateOnly: value => value.AddMonths(months));

    /// <summary>
    /// Adds days. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="DateOnly"/>, and <see cref="TimeSpan"/>.
    /// <see cref="DateOnly"/> truncates the fractional days toward zero.
    /// </summary>
    public static TransformerRule<T, T> AddDays<T>(double days)
        => Pick<T>(nameof(AddDays),
            dateTime: value => value.AddDays(days),
            dateTimeOffset: value => value.AddDays(days),
            dateOnly: value => value.AddDays((int)days),
            timeSpan: value => value + TimeSpan.FromDays(days));

    /// <summary>Adds hours. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>. <see cref="TimeOnly"/> wraps past midnight.</summary>
    public static TransformerRule<T, T> AddHours<T>(double hours)
        => Pick<T>(nameof(AddHours),
            dateTime: value => value.AddHours(hours),
            dateTimeOffset: value => value.AddHours(hours),
            timeOnly: value => value.AddHours(hours),
            timeSpan: value => value + TimeSpan.FromHours(hours));

    /// <summary>Adds minutes. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<T, T> AddMinutes<T>(double minutes)
        => Pick<T>(nameof(AddMinutes),
            dateTime: value => value.AddMinutes(minutes),
            dateTimeOffset: value => value.AddMinutes(minutes),
            timeOnly: value => value.Add(TimeSpan.FromMinutes(minutes)),
            timeSpan: value => value + TimeSpan.FromMinutes(minutes));

    /// <summary>Adds seconds. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<T, T> AddSeconds<T>(double seconds)
        => Pick<T>(nameof(AddSeconds),
            dateTime: value => value.AddSeconds(seconds),
            dateTimeOffset: value => value.AddSeconds(seconds),
            timeOnly: value => value.Add(TimeSpan.FromSeconds(seconds)),
            timeSpan: value => value + TimeSpan.FromSeconds(seconds));

    /// <summary>Adds milliseconds. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<T, T> AddMilliseconds<T>(double milliseconds)
        => Pick<T>(nameof(AddMilliseconds),
            dateTime: value => value.AddMilliseconds(milliseconds),
            dateTimeOffset: value => value.AddMilliseconds(milliseconds),
            timeOnly: value => value.Add(TimeSpan.FromMilliseconds(milliseconds)),
            timeSpan: value => value + TimeSpan.FromMilliseconds(milliseconds));

    /// <summary>Adds ticks. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<T, T> AddTicks<T>(long ticks)
        => Pick<T>(nameof(AddTicks),
            dateTime: value => value.AddTicks(ticks),
            dateTimeOffset: value => value.AddTicks(ticks),
            timeOnly: value => value.Add(TimeSpan.FromTicks(ticks)),
            timeSpan: value => value + TimeSpan.FromTicks(ticks));

    /// <summary>
    /// Adds a <see cref="TimeSpan"/>. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.
    /// The type argument is required, for example <c>Add&lt;DateTime&gt;(time)</c>, because a numeric <c>Add</c> also exists.
    /// </summary>
    public static TransformerRule<T, T> Add<T>(TimeSpan value)
        => Pick<T>(nameof(Add),
            dateTime: source => source.Add(value),
            dateTimeOffset: source => source.Add(value),
            timeOnly: source => source.Add(value),
            timeSpan: source => source.Add(value));

    /// <summary>Subtracts a <see cref="TimeSpan"/>. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<T, T> Subtract<T>(TimeSpan value)
        => Pick<T>(nameof(Subtract),
            dateTime: source => source.Subtract(value),
            dateTimeOffset: source => source.Subtract(value),
            timeOnly: source => source.Add(-value),
            timeSpan: source => source.Subtract(value));

    /// <summary>Flips the sign of a <see cref="TimeSpan"/>. <see cref="TimeSpan.MinValue"/> throws <see cref="OverflowException"/>.</summary>
    public static TransformerRule<TimeSpan, TimeSpan> Negate()
        => Same<TimeSpan>(value => value.Negate());

    /// <summary>Returns the absolute duration of a <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<TimeSpan, TimeSpan> Duration()
        => Same<TimeSpan>(value => value.Duration());

    /// <summary>Clamps a <see cref="DateTime"/> to the inclusive range. Comparison uses ticks.</summary>
    public static TransformerRule<DateTime, DateTime> Clamp(DateTime min, DateTime max)
        => ClampCore(min, max);

    /// <summary>Clamps a <see cref="DateTimeOffset"/> to the inclusive range.</summary>
    public static TransformerRule<DateTimeOffset, DateTimeOffset> Clamp(DateTimeOffset min, DateTimeOffset max)
        => ClampCore(min, max);

    /// <summary>Clamps a <see cref="TimeSpan"/> to the inclusive range.</summary>
    public static TransformerRule<TimeSpan, TimeSpan> Clamp(TimeSpan min, TimeSpan max)
        => ClampCore(min, max);

    /// <summary>Clamps a <see cref="DateOnly"/> to the inclusive range.</summary>
    public static TransformerRule<DateOnly, DateOnly> Clamp(DateOnly min, DateOnly max)
        => ClampCore(min, max);

    /// <summary>Clamps a <see cref="TimeOnly"/> to the inclusive range.</summary>
    public static TransformerRule<TimeOnly, TimeOnly> Clamp(TimeOnly min, TimeOnly max)
        => ClampCore(min, max);

    /// <summary>Moves to midnight, keeping <see cref="DateTime.Kind"/> or the offset. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.</summary>
    public static TransformerRule<T, T> StartOfDay<T>()
        => Pick<T>(nameof(StartOfDay),
            dateTime: value => value.Date,
            dateTimeOffset: value => new DateTimeOffset(value.Date, value.Offset));

    /// <summary>Moves to the last tick of the day. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.</summary>
    public static TransformerRule<T, T> EndOfDay<T>()
        => Pick<T>(nameof(EndOfDay),
            dateTime: value => value.Date.AddDays(1).AddTicks(-1),
            dateTimeOffset: value => new DateTimeOffset(value.Date, value.Offset).AddDays(1).AddTicks(-1));

    /// <summary>Moves back to <paramref name="firstDayOfWeek"/>, or stays when already on that day. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> StartOfWeek<T>(DayOfWeek firstDayOfWeek)
        => Pick<T>(nameof(StartOfWeek),
            dateTime: value => value.Date.AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true)),
            dateTimeOffset: value => new DateTimeOffset(value.Date, value.Offset).AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true)),
            dateOnly: value => value.AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true)));

    /// <summary>
    /// Moves to the end of the week that starts on <paramref name="firstDayOfWeek"/>.
    /// <see cref="DateTime"/> and <see cref="DateTimeOffset"/> land on the last tick. <see cref="DateOnly"/> lands on the last day.
    /// </summary>
    public static TransformerRule<T, T> EndOfWeek<T>(DayOfWeek firstDayOfWeek)
        => Pick<T>(nameof(EndOfWeek),
            dateTime: value => value.Date.AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true) + 7).AddTicks(-1),
            dateTimeOffset: value => new DateTimeOffset(value.Date, value.Offset).AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true) + 7).AddTicks(-1),
            dateOnly: value => value.AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true) + 6));

    /// <summary>Moves to the first day of the month, keeping kind or offset. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> StartOfMonth<T>()
        => Pick<T>(nameof(StartOfMonth),
            dateTime: value => new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind),
            dateTimeOffset: value => new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset),
            dateOnly: value => new DateOnly(value.Year, value.Month, 1));

    /// <summary>Moves to the end of the month. Time-bearing types use the last tick. <see cref="DateOnly"/> uses the last calendar day.</summary>
    public static TransformerRule<T, T> EndOfMonth<T>()
        => Pick<T>(nameof(EndOfMonth),
            dateTime: value => new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind).AddMonths(1).AddTicks(-1),
            dateTimeOffset: value => new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset).AddMonths(1).AddTicks(-1),
            dateOnly: value => EndOfMonth(value));

    /// <summary>Moves to the first day of the calendar quarter. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> StartOfQuarter<T>()
        => Pick<T>(nameof(StartOfQuarter),
            dateTime: value => new DateTime(value.Year, QuarterStartMonth(value.Month), 1, 0, 0, 0, value.Kind),
            dateTimeOffset: value => new DateTimeOffset(value.Year, QuarterStartMonth(value.Month), 1, 0, 0, 0, value.Offset),
            dateOnly: value => new DateOnly(value.Year, QuarterStartMonth(value.Month), 1));

    /// <summary>Moves to the end of the calendar quarter. Time-bearing types use the last tick. <see cref="DateOnly"/> uses the last day.</summary>
    public static TransformerRule<T, T> EndOfQuarter<T>()
        => Pick<T>(nameof(EndOfQuarter),
            dateTime: value => new DateTime(value.Year, QuarterStartMonth(value.Month), 1, 0, 0, 0, value.Kind).AddMonths(3).AddTicks(-1),
            dateTimeOffset: value => new DateTimeOffset(value.Year, QuarterStartMonth(value.Month), 1, 0, 0, 0, value.Offset).AddMonths(3).AddTicks(-1),
            dateOnly: value => EndOfMonth(new DateOnly(value.Year, QuarterStartMonth(value.Month), 1).AddMonths(2)));

    /// <summary>Moves to the start of the year. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> StartOfYear<T>()
        => Pick<T>(nameof(StartOfYear),
            dateTime: value => new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind),
            dateTimeOffset: value => new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset),
            dateOnly: value => new DateOnly(value.Year, 1, 1));

    /// <summary>Moves to the end of the year. Time-bearing types use the last tick. <see cref="DateOnly"/> is 31 December.</summary>
    public static TransformerRule<T, T> EndOfYear<T>()
        => Pick<T>(nameof(EndOfYear),
            dateTime: value => new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind).AddYears(1).AddTicks(-1),
            dateTimeOffset: value => new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset).AddYears(1).AddTicks(-1),
            dateOnly: value => new DateOnly(value.Year, 12, 31));

    /// <summary>
    /// Rounds to the nearest multiple of <paramref name="interval"/>. An exact half rounds toward positive infinity.
    /// Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeOnly"/>, and <see cref="TimeSpan"/>.
    /// <see cref="TimeOnly"/> wraps into the same day. A non-positive interval is rejected when the rule is created.
    /// Negative spans are rounded with the same half-up rule, which the previous tick formula got wrong.
    /// </summary>
    public static TransformerRule<T, T> RoundTo<T>(TimeSpan interval)
    {
        var unit = RequirePositiveInterval(interval);
        return Quantize<T>(nameof(RoundTo), unit, RoundHalfUp);
    }

    /// <summary>Truncates toward zero to a multiple of <paramref name="interval"/>. Supports the same types as <see cref="RoundTo{T}"/>.</summary>
    public static TransformerRule<T, T> TruncateTo<T>(TimeSpan interval)
    {
        var unit = RequirePositiveInterval(interval);
        return Quantize<T>(nameof(TruncateTo), unit, TruncateTowardZero);
    }

    /// <summary>Rounds toward positive infinity to a multiple of <paramref name="interval"/>. Supports the same types as <see cref="RoundTo{T}"/>.</summary>
    public static TransformerRule<T, T> CeilingTo<T>(TimeSpan interval)
    {
        var unit = RequirePositiveInterval(interval);
        return Quantize<T>(nameof(CeilingTo), unit, CeilingTowardPositiveInfinity);
    }

    /// <summary>Replaces the year, keeping the month, day, time, kind, or offset. Invalid dates throw when transformed. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> WithYear<T>(int year)
        => Pick<T>(nameof(WithYear),
            dateTime: value => WithDate(value, year, value.Month, value.Day),
            dateTimeOffset: value => WithDate(value, year, value.Month, value.Day),
            dateOnly: value => new DateOnly(year, value.Month, value.Day));

    /// <summary>Replaces the month. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> WithMonth<T>(int month)
        => Pick<T>(nameof(WithMonth),
            dateTime: value => WithDate(value, value.Year, month, value.Day),
            dateTimeOffset: value => WithDate(value, value.Year, month, value.Day),
            dateOnly: value => new DateOnly(value.Year, month, value.Day));

    /// <summary>Replaces the day of the month. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> WithDay<T>(int day)
        => Pick<T>(nameof(WithDay),
            dateTime: value => WithDate(value, value.Year, value.Month, day),
            dateTimeOffset: value => WithDate(value, value.Year, value.Month, day),
            dateOnly: value => new DateOnly(value.Year, value.Month, day));

    /// <summary>Replaces the hour, keeping sub-millisecond ticks. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="TimeOnly"/>.</summary>
    public static TransformerRule<T, T> WithHour<T>(int hour)
    {
        if (hour is < 0 or > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour));
        }

        return Pick<T>(nameof(WithHour),
            dateTime: value => WithTime(value, hour, value.Minute, value.Second, value.Millisecond),
            dateTimeOffset: value => WithTime(value, hour, value.Minute, value.Second, value.Millisecond),
            timeOnly: value => WithTime(value, hour, value.Minute, value.Second, value.Millisecond));
    }

    /// <summary>Replaces the minute, keeping sub-millisecond ticks. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="TimeOnly"/>.</summary>
    public static TransformerRule<T, T> WithMinute<T>(int minute)
    {
        if (minute is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minute));
        }

        return Pick<T>(nameof(WithMinute),
            dateTime: value => WithTime(value, value.Hour, minute, value.Second, value.Millisecond),
            dateTimeOffset: value => WithTime(value, value.Hour, minute, value.Second, value.Millisecond),
            timeOnly: value => WithTime(value, value.Hour, minute, value.Second, value.Millisecond));
    }

    /// <summary>Replaces the second, keeping sub-millisecond ticks. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="TimeOnly"/>.</summary>
    public static TransformerRule<T, T> WithSecond<T>(int second)
    {
        if (second is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(second));
        }

        return Pick<T>(nameof(WithSecond),
            dateTime: value => WithTime(value, value.Hour, value.Minute, second, value.Millisecond),
            dateTimeOffset: value => WithTime(value, value.Hour, value.Minute, second, value.Millisecond),
            timeOnly: value => WithTime(value, value.Hour, value.Minute, second, value.Millisecond));
    }

    /// <summary>Replaces the millisecond, keeping sub-millisecond ticks. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="TimeOnly"/>.</summary>
    public static TransformerRule<T, T> WithMillisecond<T>(int millisecond)
    {
        if (millisecond is < 0 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(millisecond));
        }

        return Pick<T>(nameof(WithMillisecond),
            dateTime: value => WithTime(value, value.Hour, value.Minute, value.Second, millisecond),
            dateTimeOffset: value => WithTime(value, value.Hour, value.Minute, value.Second, millisecond),
            timeOnly: value => WithTime(value, value.Hour, value.Minute, value.Second, millisecond));
    }

    /// <summary>Replaces the time of day and keeps the date, kind, or offset. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.</summary>
    public static TransformerRule<T, T> SetTimeOfDay<T>(TimeOnly timeOfDay)
        => Pick<T>(nameof(SetTimeOfDay),
            dateTime: value => value.Date.Add(timeOfDay.ToTimeSpan()),
            dateTimeOffset: value => new DateTimeOffset(value.Date, value.Offset).Add(timeOfDay.ToTimeSpan()));

    /// <summary>Replaces the date and keeps the time of day, kind, or offset. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.</summary>
    public static TransformerRule<T, T> SetDate<T>(DateOnly date)
        => Pick<T>(nameof(SetDate),
            dateTime: value => date.ToDateTime(TimeOnly.FromTimeSpan(value.TimeOfDay), value.Kind),
            dateTimeOffset: value => new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, value.Offset).Add(value.TimeOfDay));

    /// <summary>Moves strictly forward to the next <paramref name="dayOfWeek"/>, never staying on the current day. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> NextDayOfWeek<T>(DayOfWeek dayOfWeek)
        => Pick<T>(nameof(NextDayOfWeek),
            dateTime: value => value.AddDays(DaysUntil(value.DayOfWeek, dayOfWeek)),
            dateTimeOffset: value => value.AddDays(DaysUntil(value.DayOfWeek, dayOfWeek)),
            dateOnly: value => value.AddDays(DaysUntil(value.DayOfWeek, dayOfWeek)));

    /// <summary>Moves strictly backward to the previous <paramref name="dayOfWeek"/>. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</summary>
    public static TransformerRule<T, T> PreviousDayOfWeek<T>(DayOfWeek dayOfWeek)
        => Pick<T>(nameof(PreviousDayOfWeek),
            dateTime: value => value.AddDays(-DaysSince(value.DayOfWeek, dayOfWeek, includeSameDay: false)),
            dateTimeOffset: value => value.AddDays(-DaysSince(value.DayOfWeek, dayOfWeek, includeSameDay: false)),
            dateOnly: value => value.AddDays(-DaysSince(value.DayOfWeek, dayOfWeek, includeSameDay: false)));

    /// <summary>
    /// Adds <paramref name="count"/> business days, skipping weekends and the supplied holidays.
    /// A positive count always moves forward at least one day, so a business day does not stay put.
    /// The holiday sequence is copied when the rule is created. Supports <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.
    /// </summary>
    public static TransformerRule<T, T> AddBusinessDays<T>(int count, IEnumerable<DateOnly>? holidays = null)
    {
        var holidaySet = CopyHolidays(holidays);
        return Pick<T>(nameof(AddBusinessDays),
            dateTime: value => MoveBusinessDays(value, count, holidaySet),
            dateTimeOffset: value => MoveBusinessDays(value, count, holidaySet),
            dateOnly: value => MoveBusinessDays(value, count, holidaySet));
    }

    /// <summary>Moves to the next business day. This is <see cref="AddBusinessDays{T}"/> of one.</summary>
    public static TransformerRule<T, T> NextBusinessDay<T>(IEnumerable<DateOnly>? holidays = null)
        => AddBusinessDays<T>(1, holidays);

    /// <summary>Moves to the previous business day. This is <see cref="AddBusinessDays{T}"/> of minus one.</summary>
    public static TransformerRule<T, T> PreviousBusinessDay<T>(IEnumerable<DateOnly>? holidays = null)
        => AddBusinessDays<T>(-1, holidays);

    /// <summary>
    /// Converts to UTC. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Unspecified"/> is treated as local, which is the BCL rule.
    /// </summary>
    public static TransformerRule<T, T> ToUniversalTime<T>()
        => Pick<T>(nameof(ToUniversalTime),
            dateTime: value => value.ToUniversalTime(),
            dateTimeOffset: value => value.ToUniversalTime());

    /// <summary>Converts to local time. Supports <see cref="DateTime"/> and <see cref="DateTimeOffset"/>. The local zone is the machine zone.</summary>
    public static TransformerRule<T, T> ToLocalTime<T>()
        => Pick<T>(nameof(ToLocalTime),
            dateTime: value => value.ToLocalTime(),
            dateTimeOffset: value => value.ToLocalTime());

    /// <summary>Stamps <paramref name="kind"/> without shifting the clock value.</summary>
    public static TransformerRule<DateTime, DateTime> SpecifyKind(DateTimeKind kind)
        => Same<DateTime>(value => DateTime.SpecifyKind(value, kind));

    /// <summary>
    /// Changes the offset while preserving the instant. An offset that is not a whole number of minutes,
    /// or is outside plus or minus 14 hours, is rejected when the rule is created.
    /// </summary>
    public static TransformerRule<DateTimeOffset, DateTimeOffset> ToOffset(TimeSpan offset)
    {
        _ = new DateTimeOffset(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), offset);
        return Same<DateTimeOffset>(value => value.ToOffset(offset));
    }

    /// <summary>Converts to <paramref name="timeZone"/>, preserving the instant.</summary>
    public static TransformerRule<DateTimeOffset, DateTimeOffset> ConvertToTimeZone(TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        return Same<DateTimeOffset>(value => TimeZoneInfo.ConvertTime(value, timeZone));
    }

    private static TransformerRule<T, T> Quantize<T>(string operation, long unit, Func<long, long, long> quantize)
        => Pick<T>(operation,
            dateTime: value => new DateTime(quantize(value.Ticks, unit), value.Kind),
            dateTimeOffset: value => new DateTimeOffset(new DateTime(quantize(value.DateTime.Ticks, unit), DateTimeKind.Unspecified), value.Offset),
            timeOnly: value => new TimeOnly(WrapDay(quantize(value.Ticks, unit))),
            timeSpan: value => new TimeSpan(quantize(value.Ticks, unit)));

    private static TransformerRule<T, T> Pick<T>(
        string operation,
        Func<DateTime, DateTime>? dateTime = null,
        Func<DateTimeOffset, DateTimeOffset>? dateTimeOffset = null,
        Func<DateOnly, DateOnly>? dateOnly = null,
        Func<TimeOnly, TimeOnly>? timeOnly = null,
        Func<TimeSpan, TimeSpan>? timeSpan = null)
    {
        if (typeof(T) == typeof(DateTime) && dateTime is not null)
        {
            return Adapt<T, DateTime>(dateTime);
        }

        if (typeof(T) == typeof(DateTimeOffset) && dateTimeOffset is not null)
        {
            return Adapt<T, DateTimeOffset>(dateTimeOffset);
        }

        if (typeof(T) == typeof(DateOnly) && dateOnly is not null)
        {
            return Adapt<T, DateOnly>(dateOnly);
        }

        if (typeof(T) == typeof(TimeOnly) && timeOnly is not null)
        {
            return Adapt<T, TimeOnly>(timeOnly);
        }

        if (typeof(T) == typeof(TimeSpan) && timeSpan is not null)
        {
            return Adapt<T, TimeSpan>(timeSpan);
        }

        throw new NotSupportedException($"{operation} does not support {typeof(T).Name}.");
    }

    private static TransformerRule<T, T> Adapt<T, TConcrete>(Func<TConcrete, TConcrete> map)
    {
        var rule = new TransformerRule<TConcrete, TConcrete>((value, _) => ValueTask.FromResult(map(value)));
        return (TransformerRule<T, T>)(object)rule;
    }

    private static TransformerRule<T, T> ClampCore<T>(T min, T max)
        where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
        {
            throw new ArgumentException("The minimum must be less than or equal to the maximum.", nameof(min));
        }

        return Same<T>(source =>
        {
            if (source.CompareTo(min) < 0)
            {
                return min;
            }

            if (source.CompareTo(max) > 0)
            {
                return max;
            }

            return source;
        });
    }

    private static long RequirePositiveInterval(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "The interval must be a positive duration.");
        }

        return interval.Ticks;
    }

    private static long TruncateTowardZero(long ticks, long unit)
    {
        var quotient = Math.DivRem(ticks, unit, out _);
        return quotient * unit;
    }

    private static long CeilingTowardPositiveInfinity(long ticks, long unit)
    {
        var quotient = Math.DivRem(ticks, unit, out var remainder);
        if (remainder == 0)
        {
            return ticks;
        }

        return remainder > 0 ? (quotient + 1) * unit : quotient * unit;
    }

    private static long RoundHalfUp(long ticks, long unit)
    {
        var quotient = Math.DivRem(ticks, unit, out var remainder);
        if (remainder == 0)
        {
            return ticks;
        }

        var absoluteRemainder = Math.Abs(remainder);
        if (absoluteRemainder * 2 < unit)
        {
            return quotient * unit;
        }

        if (absoluteRemainder * 2 > unit)
        {
            return remainder > 0 ? (quotient + 1) * unit : (quotient - 1) * unit;
        }

        return remainder > 0 ? (quotient + 1) * unit : quotient * unit;
    }

    private static long WrapDay(long ticks)
    {
        var day = TimeSpan.TicksPerDay;
        var remainder = ticks % day;
        return remainder < 0 ? remainder + day : remainder;
    }

    private static int QuarterStartMonth(int month) => ((month - 1) / 3) * 3 + 1;

    private static int DaysUntil(DayOfWeek from, DayOfWeek to)
    {
        var diff = ((int)to - (int)from + 7) % 7;
        return diff == 0 ? 7 : diff;
    }

    private static int DaysSince(DayOfWeek from, DayOfWeek to, bool includeSameDay)
    {
        var diff = ((int)from - (int)to + 7) % 7;
        return diff == 0 && !includeSameDay ? 7 : diff;
    }

    private static DateOnly EndOfMonth(DateOnly value)
        => new(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));

    private static DateTime WithDate(DateTime value, int year, int month, int day)
        => new DateTime(year, month, day, 0, 0, 0, value.Kind).Add(value.TimeOfDay);

    private static DateTimeOffset WithDate(DateTimeOffset value, int year, int month, int day)
        => new DateTimeOffset(year, month, day, 0, 0, 0, value.Offset).Add(value.TimeOfDay);

    private static DateTime WithTime(DateTime value, int hour, int minute, int second, int millisecond)
    {
        var subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return value.Date.Add(new TimeSpan(0, hour, minute, second, millisecond) + TimeSpan.FromTicks(subMillisecond));
    }

    private static DateTimeOffset WithTime(DateTimeOffset value, int hour, int minute, int second, int millisecond)
    {
        var subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset)
            .Add(new TimeSpan(0, hour, minute, second, millisecond) + TimeSpan.FromTicks(subMillisecond));
    }

    private static TimeOnly WithTime(TimeOnly value, int hour, int minute, int second, int millisecond)
    {
        var subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return new TimeOnly(new TimeSpan(0, hour, minute, second, millisecond).Ticks + subMillisecond);
    }

    private static HashSet<DateOnly>? CopyHolidays(IEnumerable<DateOnly>? holidays)
        => holidays is null ? null : new HashSet<DateOnly>(holidays);

    private static DateTime MoveBusinessDays(DateTime value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0)
        {
            return value;
        }

        var step = Math.Sign(count);
        var remaining = Math.Abs(count);
        var result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(DateOnly.FromDateTime(result), holidays))
            {
                remaining--;
            }
        }

        return result;
    }

    private static DateTimeOffset MoveBusinessDays(DateTimeOffset value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0)
        {
            return value;
        }

        var step = Math.Sign(count);
        var remaining = Math.Abs(count);
        var result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(DateOnly.FromDateTime(result.DateTime), holidays))
            {
                remaining--;
            }
        }

        return result;
    }

    private static DateOnly MoveBusinessDays(DateOnly value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0)
        {
            return value;
        }

        var step = Math.Sign(count);
        var remaining = Math.Abs(count);
        var result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(result, holidays))
            {
                remaining--;
            }
        }

        return result;
    }

    private static bool IsBusinessDay(DateOnly date, HashSet<DateOnly>? holidays)
        => date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)
            && (holidays is null || !holidays.Contains(date));
}
