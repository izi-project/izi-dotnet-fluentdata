using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation.Tests;

public class RuleTests
{
    [Fact]
    public async Task NotNull_and_Null_use_the_default_messages()
    {
        await Fail(ValidatorRules.NotNull<string?>(), null, "Must not be null.").ConfigureAwait(true);
        await Pass(ValidatorRules.NotNull<string?>(), "x").ConfigureAwait(true);
        await Pass(ValidatorRules.Null<string?>(), null).ConfigureAwait(true);
        await Fail(ValidatorRules.Null<string?>(), "x", "Must be null.").ConfigureAwait(true);
        await Fail(ValidatorRules.NotNull<string?>("Name is required."), null, "Name is required.").ConfigureAwait(true);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task NotEmpty_fails_blank_strings(string? value)
        => await Fail(ValidatorRules.NotEmpty<string?>(), value, "Must not be empty.").ConfigureAwait(true);

    [Fact]
    public async Task NotEmpty_and_Empty_handle_text_and_collections()
    {
        await Pass(ValidatorRules.NotEmpty<string>(), "x").ConfigureAwait(true);
        await Fail(ValidatorRules.NotEmpty<List<int>>(), [], "Must not be empty.").ConfigureAwait(true);
        await Pass(ValidatorRules.NotEmpty<List<int>>(), [1]).ConfigureAwait(true);
        await Pass(ValidatorRules.Empty<string?>(), null).ConfigureAwait(true);
        await Pass(ValidatorRules.Empty<string>(), "  ").ConfigureAwait(true);
        await Fail(ValidatorRules.Empty<string>(), "x", "Must be empty.").ConfigureAwait(true);
        await Pass(ValidatorRules.Empty<List<int>>(), []).ConfigureAwait(true);
        await Fail(ValidatorRules.Empty<int>(), 1, "Must be empty.").ConfigureAwait(true);
    }

    [Fact]
    public async Task Equal_and_NotEqual_embed_the_bound()
    {
        await Pass(ValidatorRules.Equal<int>(5), 5).ConfigureAwait(true);
        await Fail(ValidatorRules.Equal<int>(5), 4, "Must be equal to 5.").ConfigureAwait(true);
        await Fail(ValidatorRules.NotEqual<int>(5), 5, "Must not be equal to 5.").ConfigureAwait(true);
        await Pass(ValidatorRules.NotEqual<int>(5), 4).ConfigureAwait(true);
        await Fail(ValidatorRules.Equal<string>("a", "same"), "b", "same").ConfigureAwait(true);
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(10, false)]
    [InlineData(11, false)]
    public async Task LessThan(int value, bool valid)
        => await Expect(ValidatorRules.LessThan<int>(10), value, valid, "Must be less than 10.").ConfigureAwait(true);

    [Theory]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public async Task LessThanOrEqual(int value, bool valid)
        => await Expect(ValidatorRules.LessThanOrEqual<int>(10), value, valid, "Must be less than or equal to 10.").ConfigureAwait(true);

    [Theory]
    [InlineData(11, true)]
    [InlineData(10, false)]
    public async Task GreaterThan(int value, bool valid)
        => await Expect(ValidatorRules.GreaterThan<int>(10), value, valid, "Must be greater than 10.").ConfigureAwait(true);

    [Theory]
    [InlineData(10, true)]
    [InlineData(9, false)]
    public async Task GreaterThanOrEqual(int value, bool valid)
        => await Expect(ValidatorRules.GreaterThanOrEqual<int>(10), value, valid, "Must be greater than or equal to 10.").ConfigureAwait(true);

    [Fact]
    public async Task A_null_comparable_fails_ordering()
    {
        var ordering = ValidatorRules.LessThan<string>("m");
        var missing = await ordering.ValidateAsync(null!).ConfigureAwait(true);
        Assert.False(missing.IsValid);
        Assert.Equal("Must be less than m.", Assert.Single(missing.Errors));
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("ab", false)]
    [InlineData(null, false)]
    public async Task Length_is_exact(string? value, bool valid)
        => await Expect(ValidatorRules.Length<string?>(3), value, valid, "Must be 3 characters.").ConfigureAwait(true);

    [Fact]
    public async Task MinLength_and_MaxLength_count_strings_and_collections()
    {
        await Pass(ValidatorRules.MinLength<string>(3), "abc").ConfigureAwait(true);
        await Fail(ValidatorRules.MinLength<string>(3), "ab", "Must be at least 3 characters.").ConfigureAwait(true);
        await Pass(ValidatorRules.MaxLength<string>(3), "abc").ConfigureAwait(true);
        await Fail(ValidatorRules.MaxLength<string>(3), "abcd", "Must be at most 3 characters.").ConfigureAwait(true);
        await Pass(ValidatorRules.Length<List<int>>(2), [4, 5]).ConfigureAwait(true);
        await Fail(ValidatorRules.Length<int>(1), 1, "Must be 1 characters.").ConfigureAwait(true);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(0, false)]
    [InlineData(11, false)]
    public async Task Range_is_inclusive(int value, bool valid)
        => await Expect(ValidatorRules.Range<int>(1, 10), value, valid, "Must be between 1 and 10.").ConfigureAwait(true);

    [Theory]
    [InlineData(5, false)]
    [InlineData(0, true)]
    [InlineData(11, true)]
    public async Task NotRange_rejects_the_inclusive_interval(int value, bool valid)
        => await Expect(ValidatorRules.NotRange<int>(1, 10), value, valid, "Must not be between 1 and 10.").ConfigureAwait(true);

    [Fact]
    public async Task ScalePrecision_uses_scale_and_integer_digits()
    {
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 3), 123.45m).ConfigureAwait(true);
        await Fail(
            ValidatorRules.ScalePrecision<decimal>(1, 3),
            123.45m,
            "Must have a scale of at most 1 and a precision of at most 3.").ConfigureAwait(true);
        await Fail(
            ValidatorRules.ScalePrecision<decimal>(2, 2),
            123.45m,
            "Must have a scale of at most 2 and a precision of at most 2.").ConfigureAwait(true);
        await Fail(ValidatorRules.ScalePrecision<int>(2, 4), 10, "Must have a scale of at most 2 and a precision of at most 4.").ConfigureAwait(true);
    }

    [Fact]
    public async Task Matches_and_NotMatches_use_fixed_messages()
    {
        await Pass(ValidatorRules.Matches<string>(@"^\d+$"), "123").ConfigureAwait(true);
        await Fail(ValidatorRules.Matches<string>(@"^\d+$"), "12a", "Must match the required pattern.").ConfigureAwait(true);
        await Pass(ValidatorRules.NotMatches<string>(@"^\d+$"), "12a").ConfigureAwait(true);
        await Fail(ValidatorRules.NotMatches<string>(@"^\d+$"), "123", "Must not match the pattern.").ConfigureAwait(true);
        await Fail(ValidatorRules.Matches<string?>(@"^\d+$"), null, "Must match the required pattern.").ConfigureAwait(true);
        await Pass(ValidatorRules.NotMatches<string?>(@"^\d+$"), null).ConfigureAwait(true);
        var timed = new Regex(@"^\d+$", RegexOptions.None, TimeSpan.FromSeconds(1));
        await Pass(ValidatorRules.Matches<string>(timed, "digits"), "123").ConfigureAwait(true);
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("a@b.co", true)]
    [InlineData("invalid", false)]
    [InlineData("no@dot", false)]
    public async Task Email(string value, bool valid)
        => await Expect(ValidatorRules.Email<string>(), value, valid, "Must be a valid email address.").ConfigureAwait(true);

    [Theory]
    [InlineData("4111111111111111", true)]
    [InlineData("1234", false)]
    public async Task CreditCard(string value, bool valid)
        => await Expect(ValidatorRules.CreditCard<string>(), value, valid, "Must be a valid credit card number.").ConfigureAwait(true);

    [Fact]
    public async Task Iso_codes_accept_known_values_and_ignore_alpha_case()
    {
        await Pass(ValidatorRules.CountryIso2<string>(), "us").ConfigureAwait(true);
        await Fail(ValidatorRules.CountryIso2<string>(), "ZZ", "Must be a valid ISO 3166-1 alpha-2 country code.").ConfigureAwait(true);
        await Pass(ValidatorRules.CountryIso3<string>(), "usa").ConfigureAwait(true);
        await Fail(ValidatorRules.CountryIso3<string>(), "ZZZ", "Must be a valid ISO 3166-1 alpha-3 country code.").ConfigureAwait(true);
        await Pass(ValidatorRules.CountryIsoNumeric<string>(), "840").ConfigureAwait(true);
        await Fail(ValidatorRules.CountryIsoNumeric<string>(), "84", "Must be a valid ISO 3166-1 numeric country code.").ConfigureAwait(true);
        await Pass(ValidatorRules.CurrencyIso<string>(), "usd").ConfigureAwait(true);
        await Fail(ValidatorRules.CurrencyIso<string>(), "ZZZ", "Must be a valid ISO 4217 currency code.").ConfigureAwait(true);
    }

    [Fact]
    public async Task Must_and_MustAsync_use_the_supplied_message()
    {
        await Fail(ValidatorRules.Must<int>(static value => value > 0, "Must be positive."), 0, "Must be positive.").ConfigureAwait(true);
        await Pass(ValidatorRules.MustAsync<int>(static (value, _) => Task.FromResult(value > 0), "Must be positive."), 2).ConfigureAwait(true);
        await Fail(
            ValidatorRules.MustAsync<int>(static value => Task.FromResult(value > 0), "Must be positive."),
            -1,
            "Must be positive.").ConfigureAwait(true);
    }

    [Fact]
    public async Task A_cancelled_token_is_thrown_by_the_rule()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync().ConfigureAwait(true);
        Assert.Throws<OperationCanceledException>(() => ValidatorRules.NotNull<string>().ValidateAsync("x", cancellation.Token));
    }

    [Fact]
    public void Creation_rejects_bad_lengths_ranges_and_regular_expressions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.Length<string>(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.MinLength<string>(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.MaxLength<string>(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.Range<int>(3, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.NotRange<int>(3, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.ScalePrecision<decimal>(-1, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.ScalePrecision<decimal>(1, -1));
        Assert.Throws<ArgumentNullException>(() => ValidatorRules.NotNull<string>(null!));
        Assert.Throws<ArgumentNullException>(() => ValidatorRules.Matches<string>((string)null!));
        Assert.Throws<ArgumentNullException>(() => ValidatorRules.Matches<string>((Regex)null!, "x"));
        Assert.Throws<RegexParseException>(() => ValidatorRules.Matches<string>("["));
        Assert.Throws<ArgumentException>(() => ValidatorRules.Matches<string>(new Regex("a"), "x"));
        var timed = new Regex("a", RegexOptions.None, TimeSpan.FromSeconds(1));
        Assert.NotNull(ValidatorRules.NotMatches<string>(timed, "x"));
    }

    private static async Task Expect<T>(IValidatorRule<T> rule, T value, bool valid, string message)
    {
        if (valid)
        {
            await Pass(rule, value).ConfigureAwait(true);
            return;
        }

        await Fail(rule, value, message).ConfigureAwait(true);
    }

    private static async Task Pass<T>(IValidatorRule<T> rule, T value)
    {
        var result = await rule.ValidateAsync(value).ConfigureAwait(true);
        Assert.True(result.IsValid);
        Assert.Same(ValidationRuleResult.Success, result);
        Assert.Same(ValidationError.Empty, result.Errors);
    }

    private static async Task Fail<T>(IValidatorRule<T> rule, T value, string message)
    {
        var result = await rule.ValidateAsync(value).ConfigureAwait(true);
        Assert.False(result.IsValid);
        Assert.Equal(message, Assert.Single(result.Errors));
    }
}
