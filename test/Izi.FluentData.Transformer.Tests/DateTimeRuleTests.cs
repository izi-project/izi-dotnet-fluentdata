namespace Izi.FluentData.Transformer.Tests;

public class DateTimeRuleTests
{
    private static readonly DateTime Sample = new(2020, 5, 15, 10, 30, 40, 50, DateTimeKind.Utc);
    private static readonly DateTimeOffset SampleOffset = new(2020, 5, 15, 10, 30, 40, TimeSpan.FromHours(2));
    private static readonly DateOnly SampleDate = new(2020, 5, 15);

    [Fact]
    public async Task Adding_calendar_units_preserves_kind_and_offset_and_truncates_date_only_days()
    {
        var added = await TransformerRules.AddYears<DateTime>(1).TransformAsync(Sample);
        Assert.Equal(new DateTime(2021, 5, 15, 10, 30, 40, 50, DateTimeKind.Utc), added);
        Assert.Equal(new DateTimeOffset(2021, 5, 15, 10, 30, 40, TimeSpan.FromHours(2)), await TransformerRules.AddMonths<DateTimeOffset>(12).TransformAsync(SampleOffset));
        Assert.Equal(new DateOnly(2020, 5, 16), await TransformerRules.AddDays<DateOnly>(1.9).TransformAsync(SampleDate));
        Assert.Equal(TimeSpan.FromDays(1.5), await TransformerRules.AddDays<TimeSpan>(1.5).TransformAsync(TimeSpan.Zero));
        Assert.Throws<NotSupportedException>(() => TransformerRules.AddYears<TimeSpan>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.AddYears<DateTime>(1).TransformAsync(DateTime.MaxValue));
    }

