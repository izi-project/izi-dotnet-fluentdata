using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation.Tests;

/// <summary>
/// Covers the built-in <see cref="ValidatorRules"/> factory rules in isolation: null/emptiness, equality and
/// comparison, length and range, scale/precision, pattern matching (regex, email, credit card), and custom
/// message propagation.
/// </summary>
public class RuleTests
{
    /// <summary>Evaluates a single rule against a value and returns its failure message, or null when valid.</summary>
    private static async Task<string?> Eval<T>(IValidatorRule<T> rule, T value)
        => await rule.ValidateAsync(value);

    // =============================
    // Null & Emptiness
    // =============================
    [Fact]
    public async Task NotNull_fails_on_null()
    {
        Assert.NotNull(await Eval(ValidatorRules.NotNull<string?>(), null));
        Assert.Null(await Eval(ValidatorRules.NotNull<string?>(), "x"));
    }

    [Fact]
    public async Task Null_fails_on_value()
    {
        Assert.Null(await Eval(ValidatorRules.Null<string?>(), null));
        Assert.NotNull(await Eval(ValidatorRules.Null<string?>(), "x"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task NotEmpty_fails_on_blank_string(string? value)
        => Assert.NotNull(await Eval(ValidatorRules.NotEmpty<string?>(), value));

    [Fact]
    public async Task NotEmpty_passes_on_text()
        => Assert.Null(await Eval(ValidatorRules.NotEmpty<string>(), "x"));

    [Fact]
    public async Task NotEmpty_handles_collections()
    {
        Assert.NotNull(await Eval(ValidatorRules.NotEmpty<List<string>>(), []));
        Assert.Null(await Eval(ValidatorRules.NotEmpty<List<string>>(), ["a"]));
    }

    [Fact]
    public async Task Empty_passes_on_blank()
    {
        Assert.Null(await Eval(ValidatorRules.Empty<string>(), ""));
        Assert.NotNull(await Eval(ValidatorRules.Empty<string>(), "x"));
    }

    // =============================
    // Equality & Comparison
    // =============================
    [Fact]
    public async Task Equal_and_NotEqual()
    {
        Assert.Null(await Eval(ValidatorRules.Equal<int>(5), 5));
        Assert.NotNull(await Eval(ValidatorRules.Equal<int>(5), 4));
        Assert.NotNull(await Eval(ValidatorRules.NotEqual<int>(5), 5));
        Assert.Null(await Eval(ValidatorRules.NotEqual<int>(5), 4));
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(10, false)]
    [InlineData(11, false)]
    public async Task LessThan(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.LessThan<int>(10), value) is null);

    [Theory]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public async Task LessThanOrEqual(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.LessThanOrEqual<int>(10), value) is null);

    [Theory]
    [InlineData(11, true)]
    [InlineData(10, false)]
    public async Task GreaterThan(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.GreaterThan<int>(10), value) is null);

    [Theory]
    [InlineData(10, true)]
    [InlineData(9, false)]
    public async Task GreaterThanOrEqual(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.GreaterThanOrEqual<int>(10), value) is null);

    // =============================
    // Length & Range
    // =============================
    [Theory]
    [InlineData("abc", true)]
    [InlineData("ab", false)]
    [InlineData(null, false)]
    public async Task Length_exact(string? value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.Length<string?>(3), value) is null);

    [Fact]
    public async Task MinLength_and_MaxLength()
    {
        Assert.Null(await Eval(ValidatorRules.MinLength<string>(3), "abc"));
        Assert.NotNull(await Eval(ValidatorRules.MinLength<string>(3), "ab"));
        Assert.Null(await Eval(ValidatorRules.MaxLength<string>(3), "abc"));
        Assert.NotNull(await Eval(ValidatorRules.MaxLength<string>(3), "abcd"));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(0, false)]
    [InlineData(11, false)]
    public async Task InRange(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.Range<int>(1, 10), value) is null);

    [Theory]
    [InlineData(5, false)]
    [InlineData(0, true)]
    [InlineData(11, true)]
    public async Task NotInRange(int value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.NotRange<int>(1, 10), value) is null);

    // =============================
    // Scale & Precision
    // =============================
    [Fact]
    public async Task ScalePrecision()
    {
        Assert.Null(await Eval(ValidatorRules.ScalePrecision<decimal>(2, 3), 123.45m));
        Assert.NotNull(await Eval(ValidatorRules.ScalePrecision<decimal>(1, 3), 123.45m)); // scale 2 > 1
        Assert.NotNull(await Eval(ValidatorRules.ScalePrecision<decimal>(2, 2), 123.45m)); // precision 3 > 2
    }

    // =============================
    // Pattern Matching
    // =============================
    [Fact]
    public async Task Matches_and_NotMatches()
    {
        Assert.Null(await Eval(ValidatorRules.Matches<string>(@"^\d+$"), "123"));
        Assert.NotNull(await Eval(ValidatorRules.Matches<string>(@"^\d+$"), "12a"));
        Assert.Null(await Eval(ValidatorRules.NotMatches<string>(@"^\d+$"), "12a"));
        Assert.NotNull(await Eval(ValidatorRules.NotMatches<string>(@"^\d+$"), "123"));
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("a@b.co", true)]
    [InlineData("invalid", false)]
    [InlineData("no@dot", false)]
    public async Task Email(string value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.Email<string>(), value) is null);

    [Theory]
    [InlineData("4111111111111111", true)]
    [InlineData("1234", false)]
    public async Task CreditCard(string value, bool valid)
        => Assert.Equal(valid, await Eval(ValidatorRules.CreditCard<string>(), value) is null);

    // =============================
    // Custom messages
    // =============================
    [Fact]
    public async Task Custom_message_is_returned()
    {
        var error = await Eval(ValidatorRules.NotNull<string?>("Name is required."), null);
        Assert.Equal("Name is required.", error);
    }

    [Fact]
    public async Task ValidateAsync_throws_when_canceled_before_sync_rule_evaluation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await ValidatorRules.NotNull<string?>().ValidateAsync("x", cancellation.Token));
    }
}
