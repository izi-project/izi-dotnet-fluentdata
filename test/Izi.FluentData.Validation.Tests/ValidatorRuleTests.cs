using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation.Tests;

/// <summary>
/// Covers <see cref="ValidatorRule{T}"/>'s two message constructors: the constant string, reported verbatim, and
/// the <see cref="Func{T, TResult}"/> factory that receives the failing value. The central guarantee exercised
/// here is laziness — the message is built only when the predicate fails — across both the synchronous fast path
/// and the async slow path.
/// </summary>
public class ValidatorRuleTests
{
    private static ValueTask<bool> Pass<T>(T _, CancellationToken __) => ValueTask.FromResult(true);
    private static ValueTask<bool> Fail<T>(T _, CancellationToken __) => ValueTask.FromResult(false);

    // Yields for real, so the rule leaves the synchronous fast path and runs through ValidateSlowAsync.
    private static async ValueTask<bool> FailAsync<T>(T _, CancellationToken ct)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();
        return false;
    }

    private static async ValueTask<bool> PassAsync<T>(T _, CancellationToken ct)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();
        return true;
    }

    // =============================
    // Constant message constructor
    // =============================
    [Fact]
    public async Task Constant_message_is_returned_on_failure()
    {
        var rule = new ValidatorRule<string>(Fail, "Value is invalid.");
        Assert.Equal("Value is invalid.", await rule.ValidateAsync("x"));
    }

    [Fact]
    public async Task Constant_message_is_not_returned_on_success()
    {
        var rule = new ValidatorRule<string>(Pass, "Value is invalid.");
        Assert.Null(await rule.ValidateAsync("x"));
    }

    /// <summary>
    /// Constant messages are reported verbatim: braces are ordinary characters, never format holes. Callers
    /// needing substitution use the factory overload and interpolate there.
    /// </summary>
    [Theory]
    [InlineData("Value {0} is invalid.")]
    [InlineData("Value must be valid JSON like {}.")]
    [InlineData("Value must be a {valid} code.")]
    public async Task Constant_message_treats_braces_as_literal_text(string message)
    {
        var rule = new ValidatorRule<string>(Fail, message);
        Assert.Equal(message, await rule.ValidateAsync("x"));
    }

    /// <summary>
    /// The catalogue's own default messages embed braces — <c>Matches</c> interpolates the caller's regex, and
    /// quantifiers like <c>{3}</c> are routine — so this pins that they survive to the failure path intact.
    /// </summary>
    [Fact]
    public async Task Constant_message_tolerates_braces_from_an_embedded_regex_pattern()
    {
        var rule = ValidatorRules.Matches<string>(@"^\d{3}$");

        var error = await rule.ValidateAsync("12");

        Assert.Equal(@"Value must match the pattern: ^\d{3}$.", error);
    }

    // =============================
    // Instance-aware message factory
    // =============================
    [Fact]
    public async Task Message_factory_receives_the_failing_value()
    {
        var rule = new ValidatorRule<string>(Fail, value => $"'{value}' is not a valid code.");
        Assert.Equal("'abc' is not a valid code.", await rule.ValidateAsync("abc"));
    }

    /// <summary>Unlike the captured format arguments, the factory sees the value supplied at validation time.</summary>
    [Fact]
    public async Task Message_factory_is_evaluated_per_call()
    {
        var rule = new ValidatorRule<int>(Fail, value => $"{value} is out of range.");

        Assert.Equal("1 is out of range.", await rule.ValidateAsync(1));
        Assert.Equal("2 is out of range.", await rule.ValidateAsync(2));
    }

    // =============================
    // Laziness
    // =============================
    [Fact]
    public async Task Message_factory_is_not_invoked_when_the_rule_passes()
    {
        var invocations = 0;
        var rule = new ValidatorRule<string>(Pass, _ => { invocations++; return "unused"; });

        Assert.Null(await rule.ValidateAsync("x"));
        Assert.Equal(0, invocations);
    }

    [Fact]
    public async Task Message_factory_is_invoked_once_per_failure()
    {
        var invocations = 0;
        var rule = new ValidatorRule<string>(Fail, _ => { invocations++; return "failed"; });

        Assert.Equal("failed", await rule.ValidateAsync("x"));
        Assert.Equal(1, invocations);

        Assert.Equal("failed", await rule.ValidateAsync("y"));
        Assert.Equal(2, invocations);
    }

    // =============================
    // Async slow path
    // =============================
    [Fact]
    public async Task Message_factory_receives_the_value_on_the_async_path()
    {
        var rule = new ValidatorRule<string>(FailAsync, value => $"'{value}' is already registered.");
        Assert.Equal("'a@b.co' is already registered.", await rule.ValidateAsync("a@b.co"));
    }

    [Fact]
    public async Task Message_factory_is_not_invoked_on_the_async_path_when_the_rule_passes()
    {
        var invocations = 0;
        var rule = new ValidatorRule<string>(PassAsync, _ => { invocations++; return "unused"; });

        Assert.Null(await rule.ValidateAsync("x"));
        Assert.Equal(0, invocations);
    }

    [Fact]
    public async Task Constant_message_is_returned_on_the_async_path()
    {
        var rule = new ValidatorRule<int>(FailAsync, "Value must be less than 10.");
        Assert.Equal("Value must be less than 10.", await rule.ValidateAsync(42));
    }

    // =============================
    // Aggregation through Validator<T>
    // =============================
    [Fact]
    public async Task Instance_aware_messages_flow_through_the_validator()
    {
        var validator = new Validator<int>().AddRule(
            new ValidatorRule<int>((value, _) => ValueTask.FromResult(value > 0), value => $"{value} must be positive."),
            new ValidatorRule<int>((value, _) => ValueTask.FromResult(value % 2 == 0), value => $"{value} must be even."));

        var errors = await validator.ValidateAsync(-3);

        Assert.Equal(["-3 must be positive.", "-3 must be even."], errors);
    }

    [Fact]
    public async Task Valid_instance_still_reports_no_errors_with_instance_aware_messages()
    {
        var validator = new Validator<int>().AddRule(
            new ValidatorRule<int>((value, _) => ValueTask.FromResult(value > 0), value => $"{value} must be positive."));

        Assert.Empty(await validator.ValidateAsync(4));
    }
}
