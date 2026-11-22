using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation.Tests;

public class FluentWrapperTests
{
    [Fact]
    public async Task General_wrappers_match_the_catalogue()
    {
        await Check<string?>(property => property.NotNull(), ValidatorRules.NotNull<string?>(), null, "x");
        await Check<string?>(property => property.NotNull("custom"), ValidatorRules.NotNull<string?>("custom"), null, "x");
        await Check<string?>(property => property.Null(), ValidatorRules.Null<string?>(), "x", null);
        await Check<string?>(property => property.Null("custom"), ValidatorRules.Null<string?>("custom"), "x", null);
        await Check<string?>(property => property.NotEmpty(), ValidatorRules.NotEmpty<string?>(), "", "x");
        await Check<string?>(property => property.NotEmpty("custom"), ValidatorRules.NotEmpty<string?>("custom"), "", "x");
        await Check<string?>(property => property.Empty(), ValidatorRules.Empty<string?>(), "x", "");
        await Check<string?>(property => property.Empty("custom"), ValidatorRules.Empty<string?>("custom"), "x", "");
        await Check<int>(property => property.Must(value => value > 0, "positive"), ValidatorRules.Must<int>(value => value > 0, "positive"), 0, 1);
        await Check<int>(
            property => property.MustAsync((value, _) => Task.FromResult(value > 0), "positive"),
            ValidatorRules.MustAsync<int>((value, _) => Task.FromResult(value > 0), "positive"),
            0,
            1);
        await Check<int>(
            property => property.MustAsync(value => Task.FromResult(value > 0), "positive"),
            ValidatorRules.MustAsync<int>(value => Task.FromResult(value > 0), "positive"),
            0,
            1);
    }

    [Fact]
    public async Task Comparison_wrappers_match_the_catalogue()
    {
        await Check<int>(property => property.Equal(5), ValidatorRules.Equal(5), 4, 5);
        await Check<int>(property => property.Equal(5, "custom"), ValidatorRules.Equal(5, "custom"), 4, 5);
        await Check<int>(property => property.NotEqual(5), ValidatorRules.NotEqual(5), 5, 4);
        await Check<int>(property => property.NotEqual(5, "custom"), ValidatorRules.NotEqual(5, "custom"), 5, 4);
        await Check<int>(property => property.LessThan(10), ValidatorRules.LessThan(10), 10, 9);
        await Check<int>(property => property.LessThan(10, "custom"), ValidatorRules.LessThan(10, "custom"), 10, 9);
        await Check<int>(property => property.LessThanOrEqual(10), ValidatorRules.LessThanOrEqual(10), 11, 10);
        await Check<int>(property => property.LessThanOrEqual(10, "custom"), ValidatorRules.LessThanOrEqual(10, "custom"), 11, 10);
        await Check<int>(property => property.GreaterThan(10), ValidatorRules.GreaterThan(10), 10, 11);
        await Check<int>(property => property.GreaterThan(10, "custom"), ValidatorRules.GreaterThan(10, "custom"), 10, 11);
        await Check<int>(property => property.GreaterThanOrEqual(10), ValidatorRules.GreaterThanOrEqual(10), 9, 10);
        await Check<int>(property => property.GreaterThanOrEqual(10, "custom"), ValidatorRules.GreaterThanOrEqual(10, "custom"), 9, 10);
    }

