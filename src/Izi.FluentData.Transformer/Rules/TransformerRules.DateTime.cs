namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// Factory methods for the built-in date/time transformation steps. Every rule is a <c>T -&gt; T</c> map, so the
/// flowing type is fixed by the caller: pass it explicitly, e.g. <c>AddDays&lt;DateTime&gt;(1)</c> or
/// <c>StartOfMonth&lt;DateOnly&gt;()</c>. Supported <c>T</c> values are <see cref="DateTime"/>,
/// <see cref="DateTimeOffset"/>, <see cref="TimeSpan"/>, <see cref="DateOnly"/> and <see cref="TimeOnly"/>; a rule
/// throws <see cref="NotSupportedException"/> when applied to a type the operation does not cover. End-of-period
/// rules return the last representable instant of the period for time-bearing types (start of the next period minus
/// one tick) and the last day for <see cref="DateOnly"/>.
/// </summary>
public static partial class TransformerRules
{
    // =============================
    // Arithmetic / offset
    // =============================

    /// <summary>Creates a step that adds <paramref name="years"/> years. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> AddYears<T>(int years) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddYears(years)),
        DateTimeOffset dto => As<T>(dto.AddYears(years)),
        DateOnly d => As<T>(d.AddYears(years)),
        _ => throw Unsupported<T>(nameof(AddYears)),
    });

    /// <summary>Creates a step that adds <paramref name="months"/> months. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> AddMonths<T>(int months) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddMonths(months)),
        DateTimeOffset dto => As<T>(dto.AddMonths(months)),
        DateOnly d => As<T>(d.AddMonths(months)),
        _ => throw Unsupported<T>(nameof(AddMonths)),
    });

    /// <summary>Creates a step that adds <paramref name="days"/> days (truncated to whole days for DateOnly). Supports DateTime, DateTimeOffset, DateOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddDays<T>(double days) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddDays(days)),
        DateTimeOffset dto => As<T>(dto.AddDays(days)),
        DateOnly d => As<T>(d.AddDays((int)days)),
        TimeSpan ts => As<T>(ts + TimeSpan.FromDays(days)),
        _ => throw Unsupported<T>(nameof(AddDays)),
    });

    /// <summary>Creates a step that adds <paramref name="hours"/> hours (wrapping past midnight for TimeOnly). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddHours<T>(double hours) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddHours(hours)),
        DateTimeOffset dto => As<T>(dto.AddHours(hours)),
        TimeOnly to => As<T>(to.AddHours(hours)),
        TimeSpan ts => As<T>(ts + TimeSpan.FromHours(hours)),
        _ => throw Unsupported<T>(nameof(AddHours)),
    });

    /// <summary>Creates a step that adds <paramref name="minutes"/> minutes (wrapping past midnight for TimeOnly). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddMinutes<T>(double minutes) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddMinutes(minutes)),
        DateTimeOffset dto => As<T>(dto.AddMinutes(minutes)),
        TimeOnly to => As<T>(to.Add(TimeSpan.FromMinutes(minutes))),
        TimeSpan ts => As<T>(ts + TimeSpan.FromMinutes(minutes)),
        _ => throw Unsupported<T>(nameof(AddMinutes)),
    });

    /// <summary>Creates a step that adds <paramref name="seconds"/> seconds (wrapping past midnight for TimeOnly). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddSeconds<T>(double seconds) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddSeconds(seconds)),
        DateTimeOffset dto => As<T>(dto.AddSeconds(seconds)),
        TimeOnly to => As<T>(to.Add(TimeSpan.FromSeconds(seconds))),
        TimeSpan ts => As<T>(ts + TimeSpan.FromSeconds(seconds)),
        _ => throw Unsupported<T>(nameof(AddSeconds)),
    });

    /// <summary>Creates a step that adds <paramref name="milliseconds"/> milliseconds (wrapping past midnight for TimeOnly). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddMilliseconds<T>(double milliseconds) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddMilliseconds(milliseconds)),
        DateTimeOffset dto => As<T>(dto.AddMilliseconds(milliseconds)),
        TimeOnly to => As<T>(to.Add(TimeSpan.FromMilliseconds(milliseconds))),
        TimeSpan ts => As<T>(ts + TimeSpan.FromMilliseconds(milliseconds)),
        _ => throw Unsupported<T>(nameof(AddMilliseconds)),
    });

    /// <summary>Creates a step that adds <paramref name="ticks"/> ticks (wrapping past midnight for TimeOnly). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> AddTicks<T>(long ticks) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddTicks(ticks)),
        DateTimeOffset dto => As<T>(dto.AddTicks(ticks)),
        TimeOnly to => As<T>(to.Add(TimeSpan.FromTicks(ticks))),
        TimeSpan ts => As<T>(ts + TimeSpan.FromTicks(ticks)),
        _ => throw Unsupported<T>(nameof(AddTicks)),
    });

    /// <summary>Creates a step that adds <paramref name="value"/> to the current value. Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> Add<T>(TimeSpan value) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.Add(value)),
        DateTimeOffset dto => As<T>(dto.Add(value)),
        TimeOnly to => As<T>(to.Add(value)),
        TimeSpan ts => As<T>(ts.Add(value)),
        _ => throw Unsupported<T>(nameof(Add)),
    });

    /// <summary>Creates a step that subtracts <paramref name="value"/> from the current value. Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> Subtract<T>(TimeSpan value) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.Subtract(value)),
        DateTimeOffset dto => As<T>(dto.Subtract(value)),
        TimeOnly to => As<T>(to.Add(-value)),
        TimeSpan ts => As<T>(ts.Subtract(value)),
        _ => throw Unsupported<T>(nameof(Subtract)),
    });

    /// <summary>Creates a step that negates the current <see cref="TimeSpan"/> (flips its sign).</summary>
    public static TransformerRule<TimeSpan> Negate() => new((source, _) => ValueTask.FromResult(source.Negate()));

    /// <summary>Creates a step that takes the absolute duration of the current <see cref="TimeSpan"/>.</summary>
    public static TransformerRule<TimeSpan> Duration() => new((source, _) => ValueTask.FromResult(source.Duration()));

    // =============================
    // Clamp / range
    // =============================

    /// <summary>Creates a step that clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<DateTime> Clamp(DateTime min, DateTime max) => new((source, _) => ValueTask.FromResult(source < min ? min : source > max ? max : source));

    /// <summary>Creates a step that clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<DateTimeOffset> Clamp(DateTimeOffset min, DateTimeOffset max) => new((source, _) => ValueTask.FromResult(source < min ? min : source > max ? max : source));

    /// <summary>Creates a step that clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<TimeSpan> Clamp(TimeSpan min, TimeSpan max) => new((source, _) => ValueTask.FromResult(source < min ? min : source > max ? max : source));

    /// <summary>Creates a step that clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<DateOnly> Clamp(DateOnly min, DateOnly max) => new((source, _) => ValueTask.FromResult(source < min ? min : source > max ? max : source));

    /// <summary>Creates a step that clamps the current value to the inclusive range <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static TransformerRule<TimeOnly> Clamp(TimeOnly min, TimeOnly max) => new((source, _) => ValueTask.FromResult(source < min ? min : source > max ? max : source));

    // =============================
    // Start / end of a calendar period
    // =============================

    /// <summary>Creates a step that snaps to the start of the day (midnight). Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> StartOfDay<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.Date),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Date, dto.Offset)),
        _ => throw Unsupported<T>(nameof(StartOfDay)),
    });

    /// <summary>Creates a step that snaps to the last instant of the day. Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> EndOfDay<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.Date.AddDays(1).AddTicks(-1)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Date, dto.Offset).AddDays(1).AddTicks(-1)),
        _ => throw Unsupported<T>(nameof(EndOfDay)),
    });

    /// <summary>Creates a step that snaps back to <paramref name="firstDayOfWeek"/>. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> StartOfWeek<T>(DayOfWeek firstDayOfWeek) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(StartOfWeek(dt, firstDayOfWeek)),
        DateTimeOffset dto => As<T>(StartOfWeek(dto, firstDayOfWeek)),
        DateOnly d => As<T>(d.AddDays(-DaysSince(d.DayOfWeek, firstDayOfWeek, includeSameDay: true))),
        _ => throw Unsupported<T>(nameof(StartOfWeek)),
    });

    /// <summary>Creates a step that snaps forward to the end of the week beginning on <paramref name="firstDayOfWeek"/>. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> EndOfWeek<T>(DayOfWeek firstDayOfWeek) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(StartOfWeek(dt, firstDayOfWeek).AddDays(7).AddTicks(-1)),
        DateTimeOffset dto => As<T>(StartOfWeek(dto, firstDayOfWeek).AddDays(7).AddTicks(-1)),
        DateOnly d => As<T>(d.AddDays(-DaysSince(d.DayOfWeek, firstDayOfWeek, includeSameDay: true) + 6)),
        _ => throw Unsupported<T>(nameof(EndOfWeek)),
    });

    /// <summary>Creates a step that snaps to the first day of the month. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> StartOfMonth<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, dto.Month, 1, 0, 0, 0, dto.Offset)),
        DateOnly d => As<T>(new DateOnly(d.Year, d.Month, 1)),
        _ => throw Unsupported<T>(nameof(StartOfMonth)),
    });

    /// <summary>Creates a step that snaps to the end of the month. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> EndOfMonth<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind).AddMonths(1).AddTicks(-1)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, dto.Month, 1, 0, 0, 0, dto.Offset).AddMonths(1).AddTicks(-1)),
        DateOnly d => As<T>(new DateOnly(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month))),
        _ => throw Unsupported<T>(nameof(EndOfMonth)),
    });

    /// <summary>Creates a step that snaps to the first day of the calendar quarter. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> StartOfQuarter<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, QuarterStartMonth(dt.Month), 1, 0, 0, 0, dt.Kind)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, QuarterStartMonth(dto.Month), 1, 0, 0, 0, dto.Offset)),
        DateOnly d => As<T>(new DateOnly(d.Year, QuarterStartMonth(d.Month), 1)),
        _ => throw Unsupported<T>(nameof(StartOfQuarter)),
    });

    /// <summary>Creates a step that snaps to the end of the calendar quarter. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> EndOfQuarter<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, QuarterStartMonth(dt.Month), 1, 0, 0, 0, dt.Kind).AddMonths(3).AddTicks(-1)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, QuarterStartMonth(dto.Month), 1, 0, 0, 0, dto.Offset).AddMonths(3).AddTicks(-1)),
        DateOnly d => As<T>(EndOfMonthCore(new DateOnly(d.Year, QuarterStartMonth(d.Month), 1).AddMonths(2))),
        _ => throw Unsupported<T>(nameof(EndOfQuarter)),
    });

    /// <summary>Creates a step that snaps to the first day of the year. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> StartOfYear<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, 1, 1, 0, 0, 0, dt.Kind)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, 1, 1, 0, 0, 0, dto.Offset)),
        DateOnly d => As<T>(new DateOnly(d.Year, 1, 1)),
        _ => throw Unsupported<T>(nameof(StartOfYear)),
    });

    /// <summary>Creates a step that snaps to the end of the year. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> EndOfYear<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(new DateTime(dt.Year, 1, 1, 0, 0, 0, dt.Kind).AddYears(1).AddTicks(-1)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Year, 1, 1, 0, 0, 0, dto.Offset).AddYears(1).AddTicks(-1)),
        DateOnly d => As<T>(new DateOnly(d.Year, 12, 31)),
        _ => throw Unsupported<T>(nameof(EndOfYear)),
    });

    // =============================
    // Rounding to an interval
    // =============================

    /// <summary>Creates a step that snaps to the nearest multiple of <paramref name="interval"/> (halves round up). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> RoundTo<T>(TimeSpan interval)
    {
        long unit = RequirePositive(interval);
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(new DateTime(RoundTicks(dt.Ticks, unit), dt.Kind)),
            DateTimeOffset dto => As<T>(new DateTimeOffset(RoundTicks(dto.Ticks, unit), dto.Offset)),
            TimeOnly to => As<T>(new TimeOnly(WrapTicks(RoundTicks(to.Ticks, unit)))),
            TimeSpan ts => As<T>(new TimeSpan(RoundTicks(ts.Ticks, unit))),
            _ => throw Unsupported<T>(nameof(RoundTo)),
        });
    }

    /// <summary>Creates a step that snaps down to a multiple of <paramref name="interval"/> (toward zero). Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> TruncateTo<T>(TimeSpan interval)
    {
        long unit = RequirePositive(interval);
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(new DateTime(FloorTicks(dt.Ticks, unit), dt.Kind)),
            DateTimeOffset dto => As<T>(new DateTimeOffset(FloorTicks(dto.Ticks, unit), dto.Offset)),
            TimeOnly to => As<T>(new TimeOnly(WrapTicks(FloorTicks(to.Ticks, unit)))),
            TimeSpan ts => As<T>(new TimeSpan(FloorTicks(ts.Ticks, unit))),
            _ => throw Unsupported<T>(nameof(TruncateTo)),
        });
    }

    /// <summary>Creates a step that snaps up to a multiple of <paramref name="interval"/>. Supports DateTime, DateTimeOffset, TimeOnly, TimeSpan.</summary>
    public static TransformerRule<T> CeilingTo<T>(TimeSpan interval)
    {
        long unit = RequirePositive(interval);
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(new DateTime(CeilingTicks(dt.Ticks, unit), dt.Kind)),
            DateTimeOffset dto => As<T>(new DateTimeOffset(CeilingTicks(dto.Ticks, unit), dto.Offset)),
            TimeOnly to => As<T>(new TimeOnly(WrapTicks(CeilingTicks(to.Ticks, unit)))),
            TimeSpan ts => As<T>(new TimeSpan(CeilingTicks(ts.Ticks, unit))),
            _ => throw Unsupported<T>(nameof(CeilingTo)),
        });
    }

    // =============================
    // Component replacement ("with" setters)
    // =============================

    /// <summary>Creates a step that replaces the year. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> WithYear<T>(int year) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(WithDate(dt, year, dt.Month, dt.Day)),
        DateTimeOffset dto => As<T>(WithDate(dto, year, dto.Month, dto.Day)),
        DateOnly d => As<T>(new DateOnly(year, d.Month, d.Day)),
        _ => throw Unsupported<T>(nameof(WithYear)),
    });

    /// <summary>Creates a step that replaces the month (1-12). Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> WithMonth<T>(int month) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(WithDate(dt, dt.Year, month, dt.Day)),
        DateTimeOffset dto => As<T>(WithDate(dto, dto.Year, month, dto.Day)),
        DateOnly d => As<T>(new DateOnly(d.Year, month, d.Day)),
        _ => throw Unsupported<T>(nameof(WithMonth)),
    });

    /// <summary>Creates a step that replaces the day of the month. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> WithDay<T>(int day) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(WithDate(dt, dt.Year, dt.Month, day)),
        DateTimeOffset dto => As<T>(WithDate(dto, dto.Year, dto.Month, day)),
        DateOnly d => As<T>(new DateOnly(d.Year, d.Month, day)),
        _ => throw Unsupported<T>(nameof(WithDay)),
    });

    /// <summary>Creates a step that replaces the hour (0-23). Supports DateTime, DateTimeOffset, TimeOnly.</summary>
    public static TransformerRule<T> WithHour<T>(int hour)
    {
        if (hour is < 0 or > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(WithTime(dt, hour, dt.Minute, dt.Second, dt.Millisecond)),
            DateTimeOffset dto => As<T>(WithTime(dto, hour, dto.Minute, dto.Second, dto.Millisecond)),
            TimeOnly to => As<T>(WithTime(to, hour, to.Minute, to.Second, to.Millisecond)),
            _ => throw Unsupported<T>(nameof(WithHour)),
        });
    }

    /// <summary>Creates a step that replaces the minute (0-59). Supports DateTime, DateTimeOffset, TimeOnly.</summary>
    public static TransformerRule<T> WithMinute<T>(int minute)
    {
        if (minute is < 0 or > 59) throw new ArgumentOutOfRangeException(nameof(minute));
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(WithTime(dt, dt.Hour, minute, dt.Second, dt.Millisecond)),
            DateTimeOffset dto => As<T>(WithTime(dto, dto.Hour, minute, dto.Second, dto.Millisecond)),
            TimeOnly to => As<T>(WithTime(to, to.Hour, minute, to.Second, to.Millisecond)),
            _ => throw Unsupported<T>(nameof(WithMinute)),
        });
    }

    /// <summary>Creates a step that replaces the second (0-59). Supports DateTime, DateTimeOffset, TimeOnly.</summary>
    public static TransformerRule<T> WithSecond<T>(int second)
    {
        if (second is < 0 or > 59) throw new ArgumentOutOfRangeException(nameof(second));
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(WithTime(dt, dt.Hour, dt.Minute, second, dt.Millisecond)),
            DateTimeOffset dto => As<T>(WithTime(dto, dto.Hour, dto.Minute, second, dto.Millisecond)),
            TimeOnly to => As<T>(WithTime(to, to.Hour, to.Minute, second, to.Millisecond)),
            _ => throw Unsupported<T>(nameof(WithSecond)),
        });
    }

    /// <summary>Creates a step that replaces the millisecond (0-999). Supports DateTime, DateTimeOffset, TimeOnly.</summary>
    public static TransformerRule<T> WithMillisecond<T>(int millisecond)
    {
        if (millisecond is < 0 or > 999) throw new ArgumentOutOfRangeException(nameof(millisecond));
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(WithTime(dt, dt.Hour, dt.Minute, dt.Second, millisecond)),
            DateTimeOffset dto => As<T>(WithTime(dto, dto.Hour, dto.Minute, dto.Second, millisecond)),
            TimeOnly to => As<T>(WithTime(to, to.Hour, to.Minute, to.Second, millisecond)),
            _ => throw Unsupported<T>(nameof(WithMillisecond)),
        });
    }

    /// <summary>Creates a step that replaces the time of day, keeping the date. Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> SetTimeOfDay<T>(TimeOnly timeOfDay) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.Date.Add(timeOfDay.ToTimeSpan())),
        DateTimeOffset dto => As<T>(new DateTimeOffset(dto.Date, dto.Offset).Add(timeOfDay.ToTimeSpan())),
        _ => throw Unsupported<T>(nameof(SetTimeOfDay)),
    });

    /// <summary>Creates a step that replaces the date, keeping the time of day. Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> SetDate<T>(DateOnly date) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(date.ToDateTime(TimeOnly.FromTimeSpan(dt.TimeOfDay), dt.Kind)),
        DateTimeOffset dto => As<T>(new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, dto.Offset).Add(dto.TimeOfDay)),
        _ => throw Unsupported<T>(nameof(SetDate)),
    });

    // =============================
    // Calendar navigation
    // =============================

    /// <summary>Creates a step that moves forward to the next <paramref name="dayOfWeek"/> (strictly after the current day). Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> NextDayOfWeek<T>(DayOfWeek dayOfWeek) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddDays(DaysUntil(dt.DayOfWeek, dayOfWeek))),
        DateTimeOffset dto => As<T>(dto.AddDays(DaysUntil(dto.DayOfWeek, dayOfWeek))),
        DateOnly d => As<T>(d.AddDays(DaysUntil(d.DayOfWeek, dayOfWeek))),
        _ => throw Unsupported<T>(nameof(NextDayOfWeek)),
    });

    /// <summary>Creates a step that moves back to the previous <paramref name="dayOfWeek"/> (strictly before the current day). Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> PreviousDayOfWeek<T>(DayOfWeek dayOfWeek) => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.AddDays(-DaysSince(dt.DayOfWeek, dayOfWeek, includeSameDay: false))),
        DateTimeOffset dto => As<T>(dto.AddDays(-DaysSince(dto.DayOfWeek, dayOfWeek, includeSameDay: false))),
        DateOnly d => As<T>(d.AddDays(-DaysSince(d.DayOfWeek, dayOfWeek, includeSameDay: false))),
        _ => throw Unsupported<T>(nameof(PreviousDayOfWeek)),
    });

    /// <summary>Creates a step that adds <paramref name="count"/> business days, skipping weekends and any <paramref name="holidays"/>. Negative counts move backward. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> AddBusinessDays<T>(int count, IEnumerable<DateOnly>? holidays = null)
    {
        var holidaySet = holidays is null ? null : new HashSet<DateOnly>(holidays);
        return Rule<T>(source => source switch
        {
            DateTime dt => As<T>(AddBusinessDays(dt, count, holidaySet)),
            DateTimeOffset dto => As<T>(AddBusinessDays(dto, count, holidaySet)),
            DateOnly d => As<T>(AddBusinessDays(d, count, holidaySet)),
            _ => throw Unsupported<T>(nameof(AddBusinessDays)),
        });
    }

    /// <summary>Creates a step that moves to the next business day, skipping weekends and any <paramref name="holidays"/>. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> NextBusinessDay<T>(IEnumerable<DateOnly>? holidays = null) => AddBusinessDays<T>(1, holidays);

    /// <summary>Creates a step that moves to the previous business day, skipping weekends and any <paramref name="holidays"/>. Supports DateTime, DateTimeOffset, DateOnly.</summary>
    public static TransformerRule<T> PreviousBusinessDay<T>(IEnumerable<DateOnly>? holidays = null) => AddBusinessDays<T>(-1, holidays);

    // =============================
    // Kind / time zone
    // =============================

    /// <summary>Creates a step that converts to Coordinated Universal Time. Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> ToUniversalTime<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.ToUniversalTime()),
        DateTimeOffset dto => As<T>(dto.ToUniversalTime()),
        _ => throw Unsupported<T>(nameof(ToUniversalTime)),
    });

    /// <summary>Creates a step that converts to local time. Supports DateTime, DateTimeOffset.</summary>
    public static TransformerRule<T> ToLocalTime<T>() => Rule<T>(source => source switch
    {
        DateTime dt => As<T>(dt.ToLocalTime()),
        DateTimeOffset dto => As<T>(dto.ToLocalTime()),
        _ => throw Unsupported<T>(nameof(ToLocalTime)),
    });

    /// <summary>Creates a step that stamps the current <see cref="DateTime"/> with <paramref name="kind"/> without shifting the clock value.</summary>
    public static TransformerRule<DateTime> SpecifyKind(DateTimeKind kind) => new((source, _) => ValueTask.FromResult(DateTime.SpecifyKind(source, kind)));

    /// <summary>Creates a step that adjusts the current <see cref="DateTimeOffset"/> to <paramref name="offset"/>, preserving the instant.</summary>
    public static TransformerRule<DateTimeOffset> ToOffset(TimeSpan offset) => new((source, _) => ValueTask.FromResult(source.ToOffset(offset)));

    /// <summary>Creates a step that converts the current <see cref="DateTimeOffset"/> into <paramref name="timeZone"/>, preserving the instant.</summary>
    public static TransformerRule<DateTimeOffset> ConvertToTimeZone(TimeZoneInfo timeZone) => new((source, _) => ValueTask.FromResult(TimeZoneInfo.ConvertTime(source, timeZone)));

    // =============================
    // Sentinel defaults
    // =============================

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTime.MinValue"/>.</summary>
    public static TransformerRule<DateTime> DefaultIfMinValue(DateTime defaultValue) => DefaultIfMinValue(DateTime.MinValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTimeOffset.MinValue"/>.</summary>
    public static TransformerRule<DateTimeOffset> DefaultIfMinValue(DateTimeOffset defaultValue) => DefaultIfMinValue(DateTimeOffset.MinValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeSpan.MinValue"/>.</summary>
    public static TransformerRule<TimeSpan> DefaultIfMinValue(TimeSpan defaultValue) => DefaultIfMinValue(TimeSpan.MinValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateOnly.MinValue"/>.</summary>
    public static TransformerRule<DateOnly> DefaultIfMinValue(DateOnly defaultValue) => DefaultIfMinValue(DateOnly.MinValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeOnly.MinValue"/>.</summary>
    public static TransformerRule<TimeOnly> DefaultIfMinValue(TimeOnly defaultValue) => DefaultIfMinValue(TimeOnly.MinValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTime.MaxValue"/>.</summary>
    public static TransformerRule<DateTime> DefaultIfMaxValue(DateTime defaultValue) => DefaultIfMaxValue(DateTime.MaxValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateTimeOffset.MaxValue"/>.</summary>
    public static TransformerRule<DateTimeOffset> DefaultIfMaxValue(DateTimeOffset defaultValue) => DefaultIfMaxValue(DateTimeOffset.MaxValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeSpan.MaxValue"/>.</summary>
    public static TransformerRule<TimeSpan> DefaultIfMaxValue(TimeSpan defaultValue) => DefaultIfMaxValue(TimeSpan.MaxValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="DateOnly.MaxValue"/>.</summary>
    public static TransformerRule<DateOnly> DefaultIfMaxValue(DateOnly defaultValue) => DefaultIfMaxValue(DateOnly.MaxValue, defaultValue);

    /// <summary>Creates a step that substitutes <paramref name="defaultValue"/> when the current value is <see cref="TimeOnly.MaxValue"/>.</summary>
    public static TransformerRule<TimeOnly> DefaultIfMaxValue(TimeOnly defaultValue) => DefaultIfMaxValue(TimeOnly.MaxValue, defaultValue);

    // =============================
    // Helpers
    // =============================

    /// <summary>Wraps a same-type mapping in a synchronous <see cref="TransformerRule{T}"/>.</summary>
    private static TransformerRule<T> Rule<T>(Func<T, T> map) => new((source, _) => ValueTask.FromResult(map(source)));

    /// <summary>Boxes a concrete date/time result back into the flowing generic type.</summary>
    private static T As<T>(object value) => (T)value;

    private static NotSupportedException Unsupported<T>(string operation) => new($"{operation} does not support the type '{typeof(T)}'.");

    private static long RequirePositive(TimeSpan interval) => interval > TimeSpan.Zero
        ? interval.Ticks
        : throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be a positive duration.");

    private static long FloorTicks(long ticks, long unit) => ticks - (ticks % unit);

    private static long CeilingTicks(long ticks, long unit)
    {
        long remainder = ticks % unit;
        return remainder == 0 ? ticks : ticks - remainder + unit;
    }

    private static long RoundTicks(long ticks, long unit)
    {
        long remainder = ticks % unit;
        return remainder * 2 >= unit ? ticks - remainder + unit : ticks - remainder;
    }

    /// <summary>Folds a tick count into the <c>[0, one day)</c> range so <see cref="TimeOnly"/> results stay valid after rounding.</summary>
    private static long WrapTicks(long ticks) => ((ticks % TimeSpan.TicksPerDay) + TimeSpan.TicksPerDay) % TimeSpan.TicksPerDay;

    private static int QuarterStartMonth(int month) => ((month - 1) / 3) * 3 + 1;

    /// <summary>Number of days from <paramref name="from"/> forward to the next <paramref name="to"/>, always 1-7 (never the same day).</summary>
    private static int DaysUntil(DayOfWeek from, DayOfWeek to)
    {
        int diff = ((int)to - (int)from + 7) % 7;
        return diff == 0 ? 7 : diff;
    }

    /// <summary>Number of days from <paramref name="from"/> back to <paramref name="to"/>. When <paramref name="includeSameDay"/> is true a match returns 0; otherwise it returns 7.</summary>
    private static int DaysSince(DayOfWeek from, DayOfWeek to, bool includeSameDay)
    {
        int diff = ((int)from - (int)to + 7) % 7;
        return diff == 0 && !includeSameDay ? 7 : diff;
    }

    private static DateTime StartOfWeek(DateTime value, DayOfWeek firstDayOfWeek)
        => value.Date.AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true));

    private static DateTimeOffset StartOfWeek(DateTimeOffset value, DayOfWeek firstDayOfWeek)
        => new DateTimeOffset(value.Date, value.Offset).AddDays(-DaysSince(value.DayOfWeek, firstDayOfWeek, includeSameDay: true));

    private static DateOnly EndOfMonthCore(DateOnly value) => new(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));

    private static DateTime WithDate(DateTime value, int year, int month, int day)
        => new DateTime(year, month, day, 0, 0, 0, value.Kind).Add(value.TimeOfDay);

    private static DateTimeOffset WithDate(DateTimeOffset value, int year, int month, int day)
        => new DateTimeOffset(year, month, day, 0, 0, 0, value.Offset).Add(value.TimeOfDay);

    private static DateTime WithTime(DateTime value, int hour, int minute, int second, int millisecond)
    {
        long subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return value.Date.Add(new TimeSpan(0, hour, minute, second, millisecond) + TimeSpan.FromTicks(subMillisecond));
    }

    private static DateTimeOffset WithTime(DateTimeOffset value, int hour, int minute, int second, int millisecond)
    {
        long subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset)
            .Add(new TimeSpan(0, hour, minute, second, millisecond) + TimeSpan.FromTicks(subMillisecond));
    }

    private static TimeOnly WithTime(TimeOnly value, int hour, int minute, int second, int millisecond)
    {
        long subMillisecond = value.Ticks % TimeSpan.TicksPerMillisecond;
        return new TimeOnly(new TimeSpan(0, hour, minute, second, millisecond).Ticks + subMillisecond);
    }

    private static DateTime AddBusinessDays(DateTime value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0) return value;
        int step = Math.Sign(count);
        int remaining = Math.Abs(count);
        DateTime result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(DateOnly.FromDateTime(result), holidays)) remaining--;
        }
        return result;
    }

    private static DateTimeOffset AddBusinessDays(DateTimeOffset value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0) return value;
        int step = Math.Sign(count);
        int remaining = Math.Abs(count);
        DateTimeOffset result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(DateOnly.FromDateTime(result.DateTime), holidays)) remaining--;
        }
        return result;
    }

    private static DateOnly AddBusinessDays(DateOnly value, int count, HashSet<DateOnly>? holidays)
    {
        if (count == 0) return value;
        int step = Math.Sign(count);
        int remaining = Math.Abs(count);
        DateOnly result = value;
        while (remaining > 0)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(result, holidays)) remaining--;
        }
        return result;
    }

    private static bool IsBusinessDay(DateOnly date, HashSet<DateOnly>? holidays)
        => date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && (holidays is null || !holidays.Contains(date));
}
