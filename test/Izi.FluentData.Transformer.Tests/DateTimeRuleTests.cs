using static Izi.FluentData.Transformer.Rules.TransformerRules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the date/time rules across the supported flowing types (<see cref="DateTime"/>, <see cref="DateTimeOffset"/>,
/// <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/>): arithmetic, clamping, start/end of
/// calendar periods, interval rounding, component replacement, calendar navigation, kind/zone conversions, and the
/// sentinel defaults — plus the <see cref="NotSupportedException"/> raised when a rule meets a type it does not cover.
/// </summary>
public class DateTimeRuleTests
{
    // ---- Arithmetic / offset ----
    [Fact]
    public async Task AddDays_on_DateTime()
        => Assert.Equal(new DateTime(2024, 1, 16), await AddDays<DateTime>(1).TransformAsync(new DateTime(2024, 1, 15)));

    [Fact]
    public async Task AddMonths_on_DateOnly_clamps_to_end_of_month()
        => Assert.Equal(new DateOnly(2024, 2, 29), await AddMonths<DateOnly>(1).TransformAsync(new DateOnly(2024, 1, 31)));

    [Fact]
    public async Task AddHours_wraps_TimeOnly_past_midnight()
        => Assert.Equal(new TimeOnly(1, 0), await AddHours<TimeOnly>(2).TransformAsync(new TimeOnly(23, 0)));

    [Fact]
    public async Task Add_TimeSpan_to_TimeSpan()
        => Assert.Equal(TimeSpan.FromHours(3), await Add<TimeSpan>(TimeSpan.FromHours(1)).TransformAsync(TimeSpan.FromHours(2)));

    [Fact]
    public async Task Negate_flips_TimeSpan_sign()
        => Assert.Equal(TimeSpan.FromHours(-2), await Negate().TransformAsync(TimeSpan.FromHours(2)));

    [Fact]
    public async Task Duration_takes_absolute_TimeSpan()
        => Assert.Equal(TimeSpan.FromHours(2), await Duration().TransformAsync(TimeSpan.FromHours(-2)));

    // ---- Clamp ----
    [Fact]
    public async Task Clamp_DateTime_below_min()
    {
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 12, 31);
        Assert.Equal(min, await Clamp(min, max).TransformAsync(new DateTime(2023, 6, 1)));
    }

    // ---- Start / end of period ----
    [Fact]
    public async Task StartOfMonth_on_DateTime()
        => Assert.Equal(new DateTime(2024, 3, 1), await StartOfMonth<DateTime>().TransformAsync(new DateTime(2024, 3, 15, 10, 30, 0)));

    [Fact]
    public async Task EndOfMonth_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 2, 29), await EndOfMonth<DateOnly>().TransformAsync(new DateOnly(2024, 2, 10)));

    [Fact]
    public async Task EndOfMonth_on_DateTime_is_last_instant()
    {
        var result = await EndOfMonth<DateTime>().TransformAsync(new DateTime(2024, 2, 10, 8, 0, 0));
        Assert.Equal(new DateTime(2024, 3, 1).AddTicks(-1), result);
    }

    [Fact]
    public async Task StartOfWeek_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 1, 15), await StartOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(new DateOnly(2024, 1, 17)));

    [Fact]
    public async Task EndOfWeek_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 1, 21), await EndOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(new DateOnly(2024, 1, 17)));

    [Fact]
    public async Task StartOfQuarter_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 4, 1), await StartOfQuarter<DateOnly>().TransformAsync(new DateOnly(2024, 5, 10)));

    [Fact]
    public async Task EndOfQuarter_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 6, 30), await EndOfQuarter<DateOnly>().TransformAsync(new DateOnly(2024, 5, 10)));

    [Fact]
    public async Task StartOfYear_on_DateOnly()
        => Assert.Equal(new DateOnly(2024, 1, 1), await StartOfYear<DateOnly>().TransformAsync(new DateOnly(2024, 7, 4)));

    // ---- Interval rounding ----
    [Fact]
    public async Task RoundTo_nearest_quarter_hour_rounds_up_on_halfway()
        => Assert.Equal(TimeSpan.FromMinutes(15), await RoundTo<TimeSpan>(TimeSpan.FromMinutes(15)).TransformAsync(TimeSpan.FromMinutes(8)));

    [Fact]
    public async Task TruncateTo_quarter_hour_floors()
        => Assert.Equal(TimeSpan.Zero, await TruncateTo<TimeSpan>(TimeSpan.FromMinutes(15)).TransformAsync(TimeSpan.FromMinutes(8)));

    [Fact]
    public async Task CeilingTo_quarter_hour_snaps_up()
        => Assert.Equal(TimeSpan.FromMinutes(15), await CeilingTo<TimeSpan>(TimeSpan.FromMinutes(15)).TransformAsync(TimeSpan.FromMinutes(8)));

    [Fact]
    public async Task RoundTo_rejects_non_positive_interval()
        => Assert.Throws<ArgumentOutOfRangeException>(() => RoundTo<TimeSpan>(TimeSpan.Zero));

    // ---- Component replacement ----
    [Fact]
    public async Task WithYear_on_DateOnly()
        => Assert.Equal(new DateOnly(2030, 3, 15), await WithYear<DateOnly>(2030).TransformAsync(new DateOnly(2024, 3, 15)));

    [Fact]
    public async Task WithHour_on_DateTime_keeps_the_date()
        => Assert.Equal(new DateTime(2024, 3, 15, 9, 30, 0), await WithHour<DateTime>(9).TransformAsync(new DateTime(2024, 3, 15, 23, 30, 0)));

    [Fact]
    public void WithHour_rejects_out_of_range()
        => Assert.Throws<ArgumentOutOfRangeException>(() => WithHour<DateTime>(24));

    // ---- Calendar navigation ----
    [Fact]
    public async Task NextDayOfWeek_moves_strictly_forward()
        => Assert.Equal(new DateOnly(2024, 1, 22), await NextDayOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(new DateOnly(2024, 1, 15)));

    [Fact]
    public async Task AddBusinessDays_skips_the_weekend()
        => Assert.Equal(new DateOnly(2024, 1, 22), await AddBusinessDays<DateOnly>(1).TransformAsync(new DateOnly(2024, 1, 19)));

    [Fact]
    public async Task AddBusinessDays_skips_holidays()
    {
        var holidays = new[] { new DateOnly(2024, 1, 22) };
        Assert.Equal(new DateOnly(2024, 1, 23), await AddBusinessDays<DateOnly>(1, holidays).TransformAsync(new DateOnly(2024, 1, 19)));
    }

    // ---- Kind / time zone ----
    [Fact]
    public async Task SpecifyKind_stamps_without_shifting()
    {
        var result = await SpecifyKind(DateTimeKind.Utc).TransformAsync(new DateTime(2024, 1, 1, 12, 0, 0));
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(12, result.Hour);
    }

    [Fact]
    public async Task ToOffset_preserves_the_instant()
    {
        var source = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var result = await ToOffset(TimeSpan.FromHours(2)).TransformAsync(source);
        Assert.Equal(source.UtcDateTime, result.UtcDateTime);
        Assert.Equal(TimeSpan.FromHours(2), result.Offset);
    }

    // ---- Sentinel defaults ----
    [Fact]
    public async Task DefaultIfMinValue_replaces_DateTime_sentinel()
    {
        var fallback = new DateTime(2024, 1, 1);
        Assert.Equal(fallback, await DefaultIfMinValue(fallback).TransformAsync(DateTime.MinValue));
    }

    // ---- Unsupported type ----
    [Fact]
    public async Task Unsupported_type_throws()
        => await Assert.ThrowsAsync<NotSupportedException>(
            async () => await AddYears<TimeSpan>(1).TransformAsync(TimeSpan.FromDays(1)));
}
