using System.Globalization;

namespace Izi.FluentData.Transformer.Tests;

public class ParseConversionTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    [Fact]
    public async Task Parse_reads_numbers_bool_guid_and_dates_with_the_supplied_culture()
    {
        Assert.Equal(42, await TransformerRules.Parse<int>(Invariant).TransformAsync("42"));
        Assert.Equal(1.5d, await TransformerRules.Parse<double>(German).TransformAsync("1,5"));
        Assert.Equal(15d, await TransformerRules.Parse<double>(Invariant).TransformAsync("1,5"));
        Assert.Equal(1.5d, await TransformerRules.Parse<double>(Invariant).TransformAsync("1.5"));
        Assert.True(await TransformerRules.Parse<bool>(Invariant).TransformAsync("True"));
        Assert.False(await TransformerRules.Parse<bool>(Invariant).TransformAsync("false"));

        var id = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
        Assert.Equal(id, await TransformerRules.Parse<Guid>(Invariant).TransformAsync("00112233-4455-6677-8899-aabbccddeeff"));
        Assert.Equal(new DateTime(2020, 1, 2), await TransformerRules.Parse<DateTime>(Invariant).TransformAsync("2020-01-02"));
    }

    [Fact]
    public void Parse_rejects_a_missing_provider_null_text_and_invalid_text()
    {
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Parse<int>(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Parse<int>(Invariant).TransformAsync(null!));
        Assert.Throws<FormatException>(() => TransformerRules.Parse<double>(Invariant).TransformAsync("abc"));
        Assert.Throws<OverflowException>(() => TransformerRules.Parse<int>(Invariant).TransformAsync("999999999999"));
    }

    [Fact]
    public async Task ParseExact_accepts_one_format_or_the_copied_format_list()
    {
        Assert.Equal(
            new DateTime(2020, 2, 29, 3, 4, 0, DateTimeKind.Unspecified),
            await TransformerRules.ParseExactDateTime("yyyy-MM-dd HH:mm", Invariant).TransformAsync("2020-02-29 03:04"));
        Assert.Equal(
            new DateTimeOffset(2020, 1, 2, 3, 4, 0, TimeSpan.FromHours(2)),
            await TransformerRules.ParseExactDateTimeOffset("yyyy-MM-dd HH:mm zzz", Invariant).TransformAsync("2020-01-02 03:04 +02:00"));
        Assert.Equal(new TimeOnly(23, 59, 59), await TransformerRules.ParseExactTimeOnly("HH:mm:ss", Invariant).TransformAsync("23:59:59"));
        Assert.Equal(
            new TimeSpan(1, 2, 3),
            await TransformerRules.ParseExactTimeSpan(@"hh\:mm\:ss", Invariant).TransformAsync("01:02:03"));

        var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };
        var date = TransformerRules.ParseExactDateOnly(formats, Invariant);
        formats[0] = "changed";
        Assert.Equal(new DateOnly(2020, 3, 4), await date.TransformAsync("04/03/2020"));
        Assert.Equal(new DateOnly(2020, 3, 4), await date.TransformAsync("2020-03-04"));
    }

    [Fact]
    public void ParseExact_rejects_bad_configuration_and_bad_values()
    {
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ParseExactDateTime("yyyy", null!));
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseExactDateOnly("", Invariant));
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseExactTimeSpan([], Invariant));
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseExactDateOnly([null!], Invariant));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ParseExactDateTime("yyyy-MM-dd", Invariant).TransformAsync(null!));
        Assert.Throws<FormatException>(() => TransformerRules.ParseExactDateOnly("yyyy-MM-dd", Invariant).TransformAsync("2019-02-29"));
        Assert.Throws<FormatException>(() => TransformerRules.ParseExactTimeOnly("HH:mm", Invariant).TransformAsync("24:00"));
    }

    [Fact]
    public async Task ParseEnum_uses_bcl_names_flags_and_undefined_numbers()
    {
        Assert.Equal(Access.Read, await TransformerRules.ParseEnum<Access>().TransformAsync("Read"));
        Assert.Equal(Access.Read | Access.Write, await TransformerRules.ParseEnum<Access>().TransformAsync("Read, Write"));
        Assert.Equal((Access)8, await TransformerRules.ParseEnum<Access>().TransformAsync("8"));
        Assert.False(Enum.IsDefined((Access)8));
        Assert.Equal(Access.Read, await TransformerRules.ParseEnum<Access>(ignoreCase: true).TransformAsync("read"));
    }

    [Fact]
    public void ParseEnum_rejects_unknown_names_wrong_case_and_null()
    {
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseEnum<Access>().TransformAsync("read"));
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseEnum<Access>().TransformAsync("Nope"));
        Assert.Throws<ArgumentException>(() => TransformerRules.ParseEnum<Access>().TransformAsync(""));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ParseEnum<Access>().TransformAsync(null!));
    }

    [Fact]
    public async Task ParseOrDefault_and_TryParse_use_try_parse_for_null_and_invalid_text()
    {
        var orDefault = TransformerRules.ParseOrDefault(7, Invariant);
        Assert.Equal(3, await orDefault.TransformAsync("3"));
        Assert.Equal(7, await orDefault.TransformAsync("nope"));
        Assert.Equal(7, await orDefault.TransformAsync(null!));
        Assert.Equal(7, await orDefault.TransformAsync("999999999999"));

        var tryParse = TransformerRules.TryParse<int>(Invariant);
        Assert.Equal(ParseResult<int>.Succeeded(0), await tryParse.TransformAsync("0"));
        Assert.Equal(ParseResult<int>.Failed(), await tryParse.TransformAsync("nope"));
        Assert.Equal(ParseResult<int>.Failed(), await tryParse.TransformAsync(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.TryParse<int>(null!));
    }

    [Flags]
    public enum Access
    {
        None = 0,
        Read = 1,
        Write = 2,
    }
}
