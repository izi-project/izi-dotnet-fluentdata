namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// Fluent extension methods that append a <see cref="ValidatorRules"/> rule to a <see cref="ValidatorRuleBuilder{T}"/>.
/// Each call returns the same builder, so rules chain directly:
/// <c>new ValidatorRuleBuilder&lt;string&gt;().NotEmpty().MaxLength(50).EmailAddress()</c>.
/// Every rule has an overload taking a custom failure message.
/// </summary>
public static class ValidatorRuleBuilderExtensions
{
    // =============================
    // Null & Emptiness
    // =============================

    /// <summary>Adds a rule requiring the value to be non-null.</summary>
    public static ValidatorRuleBuilder<T> NotNull<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.NotNull<T>());
    /// <summary>Adds a rule requiring the value to be non-null, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> NotNull<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.NotNull<T>(message));

    /// <summary>Adds a rule requiring the value to be null.</summary>
    public static ValidatorRuleBuilder<T> Null<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.Null<T>());
    /// <summary>Adds a rule requiring the value to be null, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> Null<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.Null<T>(message));

    /// <summary>Adds a rule requiring a non-empty value.</summary>
    public static ValidatorRuleBuilder<T> NotEmpty<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.NotEmpty<T>());
    /// <summary>Adds a rule requiring a non-empty value, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> NotEmpty<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.NotEmpty<T>(message));

    /// <summary>Adds a rule requiring an empty value.</summary>
    public static ValidatorRuleBuilder<T> Empty<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.Empty<T>());
    /// <summary>Adds a rule requiring an empty value, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> Empty<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.Empty<T>(message));

    // =============================
    // Equality
    // =============================

    /// <summary>Adds a rule requiring the value to equal <paramref name="expected"/>.</summary>
    public static ValidatorRuleBuilder<T> Equal<T>(this ValidatorRuleBuilder<T> builder, T expected) => builder.AddRule(ValidatorRules.Equal(expected));
    /// <summary>Adds a rule requiring the value to equal <paramref name="expected"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> Equal<T>(this ValidatorRuleBuilder<T> builder, T expected, string message) => builder.AddRule(ValidatorRules.Equal(expected, message));

    /// <summary>Adds a rule requiring the value to differ from <paramref name="expected"/>.</summary>
    public static ValidatorRuleBuilder<T> NotEqual<T>(this ValidatorRuleBuilder<T> builder, T expected) => builder.AddRule(ValidatorRules.NotEqual(expected));
    /// <summary>Adds a rule requiring the value to differ from <paramref name="expected"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> NotEqual<T>(this ValidatorRuleBuilder<T> builder, T expected, string message) => builder.AddRule(ValidatorRules.NotEqual(expected, message));

    // =============================
    // Comparison (require IComparable)
    // =============================

    /// <summary>Adds a rule requiring the value to be less than <paramref name="threshold"/>.</summary>
    public static ValidatorRuleBuilder<T> LessThan<T>(this ValidatorRuleBuilder<T> builder, T threshold) where T : IComparable<T> => builder.AddRule(ValidatorRules.LessThan(threshold));
    /// <summary>Adds a rule requiring the value to be less than <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> LessThan<T>(this ValidatorRuleBuilder<T> builder, T threshold, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.LessThan(threshold, message));

    /// <summary>Adds a rule requiring the value to be at most <paramref name="threshold"/>.</summary>
    public static ValidatorRuleBuilder<T> LessThanOrEqual<T>(this ValidatorRuleBuilder<T> builder, T threshold) where T : IComparable<T> => builder.AddRule(ValidatorRules.LessThanOrEqual(threshold));
    /// <summary>Adds a rule requiring the value to be at most <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> LessThanOrEqual<T>(this ValidatorRuleBuilder<T> builder, T threshold, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.LessThanOrEqual(threshold, message));

    /// <summary>Adds a rule requiring the value to be greater than <paramref name="threshold"/>.</summary>
    public static ValidatorRuleBuilder<T> GreaterThan<T>(this ValidatorRuleBuilder<T> builder, T threshold) where T : IComparable<T> => builder.AddRule(ValidatorRules.GreaterThan(threshold));
    /// <summary>Adds a rule requiring the value to be greater than <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> GreaterThan<T>(this ValidatorRuleBuilder<T> builder, T threshold, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.GreaterThan(threshold, message));

    /// <summary>Adds a rule requiring the value to be at least <paramref name="threshold"/>.</summary>
    public static ValidatorRuleBuilder<T> GreaterThanOrEqual<T>(this ValidatorRuleBuilder<T> builder, T threshold) where T : IComparable<T> => builder.AddRule(ValidatorRules.GreaterThanOrEqual(threshold));
    /// <summary>Adds a rule requiring the value to be at least <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> GreaterThanOrEqual<T>(this ValidatorRuleBuilder<T> builder, T threshold, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.GreaterThanOrEqual(threshold, message));

    /// <summary>Adds a rule requiring the value to fall within <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static ValidatorRuleBuilder<T> InRange<T>(this ValidatorRuleBuilder<T> builder, T min, T max) where T : IComparable<T> => builder.AddRule(ValidatorRules.Range(min, max));
    /// <summary>Adds a rule requiring the value to fall within <c>[<paramref name="min"/>, <paramref name="max"/>]</c>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> InRange<T>(this ValidatorRuleBuilder<T> builder, T min, T max, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.Range(min, max, message));

    /// <summary>Adds a rule requiring the value to fall outside <c>[<paramref name="min"/>, <paramref name="max"/>]</c>.</summary>
    public static ValidatorRuleBuilder<T> NotInRange<T>(this ValidatorRuleBuilder<T> builder, T min, T max) where T : IComparable<T> => builder.AddRule(ValidatorRules.NotRange(min, max));
    /// <summary>Adds a rule requiring the value to fall outside <c>[<paramref name="min"/>, <paramref name="max"/>]</c>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> NotInRange<T>(this ValidatorRuleBuilder<T> builder, T min, T max, string message) where T : IComparable<T> => builder.AddRule(ValidatorRules.NotRange(min, max, message));

    // =============================
    // Length
    // =============================

    /// <summary>Adds a rule requiring an exact length/count of <paramref name="expectedLength"/>.</summary>
    public static ValidatorRuleBuilder<T> Length<T>(this ValidatorRuleBuilder<T> builder, int expectedLength) => builder.AddRule(ValidatorRules.Length<T>(expectedLength));
    /// <summary>Adds a rule requiring an exact length/count of <paramref name="expectedLength"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> Length<T>(this ValidatorRuleBuilder<T> builder, int expectedLength, string message) => builder.AddRule(ValidatorRules.Length<T>(expectedLength, message));

    /// <summary>Adds a rule requiring a minimum length/count of <paramref name="minLength"/>.</summary>
    public static ValidatorRuleBuilder<T> MinLength<T>(this ValidatorRuleBuilder<T> builder, int minLength) => builder.AddRule(ValidatorRules.MinLength<T>(minLength));
    /// <summary>Adds a rule requiring a minimum length/count of <paramref name="minLength"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> MinLength<T>(this ValidatorRuleBuilder<T> builder, int minLength, string message) => builder.AddRule(ValidatorRules.MinLength<T>(minLength, message));

    /// <summary>Adds a rule requiring a maximum length/count of <paramref name="maxLength"/>.</summary>
    public static ValidatorRuleBuilder<T> MaxLength<T>(this ValidatorRuleBuilder<T> builder, int maxLength) => builder.AddRule(ValidatorRules.MaxLength<T>(maxLength));
    /// <summary>Adds a rule requiring a maximum length/count of <paramref name="maxLength"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> MaxLength<T>(this ValidatorRuleBuilder<T> builder, int maxLength, string message) => builder.AddRule(ValidatorRules.MaxLength<T>(maxLength, message));

    /// <summary>Adds a rule constraining a decimal's scale and precision.</summary>
    public static ValidatorRuleBuilder<T> ScalePrecision<T>(this ValidatorRuleBuilder<T> builder, int maxScale, int maxPrecision) => builder.AddRule(ValidatorRules.ScalePrecision<T>(maxScale, maxPrecision));
    /// <summary>Adds a rule constraining a decimal's scale and precision, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> ScalePrecision<T>(this ValidatorRuleBuilder<T> builder, int maxScale, int maxPrecision, string message) => builder.AddRule(ValidatorRules.ScalePrecision<T>(maxScale, maxPrecision, message));

    // =============================
    // Pattern matching
    // =============================

    /// <summary>Adds a rule requiring the value to match <paramref name="pattern"/>.</summary>
    public static ValidatorRuleBuilder<T> Matches<T>(this ValidatorRuleBuilder<T> builder, string pattern) => builder.AddRule(ValidatorRules.Matches<T>(pattern));
    /// <summary>Adds a rule requiring the value to match <paramref name="pattern"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> Matches<T>(this ValidatorRuleBuilder<T> builder, string pattern, string message) => builder.AddRule(ValidatorRules.Matches<T>(pattern, message));

    /// <summary>Adds a rule requiring the value to not match <paramref name="pattern"/>.</summary>
    public static ValidatorRuleBuilder<T> NotMatches<T>(this ValidatorRuleBuilder<T> builder, string pattern) => builder.AddRule(ValidatorRules.NotMatches<T>(pattern));
    /// <summary>Adds a rule requiring the value to not match <paramref name="pattern"/>, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> NotMatches<T>(this ValidatorRuleBuilder<T> builder, string pattern, string message) => builder.AddRule(ValidatorRules.NotMatches<T>(pattern, message));

    /// <summary>Adds a rule requiring the value to be a valid email address.</summary>
    public static ValidatorRuleBuilder<T> EmailAddress<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.Email<T>());
    /// <summary>Adds a rule requiring the value to be a valid email address, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> EmailAddress<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.Email<T>(message));

    /// <summary>Adds a rule requiring the value to be a valid credit-card number.</summary>
    public static ValidatorRuleBuilder<T> CreditCard<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.CreditCard<T>());
    /// <summary>Adds a rule requiring the value to be a valid credit-card number, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> CreditCard<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.CreditCard<T>(message));

    // =============================
    // ISO Codes
    // =============================

    /// <summary>Adds a rule requiring a valid ISO 3166-1 alpha-2 country code (e.g. <c>US</c>).</summary>
    public static ValidatorRuleBuilder<T> CountryIso2<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.CountryIso2<T>());
    /// <summary>Adds a rule requiring a valid ISO 3166-1 alpha-2 country code, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> CountryIso2<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.CountryIso2<T>(message));

    /// <summary>Adds a rule requiring a valid ISO 3166-1 alpha-3 country code (e.g. <c>USA</c>).</summary>
    public static ValidatorRuleBuilder<T> CountryIso3<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.CountryIso3<T>());
    /// <summary>Adds a rule requiring a valid ISO 3166-1 alpha-3 country code, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> CountryIso3<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.CountryIso3<T>(message));

    /// <summary>Adds a rule requiring a valid ISO 3166-1 numeric country code (e.g. <c>840</c>).</summary>
    public static ValidatorRuleBuilder<T> CountryIsoNumeric<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.CountryIsoNumeric<T>());
    /// <summary>Adds a rule requiring a valid ISO 3166-1 numeric country code, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> CountryIsoNumeric<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.CountryIsoNumeric<T>(message));

    /// <summary>Adds a rule requiring a valid ISO 4217 currency code (e.g. <c>USD</c>).</summary>
    public static ValidatorRuleBuilder<T> CurrencyIso<T>(this ValidatorRuleBuilder<T> builder) => builder.AddRule(ValidatorRules.CurrencyIso<T>());
    /// <summary>Adds a rule requiring a valid ISO 4217 currency code, with a custom message.</summary>
    public static ValidatorRuleBuilder<T> CurrencyIso<T>(this ValidatorRuleBuilder<T> builder, string message) => builder.AddRule(ValidatorRules.CurrencyIso<T>(message));

    // =============================
    // Custom predicate escape hatch
    // =============================

    /// <summary>Adds a rule from a custom predicate and failure message.</summary>
    /// <param name="builder">The rule builder.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message produced when the predicate fails.</param>
    public static ValidatorRuleBuilder<T> Must<T>(this ValidatorRuleBuilder<T> builder, Func<T, bool> predicate, string message)
        => builder.AddRule(new ValidatorRule<T>((value, _) => ValueTask.FromResult(predicate(value)), message));
}
