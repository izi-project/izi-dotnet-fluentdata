using System.Globalization;

namespace Izi.FluentData.Transformer.Tests;

public class DateTimeConversionTests
{
    [Fact]
    public async Task Date_and_time_parts_use_the_displayed_clock()
    {
        var localClock = new DateTime(2020, 1, 2, 0, 30, 0, DateTimeKind.Utc);
        Assert.Equal(
            new DateOnly(2020, 1, 2),
            await TransformerRules.ToDateOnlyFromDateTime().TransformAsync(localClock));
        Assert.Equal(new TimeOnly(0, 30), await TransformerRules.ToTimeOnlyFromDateTime().TransformAsync(localClock));

        var offset = new DateTimeOffset(2020, 1, 2, 0, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(offset.UtcDateTime));
        Assert.Equal(
            new DateOnly(2020, 1, 2),
            await TransformerRules.ToDateOnlyFromDateTimeOffset().TransformAsync(offset));
        Assert.Equal(new TimeOnly(0, 30), await TransformerRules.ToTimeOnlyFromDateTimeOffset().TransformAsync(offset));
    }

    [Fact]
    public async Task Date_and_time_can_be_reassembled_with_an_explicit_kind()
    {
        var fromDate = await TransformerRules.ToDateTime(new TimeOnly(3, 4, 5), DateTimeKind.Utc)
            .TransformAsync(new DateOnly(2020, 1, 2));
        Assert.Equal(new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc), fromDate);

        var fromTime = await TransformerRules.ToDateTime(new DateOnly(2020, 1, 2), DateTimeKind.Unspecified)
            .TransformAsync(new TimeOnly(3, 4));
        Assert.Equal(new DateTime(2020, 1, 2, 3, 4, 0, DateTimeKind.Unspecified), fromTime);
    }

    [Fact]
    public async Task TimeOnly_and_TimeSpan_convert_inside_a_single_day()
    {
        var time = new TimeOnly(1, 2, 3, 4);
        Assert.Equal(time.ToTimeSpan(), await TransformerRules.ToTimeSpan().TransformAsync(time));

        var almostADay = TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1);
        Assert.Equal(
            almostADay,
            (await TransformerRules.ToTimeOnlyFromTimeSpan().TransformAsync(almostADay)).ToTimeSpan());
        Assert.Equal(
            TimeSpan.Zero,
            (await TransformerRules.ToTimeOnlyFromTimeSpan().TransformAsync(TimeSpan.Zero)).ToTimeSpan());
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TransformerRules.ToTimeOnlyFromTimeSpan().TransformAsync(TimeSpan.FromDays(1)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TransformerRules.ToTimeOnlyFromTimeSpan().TransformAsync(TimeSpan.FromTicks(-1)));
    }

    [Fact]
    public async Task ToDateTimeOffset_honours_kind_and_rejects_a_bad_offset()
    {
        Assert.Throws<ArgumentException>(() => TransformerRules.ToDateTimeOffset(TimeSpan.FromSeconds(30)));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.ToDateTimeOffset(TimeSpan.FromHours(15)));

        var utc = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        Assert.Throws<ArgumentException>(() =>
            TransformerRules.ToDateTimeOffset(TimeSpan.FromHours(1)).TransformAsync(utc));
        var utcOffset = await TransformerRules.ToDateTimeOffset(TimeSpan.Zero).TransformAsync(utc);
        Assert.Equal(TimeSpan.Zero, utcOffset.Offset);
        Assert.Equal(utc, utcOffset.UtcDateTime);

        var unspecified = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
        var stamped = await TransformerRules.ToDateTimeOffset(TimeSpan.FromHours(2)).TransformAsync(unspecified);
        Assert.Equal(TimeSpan.FromHours(2), stamped.Offset);
        Assert.Equal(unspecified, stamped.DateTime);

        var local = new DateTime(2020, 6, 15, 12, 0, 0, DateTimeKind.Local);
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(local);
        var localResult = await TransformerRules.ToDateTimeOffset(localOffset).TransformAsync(local);
        Assert.Equal(localOffset, localResult.Offset);
        Assert.Equal(local, localResult.DateTime);
    }

    [Fact]
    public async Task Unix_time_round_trips_as_utc_and_rejects_an_out_of_range_value()
    {
        var instant = DateTimeOffset.FromUnixTimeSeconds(1_700_000_000);
        Assert.Equal(1_700_000_000L, await TransformerRules.ToUnixTimeSeconds().TransformAsync(instant));
        Assert.Equal(instant, await TransformerRules.FromUnixTimeSeconds().TransformAsync(1_700_000_000L));

        var pipeline = new Transformer<DateTimeOffset, long>(TransformerRules.ToUnixTimeMilliseconds())
            .Then(TransformerRules.FromUnixTimeMilliseconds());
        var milliseconds = DateTimeOffset.FromUnixTimeMilliseconds(-1);
        Assert.Equal(milliseconds, await pipeline.TransformAsync(milliseconds));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TransformerRules.FromUnixTimeSeconds().TransformAsync(long.MaxValue));
    }

    [Fact]
    public async Task An_exact_date_parse_can_feed_a_date_only_conversion()
    {
        var pipeline = new Transformer<string, DateTime>(
                TransformerRules.ParseExactDateTime("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture))
            .Then(TransformerRules.ToDateOnlyFromDateTime());

        Assert.Equal(new DateOnly(2020, 1, 2), await pipeline.TransformAsync("2020-01-02 03:04"));
    }
}
