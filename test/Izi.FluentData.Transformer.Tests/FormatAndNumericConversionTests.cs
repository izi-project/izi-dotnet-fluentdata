using System.Globalization;

namespace Izi.FluentData.Transformer.Tests;

public class FormatAndNumericConversionTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    [Fact]
    public async Task Format_uses_the_format_and_provider_and_allocates_the_text()
    {
        Assert.Equal("0042", await TransformerRules.Format<int>("D4", Invariant).TransformAsync(42));
        Assert.Equal("42", await TransformerRules.Format<int>(null, Invariant).TransformAsync(42));
        Assert.Equal("1.5", await TransformerRules.Format<double>("0.0", Invariant).TransformAsync(1.5d));
        Assert.Equal("1,5", await TransformerRules.Format<double>("0.0", German).TransformAsync(1.5d));
    }

    [Fact]
    public void Format_rejects_an_empty_format_and_a_missing_provider()
    {
        Assert.Throws<ArgumentException>(() => TransformerRules.Format<int>("", Invariant));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Format<int>("D", null!));
    }

    [Fact]
    public async Task EnumName_renders_names_flags_and_unnamed_numbers()
    {
        var names = TransformerRules.EnumName<ParseConversionTests.Access>();
        Assert.Equal("Read", await names.TransformAsync(ParseConversionTests.Access.Read));
        Assert.Equal("Read, Write", await names.TransformAsync(ParseConversionTests.Access.Read | ParseConversionTests.Access.Write));
        Assert.Equal("8", await names.TransformAsync((ParseConversionTests.Access)8));
        Assert.Equal("None", await names.TransformAsync(ParseConversionTests.Access.None));
    }

    [Fact]
    public async Task Checked_saturating_and_truncating_conversions_follow_the_bcl()
    {
        Assert.Equal((byte)44, await TransformerRules.ConvertChecked<int, byte>().TransformAsync(44));
        Assert.Equal(1, await TransformerRules.ConvertChecked<double, int>().TransformAsync(1.9d));
        Assert.Equal(1, await TransformerRules.ConvertChecked<decimal, int>().TransformAsync(1.5m));
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<int, byte>().TransformAsync(300));
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<int, byte>().TransformAsync(-1));
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<double, int>().TransformAsync(double.PositiveInfinity));
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<double, int>().TransformAsync(double.NaN));

        Assert.Equal(byte.MaxValue, await TransformerRules.ConvertSaturating<int, byte>().TransformAsync(300));
        Assert.Equal(byte.MinValue, await TransformerRules.ConvertSaturating<int, byte>().TransformAsync(-1));
        Assert.Equal(int.MaxValue, await TransformerRules.ConvertSaturating<long, int>().TransformAsync(4_000_000_000L));
        Assert.Equal(int.MinValue, await TransformerRules.ConvertSaturating<long, int>().TransformAsync(long.MinValue));
        Assert.Equal(1, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(1.9d));
        Assert.Equal(-1, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(-1.9d));

        Assert.Equal((byte)44, await TransformerRules.ConvertTruncating<int, byte>().TransformAsync(300));
        Assert.Equal((byte)255, await TransformerRules.ConvertTruncating<int, byte>().TransformAsync(-1));
        Assert.Equal(1, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(1.9d));
        Assert.Equal(-1, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(-1.9d));
        Assert.Equal(-294_967_296, await TransformerRules.ConvertTruncating<long, int>().TransformAsync(4_000_000_000L));
        Assert.Equal((short)-25_536, await TransformerRules.ConvertTruncating<int, short>().TransformAsync(40_000));
    }

    [Fact]
    public async Task A_parse_then_checked_conversion_chain_returns_the_narrowed_value()
    {
        var pipeline = new Transformer<string, int>(TransformerRules.Parse<int>(Invariant))
            .Then(TransformerRules.ConvertChecked<int, short>());

        Assert.Equal((short)42, await pipeline.TransformAsync("42"));
    }

    [Fact]
    public async Task ConvertChecked_overflows_where_Saturating_clamps()
    {
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<int, byte>().TransformAsync(300));
        Assert.Equal(byte.MaxValue, await TransformerRules.ConvertSaturating<int, byte>().TransformAsync(300));
        Assert.Equal((byte)44, await TransformerRules.ConvertTruncating<int, byte>().TransformAsync(300));

        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<int, byte>().TransformAsync(-1));
        Assert.Equal(byte.MinValue, await TransformerRules.ConvertSaturating<int, byte>().TransformAsync(-1));
        Assert.Equal(byte.MaxValue, await TransformerRules.ConvertTruncating<int, byte>().TransformAsync(-1));

        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<double, int>().TransformAsync(double.PositiveInfinity));
        Assert.Equal(int.MaxValue, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(double.PositiveInfinity));
        Assert.Equal(int.MaxValue, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(double.PositiveInfinity));
        Assert.Equal(int.MinValue, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(double.NegativeInfinity));
        Assert.Equal(int.MinValue, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(double.NegativeInfinity));

        // Checked rejects NaN on both runtimes. Saturating and truncating follow the BCL, which disagrees:
        // .NET 8 yields int.MinValue and .NET 10 yields 0.
        Assert.Throws<OverflowException>(() => TransformerRules.ConvertChecked<double, int>().TransformAsync(double.NaN));
#if NET8_0
        Assert.Equal(int.MinValue, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(double.NaN));
        Assert.Equal(int.MinValue, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(double.NaN));
#elif NET10_0
        Assert.Equal(0, await TransformerRules.ConvertSaturating<double, int>().TransformAsync(double.NaN));
        Assert.Equal(0, await TransformerRules.ConvertTruncating<double, int>().TransformAsync(double.NaN));
#else
#error This suite targets net8.0 and net10.0 only.
#endif
    }
}