    [Fact]
    public async Task String_wrappers_match_the_catalogue()
    {
        var regex = new Regex(@"^\d+$", RegexOptions.None, TimeSpan.FromSeconds(1));
        await Check<string?>(property => property.Length(2), ValidatorRules.Length<string?>(2), "a", "ab");
        await Check<string?>(property => property.Length(2, "custom"), ValidatorRules.Length<string?>(2, "custom"), "a", "ab");
        await Check<string?>(property => property.MinLength(2), ValidatorRules.MinLength<string?>(2), "a", "ab");
        await Check<string?>(property => property.MinLength(2, "custom"), ValidatorRules.MinLength<string?>(2, "custom"), "a", "ab");
        await Check<string?>(property => property.MaxLength(2), ValidatorRules.MaxLength<string?>(2), "abc", "ab");
        await Check<string?>(property => property.MaxLength(2, "custom"), ValidatorRules.MaxLength<string?>(2, "custom"), "abc", "ab");
        await Check<string>(property => property.Matches(@"^\d+$"), ValidatorRules.Matches<string>(@"^\d+$"), "ab", "12");
        await Check<string>(property => property.Matches(@"^\d+$", "custom"), ValidatorRules.Matches<string>(@"^\d+$", "custom"), "ab", "12");
        await Check<string>(property => property.Matches(regex, "custom"), ValidatorRules.Matches<string>(regex, "custom"), "ab", "12");
        await Check<string>(property => property.NotMatches(@"^\d+$"), ValidatorRules.NotMatches<string>(@"^\d+$"), "12", "ab");
        await Check<string>(property => property.NotMatches(@"^\d+$", "custom"), ValidatorRules.NotMatches<string>(@"^\d+$", "custom"), "12", "ab");
        await Check<string>(property => property.NotMatches(regex, "custom"), ValidatorRules.NotMatches<string>(regex, "custom"), "12", "ab");
        await Check<string>(property => property.Email(), ValidatorRules.Email<string>(), "nope", "a@b.co");
        await Check<string>(property => property.Email("custom"), ValidatorRules.Email<string>("custom"), "nope", "a@b.co");
        await Check<string>(property => property.CreditCard(), ValidatorRules.CreditCard<string>(), "1234", "4111111111111111");
        await Check<string>(property => property.CreditCard("custom"), ValidatorRules.CreditCard<string>("custom"), "1234", "4111111111111111");
    }

    [Fact]
    public async Task Numeric_and_iso_wrappers_match_the_catalogue()
    {
        await Check<int>(property => property.Range(1, 3), ValidatorRules.Range(1, 3), 0, 2);
        await Check<int>(property => property.Range(1, 3, "custom"), ValidatorRules.Range(1, 3, "custom"), 0, 2);
        await Check<int>(property => property.NotRange(1, 3), ValidatorRules.NotRange(1, 3), 2, 0);
        await Check<int>(property => property.NotRange(1, 3, "custom"), ValidatorRules.NotRange(1, 3, "custom"), 2, 0);
        await Check<decimal>(property => property.ScalePrecision(2, 3), ValidatorRules.ScalePrecision<decimal>(2, 3), 1234.5m, 12.3m);
        await Check<decimal>(property => property.ScalePrecision(2, 3, "custom"), ValidatorRules.ScalePrecision<decimal>(2, 3, "custom"), 1234.5m, 12.3m);
        await Check<string>(property => property.CountryIso2(), ValidatorRules.CountryIso2<string>(), "ZZ", "US");
        await Check<string>(property => property.CountryIso2("custom"), ValidatorRules.CountryIso2<string>("custom"), "ZZ", "US");
        await Check<string>(property => property.CountryIso3(), ValidatorRules.CountryIso3<string>(), "ZZZ", "USA");
        await Check<string>(property => property.CountryIso3("custom"), ValidatorRules.CountryIso3<string>("custom"), "ZZZ", "USA");
        await Check<string>(property => property.CountryIsoNumeric(), ValidatorRules.CountryIsoNumeric<string>(), "999", "840");
        await Check<string>(property => property.CountryIsoNumeric("custom"), ValidatorRules.CountryIsoNumeric<string>("custom"), "999", "840");
        await Check<string>(property => property.CurrencyIso(), ValidatorRules.CurrencyIso<string>(), "ZZZ", "USD");
        await Check<string>(property => property.CurrencyIso("custom"), ValidatorRules.CurrencyIso<string>("custom"), "ZZZ", "USD");
    }

    private static async Task Check<T>(
        Func<PropertyValidator<Box<T>, T>, PropertyValidator<Box<T>, T>> apply,
        IValidatorRule<T> rule,
        T failing,
        T passing)
    {
        var validator = new Validator<Box<T>>();
        apply(validator.RuleFor(box => box.Value));
        var failed = await validator.ValidateAsync(new Box<T> { Value = failing }).ConfigureAwait(true);
        var direct = await rule.ValidateAsync(failing).ConfigureAwait(true);
        Assert.False(direct.IsValid);
        Assert.Equal(direct.Errors[0], Assert.Single(Assert.Single(failed.Errors).Errors));
        Assert.True((await validator.ValidateAsync(new Box<T> { Value = passing }).ConfigureAwait(true)).IsValid);
        Assert.True((await rule.ValidateAsync(passing).ConfigureAwait(true)).IsValid);
    }

    private sealed class Box<T>
    {
        public T Value { get; set; } = default!;
    }
}