    [Fact]
    public async Task Adding_time_units_wraps_time_only_and_can_chain()
    {
        Assert.Equal(new TimeOnly(1, 0), await TransformerRules.AddHours<TimeOnly>(2).TransformAsync(new TimeOnly(23, 0)));
        Assert.Equal(Sample.AddMinutes(90), await TransformerRules.AddMinutes<DateTime>(90).TransformAsync(Sample));
        Assert.Equal(TimeSpan.FromSeconds(5), await TransformerRules.AddSeconds<TimeSpan>(2).TransformAsync(TimeSpan.FromSeconds(3)));
        Assert.Equal(TimeSpan.FromMilliseconds(4), await TransformerRules.AddMilliseconds<TimeSpan>(4).TransformAsync(TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromTicks(4), await TransformerRules.AddTicks<TimeSpan>(4).TransformAsync(TimeSpan.Zero));
        Assert.Equal(Sample.Add(TimeSpan.FromHours(1)), await TransformerRules.Add<DateTime>(TimeSpan.FromHours(1)).TransformAsync(Sample));
        Assert.Equal(SampleOffset.Subtract(TimeSpan.FromMinutes(30)), await TransformerRules.Subtract<DateTimeOffset>(TimeSpan.FromMinutes(30)).TransformAsync(SampleOffset));
        Assert.Equal(new TimeOnly(9, 30), await TransformerRules.Subtract<TimeOnly>(TimeSpan.FromHours(1)).TransformAsync(new TimeOnly(10, 30)));
        Assert.Equal(TimeSpan.FromHours(-2), await TransformerRules.Negate().TransformAsync(TimeSpan.FromHours(2)));
        Assert.Equal(TimeSpan.FromHours(2), await TransformerRules.Duration().TransformAsync(TimeSpan.FromHours(-2)));
        Assert.Throws<OverflowException>(() => TransformerRules.Negate().TransformAsync(TimeSpan.MinValue));
    }

    [Fact]
    public async Task Period_boundaries_keep_kind_or_offset_and_use_the_last_tick_or_last_day()
    {
        Assert.Equal(new DateTime(2020, 5, 15, 0, 0, 0, DateTimeKind.Utc), await TransformerRules.StartOfDay<DateTime>().TransformAsync(Sample));
        Assert.Equal(new DateTime(2020, 5, 16, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1), await TransformerRules.EndOfDay<DateTime>().TransformAsync(Sample));
        Assert.Equal(new DateTimeOffset(2020, 5, 15, 0, 0, 0, TimeSpan.FromHours(2)), await TransformerRules.StartOfDay<DateTimeOffset>().TransformAsync(SampleOffset));

        var monday = new DateOnly(2020, 5, 11);
        Assert.Equal(monday, await TransformerRules.StartOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(monday));
        Assert.Equal(new DateOnly(2020, 5, 17), await TransformerRules.EndOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(SampleDate));
        Assert.Equal(new DateTime(2020, 5, 18, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1), await TransformerRules.EndOfWeek<DateTime>(DayOfWeek.Monday).TransformAsync(Sample));

        Assert.Equal(new DateTime(2020, 5, 1, 0, 0, 0, DateTimeKind.Utc), await TransformerRules.StartOfMonth<DateTime>().TransformAsync(Sample));
        Assert.Equal(new DateOnly(2020, 5, 31), await TransformerRules.EndOfMonth<DateOnly>().TransformAsync(SampleDate));
        Assert.Equal(new DateTime(2020, 6, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1), await TransformerRules.EndOfMonth<DateTime>().TransformAsync(Sample));
        Assert.Equal(new DateOnly(2020, 4, 1), await TransformerRules.StartOfQuarter<DateOnly>().TransformAsync(SampleDate));
        Assert.Equal(new DateOnly(2020, 6, 30), await TransformerRules.EndOfQuarter<DateOnly>().TransformAsync(SampleDate));
        Assert.Equal(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)), await TransformerRules.StartOfYear<DateTimeOffset>().TransformAsync(SampleOffset));
        Assert.Equal(new DateOnly(2020, 12, 31), await TransformerRules.EndOfYear<DateOnly>().TransformAsync(SampleDate));
        Assert.Throws<NotSupportedException>(() => TransformerRules.StartOfDay<DateOnly>());
    }

    [Fact]
    public async Task Component_replacement_keeps_the_rest_of_the_value()
    {
        Assert.Equal(new DateTime(2024, 5, 15, 10, 30, 40, 50, DateTimeKind.Utc), await TransformerRules.WithYear<DateTime>(2024).TransformAsync(Sample));
        Assert.Equal(new DateOnly(2020, 2, 15), await TransformerRules.WithMonth<DateOnly>(2).TransformAsync(SampleDate));
        Assert.Equal(new DateTimeOffset(2020, 5, 1, 10, 30, 40, TimeSpan.FromHours(2)), await TransformerRules.WithDay<DateTimeOffset>(1).TransformAsync(SampleOffset));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithDay<DateTime>(31).TransformAsync(new DateTime(2020, 2, 1)));

        var withTicks = new DateTime(2020, 5, 15, 10, 30, 40, 50, DateTimeKind.Utc).AddTicks(25);
        var replacedHour = await TransformerRules.WithHour<DateTime>(8).TransformAsync(withTicks);
        Assert.Equal(8, replacedHour.Hour);
        Assert.Equal(25, replacedHour.Ticks % TimeSpan.TicksPerMillisecond);
        Assert.Equal(DateTimeKind.Utc, replacedHour.Kind);
        Assert.Equal(new TimeOnly(10, 15, 40, 50), await TransformerRules.WithMinute<TimeOnly>(15).TransformAsync(new TimeOnly(10, 30, 40, 50)));
        Assert.Equal(new TimeOnly(10, 30, 5), await TransformerRules.WithSecond<TimeOnly>(5).TransformAsync(new TimeOnly(10, 30, 40)));
        Assert.Equal(new TimeOnly(10, 30, 40, 9), await TransformerRules.WithMillisecond<TimeOnly>(9).TransformAsync(new TimeOnly(10, 30, 40, 50)));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithHour<DateTime>(24));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithMinute<DateTime>(60));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithSecond<DateTime>(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithMillisecond<DateTime>(1000));

        Assert.Equal(new DateTime(2020, 5, 15, 1, 2, 0, DateTimeKind.Utc), await TransformerRules.SetTimeOfDay<DateTime>(new TimeOnly(1, 2)).TransformAsync(Sample));
        Assert.Equal(new DateTimeOffset(2024, 1, 2, 10, 30, 40, TimeSpan.FromHours(2)), await TransformerRules.SetDate<DateTimeOffset>(new DateOnly(2024, 1, 2)).TransformAsync(SampleOffset));
    }

    [Fact]
    public async Task Navigation_skips_the_current_day_and_copies_holidays()
    {
        var friday = new DateOnly(2020, 5, 15);
        Assert.Equal(friday.AddDays(7), await TransformerRules.NextDayOfWeek<DateOnly>(DayOfWeek.Friday).TransformAsync(friday));
        Assert.Equal(friday.AddDays(-1), await TransformerRules.PreviousDayOfWeek<DateOnly>(DayOfWeek.Thursday).TransformAsync(friday));
        Assert.Equal(new DateOnly(2020, 5, 18), await TransformerRules.NextBusinessDay<DateOnly>().TransformAsync(friday));
        Assert.Equal(new DateOnly(2020, 5, 18), await TransformerRules.AddBusinessDays<DateOnly>(1).TransformAsync(new DateOnly(2020, 5, 16)));
        Assert.Equal(friday, await TransformerRules.PreviousBusinessDay<DateOnly>().TransformAsync(new DateOnly(2020, 5, 18)));

        var holidays = new List<DateOnly> { new(2020, 5, 18) };
        var rule = TransformerRules.AddBusinessDays<DateTime>(1, holidays);
        holidays.Add(new DateOnly(2020, 5, 19));
        var moved = await rule.TransformAsync(new DateTime(2020, 5, 15, 10, 0, 0, DateTimeKind.Utc));
        Assert.Equal(new DateTime(2020, 5, 19, 10, 0, 0, DateTimeKind.Utc), moved);
    }

    [Fact]
    public async Task Interval_rounding_handles_halves_negatives_and_midnight_wrap()
    {
        var tenMinutes = TimeSpan.FromMinutes(10);
        Assert.Equal(TimeSpan.FromMinutes(10), await TransformerRules.RoundTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(5)));
        Assert.Equal(TimeSpan.Zero, await TransformerRules.RoundTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(-5)));
        Assert.Equal(TimeSpan.FromMinutes(-10), await TransformerRules.RoundTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(-7)));
        Assert.Equal(TimeSpan.Zero, await TransformerRules.CeilingTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(-7)));
        Assert.Equal(TimeSpan.Zero, await TransformerRules.TruncateTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(-7)));
        Assert.Equal(TimeSpan.FromMinutes(-10), await TransformerRules.TruncateTo<TimeSpan>(tenMinutes).TransformAsync(TimeSpan.FromMinutes(-17)));

        var rounded = await TransformerRules.RoundTo<DateTimeOffset>(tenMinutes).TransformAsync(new DateTimeOffset(2020, 6, 15, 10, 7, 0, TimeSpan.FromHours(2)));
        Assert.Equal(new DateTimeOffset(2020, 6, 15, 10, 10, 0, TimeSpan.FromHours(2)), rounded);
        Assert.Equal(new TimeOnly(0, 0), await TransformerRules.RoundTo<TimeOnly>(TimeSpan.FromMinutes(15)).TransformAsync(new TimeOnly(23, 55)));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.RoundTo<TimeSpan>(TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.CeilingTo<DateTime>(TimeSpan.FromMinutes(-1)));
        Assert.Throws<NotSupportedException>(() => TransformerRules.TruncateTo<DateOnly>(TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public async Task Clamps_kind_offset_and_zone_conversions_follow_the_bcl_rules()
    {
        Assert.Equal(SampleDate, await TransformerRules.Clamp(new DateOnly(2020, 5, 1), new DateOnly(2020, 5, 31)).TransformAsync(SampleDate));
        Assert.Equal(new DateOnly(2020, 5, 1), await TransformerRules.Clamp(new DateOnly(2020, 5, 1), new DateOnly(2020, 5, 31)).TransformAsync(new DateOnly(2020, 1, 1)));
        Assert.Equal(TimeSpan.FromHours(2), await TransformerRules.Clamp(TimeSpan.FromHours(1), TimeSpan.FromHours(2)).TransformAsync(TimeSpan.FromHours(5)));
        Assert.Equal(new TimeOnly(9, 0), await TransformerRules.Clamp(new TimeOnly(8, 0), new TimeOnly(9, 0)).TransformAsync(new TimeOnly(12, 0)));
        Assert.Throws<ArgumentException>(() => TransformerRules.Clamp(Sample, Sample.AddDays(-1)));

        Assert.Equal(DateTimeKind.Unspecified, (await TransformerRules.SpecifyKind(DateTimeKind.Unspecified).TransformAsync(Sample)).Kind);
        Assert.Equal(Sample, await TransformerRules.ToUniversalTime<DateTime>().TransformAsync(Sample));
        Assert.Equal(SampleOffset.ToUniversalTime(), await TransformerRules.ToUniversalTime<DateTimeOffset>().TransformAsync(SampleOffset));
        Assert.Equal(SampleOffset.ToLocalTime(), await TransformerRules.ToLocalTime<DateTimeOffset>().TransformAsync(SampleOffset));

        var shifted = await TransformerRules.ToOffset(TimeSpan.FromHours(3)).TransformAsync(SampleOffset);
        Assert.Equal(SampleOffset.UtcDateTime, shifted.UtcDateTime);
        Assert.Equal(TimeSpan.FromHours(3), shifted.Offset);
        Assert.Equal(TimeZoneInfo.ConvertTime(SampleOffset, TimeZoneInfo.Utc), await TransformerRules.ConvertToTimeZone(TimeZoneInfo.Utc).TransformAsync(SampleOffset));
        Assert.Throws<ArgumentException>(() => TransformerRules.ToOffset(TimeSpan.FromSeconds(30)));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ConvertToTimeZone(null!));
    }

    [Fact]
    public async Task A_cancelled_token_stops_a_date_rule_before_it_returns()
    {
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        Assert.Throws<OperationCanceledException>(() => TransformerRules.AddDays<DateOnly>(1).TransformAsync(SampleDate, cancelled.Token));
    }
}
