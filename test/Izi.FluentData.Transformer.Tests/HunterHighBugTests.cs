namespace Izi.FluentData.Transformer.Tests;

public class HunterHighBugTests
{
    [Fact]
    public async Task RoundTo_saturates_instead_of_overflowing()
    {
        var fiveExa = TimeSpan.FromTicks(5_000_000_000_000_000_000);
        var nineExa = TimeSpan.FromTicks(9_000_000_000_000_000_000);

        Assert.Equal(
            TimeSpan.MaxValue,
            await TransformerRules.RoundTo<TimeSpan>(TimeSpan.MaxValue).TransformAsync(fiveExa));
        Assert.Equal(
            TimeSpan.MaxValue,
            await TransformerRules.RoundTo<TimeSpan>(fiveExa).TransformAsync(nineExa));

        var lastAfternoon = new DateTime(9999, 12, 31, 18, 0, 0, DateTimeKind.Utc);
        Assert.Equal(
            DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc),
            await TransformerRules.RoundTo<DateTime>(TimeSpan.FromDays(1)).TransformAsync(lastAfternoon));
        Assert.Equal(
            DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc),
            await TransformerRules.CeilingTo<DateTime>(TimeSpan.FromDays(1)).TransformAsync(lastAfternoon));
    }

    [Fact]
    public async Task Period_ends_at_the_maximum_stay_inside_the_calendar()
    {
        var lastUtc = new DateTime(9999, 12, 31, 18, 0, 0, DateTimeKind.Utc);
        var maxUtc = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
        Assert.Equal(maxUtc, await TransformerRules.EndOfDay<DateTime>().TransformAsync(lastUtc));
        Assert.Equal(maxUtc, await TransformerRules.EndOfWeek<DateTime>(DayOfWeek.Monday).TransformAsync(lastUtc));
        Assert.Equal(maxUtc, await TransformerRules.EndOfMonth<DateTime>().TransformAsync(lastUtc));
        Assert.Equal(maxUtc, await TransformerRules.EndOfQuarter<DateTime>().TransformAsync(lastUtc));
        Assert.Equal(maxUtc, await TransformerRules.EndOfYear<DateTime>().TransformAsync(lastUtc));
        Assert.Equal(maxUtc, await TransformerRules.NextDayOfWeek<DateTime>(DayOfWeek.Friday).TransformAsync(lastUtc));

        var offset = TimeSpan.FromHours(2);
        var lastOffset = new DateTimeOffset(9999, 12, 31, 18, 0, 0, offset);
        Assert.Equal(offset, (await TransformerRules.EndOfDay<DateTimeOffset>().TransformAsync(lastOffset)).Offset);
        Assert.Equal(offset, (await TransformerRules.EndOfWeek<DateTimeOffset>(DayOfWeek.Monday).TransformAsync(lastOffset)).Offset);
        Assert.Equal(offset, (await TransformerRules.EndOfMonth<DateTimeOffset>().TransformAsync(lastOffset)).Offset);
        Assert.Equal(offset, (await TransformerRules.EndOfQuarter<DateTimeOffset>().TransformAsync(lastOffset)).Offset);
        Assert.Equal(offset, (await TransformerRules.EndOfYear<DateTimeOffset>().TransformAsync(lastOffset)).Offset);
        var next = await TransformerRules.NextDayOfWeek<DateTimeOffset>(DayOfWeek.Friday).TransformAsync(lastOffset);
        Assert.Equal(offset, next.Offset);

        Assert.Equal(
            DateOnly.MaxValue,
            await TransformerRules.EndOfWeek<DateOnly>(DayOfWeek.Monday).TransformAsync(DateOnly.MaxValue));
        Assert.Equal(
            DateOnly.MaxValue,
            await TransformerRules.NextDayOfWeek<DateOnly>(DayOfWeek.Friday).TransformAsync(DateOnly.MaxValue));
    }

    [Fact]
    public void With_time_components_reject_unsupported_types_before_the_range()
    {
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithHour<TimeSpan>(99));
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithHour<string>(99));
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithHour<DateOnly>(99));
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithMinute<TimeSpan>(99));
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithSecond<string>(99));
        Assert.Throws<NotSupportedException>(() => TransformerRules.WithMillisecond<DateOnly>(99));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.WithHour<DateTime>(99));
    }

    [Fact]
    public void Replace_rejects_an_empty_old_value_when_the_rule_is_created()
    {
        var error = Assert.Throws<ArgumentException>(() => TransformerRules.Replace(string.Empty, "x"));
        Assert.Equal("oldValue", error.ParamName);
    }

    [Fact]
    public async Task A_record_with_three_init_properties_is_cloned_once()
    {
        var transformer = new Transformer<Trio>();
        transformer.RuleFor(item => item.A).Add(1);
        transformer.RuleFor(item => item.B).Add(1);
        transformer.RuleFor(item => item.C).Add(1);

        var warmup = new Trio(1, 2, 3);
        _ = await transformer.TransformAsync(warmup);

        var original = new Trio(4, 5, 6);
        var before = GC.GetAllocatedBytesForCurrentThread();
        var updated = await transformer.TransformAsync(original);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(4, original.A);
        Assert.Equal(5, original.B);
        Assert.Equal(6, original.C);
        Assert.Equal(5, updated.A);
        Assert.Equal(6, updated.B);
        Assert.Equal(7, updated.C);
        Assert.NotSame(original, updated);

        long oneClone = 0;
        for (var i = 0; i < 8; i++)
        {
            var sample = new Trio(1, 2, 3);
            var start = GC.GetAllocatedBytesForCurrentThread();
            _ = sample with { A = sample.A };
            var used = GC.GetAllocatedBytesForCurrentThread() - start;
            if (used > 0)
            {
                oneClone = used;
                break;
            }
        }

        Assert.True(oneClone > 0);
        Assert.Equal(oneClone, allocated);
    }

    public sealed record Trio(int A, int B, int C);
}
