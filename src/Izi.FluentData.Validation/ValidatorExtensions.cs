using System.Text.RegularExpressions;
using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation;

/// <summary>
/// Fluent wrappers over <see cref="ValidatorRules"/> that call <see cref="Validator{T}.AddRule"/> and
/// return the same <see cref="Validator{T}"/> so calls can be chained, e.g. <c>RuleFor(x => x.Name).NotNull().NotEmpty()</c>.
/// </summary>
/// <remarks>
/// Every rule comes in three flavours: the default message, a constant custom message, and a
/// <c>Func&lt;T, string&gt;</c> factory that is invoked only on failure and receives the value that failed —
/// e.g. <c>.CountryIso2(code =&gt; $"'{code}' is not a supported country.")</c>.
/// </remarks>
public static class ValidatorExtensions
{
    // =============================
    // Null & Emptiness Checks
    // =============================

    /// <summary>Adds a rule requiring the value to be non-null.</summary>
    public static Validator<T> NotNull<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.NotNull<T>());
    /// <summary>Adds a rule requiring the value to be non-null, with a custom message.</summary>
    public static Validator<T> NotNull<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.NotNull<T>(message));
    /// <summary>Adds a rule requiring the value to be non-null, with a message built from the failing value.</summary>
    public static Validator<T> NotNull<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.NotNull(messageFunc));

    /// <summary>Adds a rule requiring the value to be null.</summary>
    public static Validator<T> Null<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.Null<T>());
    /// <summary>Adds a rule requiring the value to be null, with a custom message.</summary>
    public static Validator<T> Null<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.Null<T>(message));
    /// <summary>Adds a rule requiring the value to be null, with a message built from the failing value.</summary>
    public static Validator<T> Null<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Null(messageFunc));

    /// <summary>Adds a rule requiring a non-empty value (non-blank string or non-empty collection).</summary>
    public static Validator<T> NotEmpty<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.NotEmpty<T>());
    /// <summary>Adds a rule requiring a non-empty value (non-blank string or non-empty collection), with a custom message.</summary>
    public static Validator<T> NotEmpty<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.NotEmpty<T>(message));
    /// <summary>Adds a rule requiring a non-empty value (non-blank string or non-empty collection), with a message built from the failing value.</summary>
    public static Validator<T> NotEmpty<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.NotEmpty(messageFunc));

    /// <summary>Adds a rule requiring an empty value (null/blank string or empty collection).</summary>
    public static Validator<T> Empty<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.Empty<T>());
    /// <summary>Adds a rule requiring an empty value (null/blank string or empty collection), with a custom message.</summary>
    public static Validator<T> Empty<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.Empty<T>(message));
    /// <summary>Adds a rule requiring an empty value (null/blank string or empty collection), with a message built from the failing value.</summary>
    public static Validator<T> Empty<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Empty(messageFunc));

    // =============================
    // Comparison Checks
    // =============================

    /// <summary>Adds a rule requiring the value to equal <paramref name="expectedValue"/>.</summary>
    public static Validator<T> Equal<T>(this Validator<T> validator, T expectedValue) => validator.AddRule(ValidatorRules.Equal(expectedValue));
    /// <summary>Adds a rule requiring the value to equal <paramref name="expectedValue"/>, with a custom message.</summary>
    public static Validator<T> Equal<T>(this Validator<T> validator, T expectedValue, string message) => validator.AddRule(ValidatorRules.Equal(expectedValue, message));
    /// <summary>Adds a rule requiring the value to equal <paramref name="expectedValue"/>, with a message built from the failing value.</summary>
    public static Validator<T> Equal<T>(this Validator<T> validator, T expectedValue, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Equal(expectedValue, messageFunc));

    /// <summary>Adds a rule requiring the value to differ from <paramref name="expectedValue"/>.</summary>
    public static Validator<T> NotEqual<T>(this Validator<T> validator, T expectedValue) => validator.AddRule(ValidatorRules.NotEqual(expectedValue));
    /// <summary>Adds a rule requiring the value to differ from <paramref name="expectedValue"/>, with a custom message.</summary>
    public static Validator<T> NotEqual<T>(this Validator<T> validator, T expectedValue, string message) => validator.AddRule(ValidatorRules.NotEqual(expectedValue, message));
    /// <summary>Adds a rule requiring the value to differ from <paramref name="expectedValue"/>, with a message built from the failing value.</summary>
    public static Validator<T> NotEqual<T>(this Validator<T> validator, T expectedValue, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.NotEqual(expectedValue, messageFunc));

    /// <summary>Adds a rule requiring the value to be strictly less than <paramref name="threshold"/>.</summary>
    public static Validator<T> LessThan<T>(this Validator<T> validator, T threshold) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThan(threshold));
    /// <summary>Adds a rule requiring the value to be strictly less than <paramref name="threshold"/>, with a custom message.</summary>
    public static Validator<T> LessThan<T>(this Validator<T> validator, T threshold, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThan(threshold, message));
    /// <summary>Adds a rule requiring the value to be strictly less than <paramref name="threshold"/>, with a message built from the failing value.</summary>
    public static Validator<T> LessThan<T>(this Validator<T> validator, T threshold, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThan(threshold, messageFunc));

    /// <summary>Adds a rule requiring the value to be less than or equal to <paramref name="threshold"/>.</summary>
    public static Validator<T> LessThanOrEqual<T>(this Validator<T> validator, T threshold) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThanOrEqual(threshold));
    /// <summary>Adds a rule requiring the value to be less than or equal to <paramref name="threshold"/>, with a custom message.</summary>
    public static Validator<T> LessThanOrEqual<T>(this Validator<T> validator, T threshold, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThanOrEqual(threshold, message));
    /// <summary>Adds a rule requiring the value to be less than or equal to <paramref name="threshold"/>, with a message built from the failing value.</summary>
    public static Validator<T> LessThanOrEqual<T>(this Validator<T> validator, T threshold, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.LessThanOrEqual(threshold, messageFunc));

    /// <summary>Adds a rule requiring the value to be strictly greater than <paramref name="threshold"/>.</summary>
    public static Validator<T> GreaterThan<T>(this Validator<T> validator, T threshold) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThan(threshold));
    /// <summary>Adds a rule requiring the value to be strictly greater than <paramref name="threshold"/>, with a custom message.</summary>
    public static Validator<T> GreaterThan<T>(this Validator<T> validator, T threshold, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThan(threshold, message));
    /// <summary>Adds a rule requiring the value to be strictly greater than <paramref name="threshold"/>, with a message built from the failing value.</summary>
    public static Validator<T> GreaterThan<T>(this Validator<T> validator, T threshold, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThan(threshold, messageFunc));

    /// <summary>Adds a rule requiring the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    public static Validator<T> GreaterThanOrEqual<T>(this Validator<T> validator, T threshold) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThanOrEqual(threshold));
    /// <summary>Adds a rule requiring the value to be greater than or equal to <paramref name="threshold"/>, with a custom message.</summary>
    public static Validator<T> GreaterThanOrEqual<T>(this Validator<T> validator, T threshold, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThanOrEqual(threshold, message));
    /// <summary>Adds a rule requiring the value to be greater than or equal to <paramref name="threshold"/>, with a message built from the failing value.</summary>
    public static Validator<T> GreaterThanOrEqual<T>(this Validator<T> validator, T threshold, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.GreaterThanOrEqual(threshold, messageFunc));

    // =============================
    // Length & Range Checks
    // =============================

    /// <summary>Adds a rule requiring a string or collection to have an exact length/count of <paramref name="expectedLength"/>.</summary>
    public static Validator<T> Length<T>(this Validator<T> validator, int expectedLength) => validator.AddRule(ValidatorRules.Length<T>(expectedLength));
    /// <summary>Adds a rule requiring a string or collection to have an exact length/count of <paramref name="expectedLength"/>, with a custom message.</summary>
    public static Validator<T> Length<T>(this Validator<T> validator, int expectedLength, string message) => validator.AddRule(ValidatorRules.Length<T>(expectedLength, message));
    /// <summary>Adds a rule requiring a string or collection to have an exact length/count of <paramref name="expectedLength"/>, with a message built from the failing value.</summary>
    public static Validator<T> Length<T>(this Validator<T> validator, int expectedLength, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Length(expectedLength, messageFunc));

    /// <summary>Adds a rule requiring a string or collection to meet a minimum length/count of <paramref name="minLength"/>.</summary>
    public static Validator<T> MinLength<T>(this Validator<T> validator, int minLength) => validator.AddRule(ValidatorRules.MinLength<T>(minLength));
    /// <summary>Adds a rule requiring a string or collection to meet a minimum length/count of <paramref name="minLength"/>, with a custom message.</summary>
    public static Validator<T> MinLength<T>(this Validator<T> validator, int minLength, string message) => validator.AddRule(ValidatorRules.MinLength<T>(minLength, message));
    /// <summary>Adds a rule requiring a string or collection to meet a minimum length/count of <paramref name="minLength"/>, with a message built from the failing value.</summary>
    public static Validator<T> MinLength<T>(this Validator<T> validator, int minLength, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.MinLength(minLength, messageFunc));

    /// <summary>Adds a rule requiring a string or collection to stay within a maximum length/count of <paramref name="maxLength"/>.</summary>
    public static Validator<T> MaxLength<T>(this Validator<T> validator, int maxLength) => validator.AddRule(ValidatorRules.MaxLength<T>(maxLength));
    /// <summary>Adds a rule requiring a string or collection to stay within a maximum length/count of <paramref name="maxLength"/>, with a custom message.</summary>
    public static Validator<T> MaxLength<T>(this Validator<T> validator, int maxLength, string message) => validator.AddRule(ValidatorRules.MaxLength<T>(maxLength, message));
    /// <summary>Adds a rule requiring a string or collection to stay within a maximum length/count of <paramref name="maxLength"/>, with a message built from the failing value.</summary>
    public static Validator<T> MaxLength<T>(this Validator<T> validator, int maxLength, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.MaxLength(maxLength, messageFunc));

    /// <summary>Adds a rule requiring the value to fall within the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>.</summary>
    public static Validator<T> Range<T>(this Validator<T> validator, T minValue, T maxValue) where T : IComparable<T> => validator.AddRule(ValidatorRules.Range(minValue, maxValue));
    /// <summary>Adds a rule requiring the value to fall within the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a custom message.</summary>
    public static Validator<T> Range<T>(this Validator<T> validator, T minValue, T maxValue, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.Range(minValue, maxValue, message));
    /// <summary>Adds a rule requiring the value to fall within the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a message built from the failing value.</summary>
    public static Validator<T> Range<T>(this Validator<T> validator, T minValue, T maxValue, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.Range(minValue, maxValue, messageFunc));

    /// <summary>Adds a rule requiring the value to fall outside the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>.</summary>
    public static Validator<T> NotRange<T>(this Validator<T> validator, T minValue, T maxValue) where T : IComparable<T> => validator.AddRule(ValidatorRules.NotRange(minValue, maxValue));
    /// <summary>Adds a rule requiring the value to fall outside the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a custom message.</summary>
    public static Validator<T> NotRange<T>(this Validator<T> validator, T minValue, T maxValue, string message) where T : IComparable<T> => validator.AddRule(ValidatorRules.NotRange(minValue, maxValue, message));
    /// <summary>Adds a rule requiring the value to fall outside the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a message built from the failing value.</summary>
    public static Validator<T> NotRange<T>(this Validator<T> validator, T minValue, T maxValue, Func<T, string> messageFunc) where T : IComparable<T> => validator.AddRule(ValidatorRules.NotRange(minValue, maxValue, messageFunc));

    // =============================
    // Format & Pattern Matching
    // =============================

    /// <summary>Adds a rule requiring a decimal value to respect a maximum scale and precision.</summary>
    public static Validator<T> ScalePrecision<T>(this Validator<T> validator, int maxScale, int maxPrecision) => validator.AddRule(ValidatorRules.ScalePrecision<T>(maxScale, maxPrecision));
    /// <summary>Adds a rule requiring a decimal value to respect a maximum scale and precision, with a custom message.</summary>
    public static Validator<T> ScalePrecision<T>(this Validator<T> validator, int maxScale, int maxPrecision, string message) => validator.AddRule(ValidatorRules.ScalePrecision<T>(maxScale, maxPrecision, message));
    /// <summary>Adds a rule requiring a decimal value to respect a maximum scale and precision, with a message built from the failing value.</summary>
    public static Validator<T> ScalePrecision<T>(this Validator<T> validator, int maxScale, int maxPrecision, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.ScalePrecision(maxScale, maxPrecision, messageFunc));

    /// <summary>Adds a rule requiring a string to match the regular expression <paramref name="pattern"/>.</summary>
    public static Validator<T> Matches<T>(this Validator<T> validator, string pattern) => validator.AddRule(ValidatorRules.Matches<T>(pattern));
    /// <summary>Adds a rule requiring a string to match the regular expression <paramref name="pattern"/>, with a custom message.</summary>
    public static Validator<T> Matches<T>(this Validator<T> validator, string pattern, string message) => validator.AddRule(ValidatorRules.Matches<T>(pattern, message));
    /// <summary>Adds a rule requiring a string to match the regular expression <paramref name="pattern"/>, with a message built from the failing value.</summary>
    public static Validator<T> Matches<T>(this Validator<T> validator, string pattern, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Matches(pattern, messageFunc));
    /// <summary>Adds a rule requiring a string to match the precompiled regular expression <paramref name="regex"/>, with a custom message.</summary>
    public static Validator<T> Matches<T>(this Validator<T> validator, Regex regex, string message) => validator.AddRule(ValidatorRules.Matches<T>(regex, message));
    /// <summary>Adds a rule requiring a string to match the precompiled regular expression <paramref name="regex"/>, with a message built from the failing value.</summary>
    public static Validator<T> Matches<T>(this Validator<T> validator, Regex regex, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Matches(regex, messageFunc));

    /// <summary>Adds a rule requiring a string to not match the regular expression <paramref name="pattern"/>.</summary>
    public static Validator<T> NotMatches<T>(this Validator<T> validator, string pattern) => validator.AddRule(ValidatorRules.NotMatches<T>(pattern));
    /// <summary>Adds a rule requiring a string to not match the regular expression <paramref name="pattern"/>, with a custom message.</summary>
    public static Validator<T> NotMatches<T>(this Validator<T> validator, string pattern, string message) => validator.AddRule(ValidatorRules.NotMatches<T>(pattern, message));
    /// <summary>Adds a rule requiring a string to not match the regular expression <paramref name="pattern"/>, with a message built from the failing value.</summary>
    public static Validator<T> NotMatches<T>(this Validator<T> validator, string pattern, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.NotMatches(pattern, messageFunc));
    /// <summary>Adds a rule requiring a string to not match the precompiled regular expression <paramref name="regex"/>, with a custom message.</summary>
    public static Validator<T> NotMatches<T>(this Validator<T> validator, Regex regex, string message) => validator.AddRule(ValidatorRules.NotMatches<T>(regex, message));
    /// <summary>Adds a rule requiring a string to not match the precompiled regular expression <paramref name="regex"/>, with a message built from the failing value.</summary>
    public static Validator<T> NotMatches<T>(this Validator<T> validator, Regex regex, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.NotMatches(regex, messageFunc));

    /// <summary>Adds a rule requiring a string to be a valid email address.</summary>
    public static Validator<T> Email<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.Email<T>());
    /// <summary>Adds a rule requiring a string to be a valid email address, with a custom message.</summary>
    public static Validator<T> Email<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.Email<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid email address, with a message built from the failing value.</summary>
    public static Validator<T> Email<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.Email(messageFunc));

    /// <summary>Adds a rule requiring a string to be a valid credit-card number.</summary>
    public static Validator<T> CreditCard<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.CreditCard<T>());
    /// <summary>Adds a rule requiring a string to be a valid credit-card number, with a custom message.</summary>
    public static Validator<T> CreditCard<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.CreditCard<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid credit-card number, with a message built from the failing value.</summary>
    public static Validator<T> CreditCard<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.CreditCard(messageFunc));

    // =============================
    // ISO Code Checks
    // =============================

    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-2 country code (e.g. <c>US</c>).</summary>
    public static Validator<T> CountryIso2<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.CountryIso2<T>());
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-2 country code, with a custom message.</summary>
    public static Validator<T> CountryIso2<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.CountryIso2<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-2 country code, with a message built from the failing value.</summary>
    public static Validator<T> CountryIso2<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.CountryIso2(messageFunc));

    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-3 country code (e.g. <c>USA</c>).</summary>
    public static Validator<T> CountryIso3<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.CountryIso3<T>());
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-3 country code, with a custom message.</summary>
    public static Validator<T> CountryIso3<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.CountryIso3<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 alpha-3 country code, with a message built from the failing value.</summary>
    public static Validator<T> CountryIso3<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.CountryIso3(messageFunc));

    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 numeric country code (e.g. <c>840</c>).</summary>
    public static Validator<T> CountryIsoNumeric<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.CountryIsoNumeric<T>());
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 numeric country code, with a custom message.</summary>
    public static Validator<T> CountryIsoNumeric<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.CountryIsoNumeric<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid ISO 3166-1 numeric country code, with a message built from the failing value.</summary>
    public static Validator<T> CountryIsoNumeric<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.CountryIsoNumeric(messageFunc));

    /// <summary>Adds a rule requiring a string to be a valid ISO 4217 currency code (e.g. <c>USD</c>).</summary>
    public static Validator<T> CurrencyIso<T>(this Validator<T> validator) => validator.AddRule(ValidatorRules.CurrencyIso<T>());
    /// <summary>Adds a rule requiring a string to be a valid ISO 4217 currency code, with a custom message.</summary>
    public static Validator<T> CurrencyIso<T>(this Validator<T> validator, string message) => validator.AddRule(ValidatorRules.CurrencyIso<T>(message));
    /// <summary>Adds a rule requiring a string to be a valid ISO 4217 currency code, with a message built from the failing value.</summary>
    public static Validator<T> CurrencyIso<T>(this Validator<T> validator, Func<T, string> messageFunc) => validator.AddRule(ValidatorRules.CurrencyIso(messageFunc));

    // =============================
    // Custom predicate escape hatch
    // =============================

    /// <summary>Adds a rule from a custom predicate and failure message. The escape hatch for checks the built-in catalogue doesn't cover.</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message produced when the predicate fails.</param>
    public static Validator<T> Must<T>(this Validator<T> validator, Func<T, bool> predicate, string message)
        => validator.AddRule(new ValidatorRule<T>((value, _) => ValueTask.FromResult(predicate(value)), message));

    /// <summary>Adds a rule from a custom predicate and a message built from the failing value.</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="messageFunc">Builds the message when the predicate fails; invoked only on failure, with the value that failed.</param>
    public static Validator<T> Must<T>(this Validator<T> validator, Func<T, bool> predicate, Func<T, string> messageFunc)
        => validator.AddRule(new ValidatorRule<T>((value, _) => ValueTask.FromResult(predicate(value)), messageFunc));

    /// <summary>Adds a rule from a custom asynchronous predicate and failure message, with cancellation support. The escape hatch for checks that require I/O (e.g. a database or HTTP lookup).</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid; receives the <see cref="CancellationToken"/> passed to <c>ValidateAsync</c>.</param>
    /// <param name="message">The message produced when the predicate fails.</param>
    public static Validator<T> MustAsync<T>(this Validator<T> validator, Func<T, CancellationToken, Task<bool>> predicate, string message)
        => validator.AddRule(new ValidatorRule<T>((value, token) => new ValueTask<bool>(predicate(value, token)), message));

    /// <summary>Adds a rule from a custom asynchronous predicate, with cancellation support and a message built from the failing value.</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid; receives the <see cref="CancellationToken"/> passed to <c>ValidateAsync</c>.</param>
    /// <param name="messageFunc">Builds the message when the predicate fails; invoked only on failure, with the value that failed.</param>
    public static Validator<T> MustAsync<T>(this Validator<T> validator, Func<T, CancellationToken, Task<bool>> predicate, Func<T, string> messageFunc)
        => validator.AddRule(new ValidatorRule<T>((value, token) => new ValueTask<bool>(predicate(value, token)), messageFunc));

    /// <summary>Adds a rule from a custom asynchronous predicate and failure message.</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message produced when the predicate fails.</param>
    public static Validator<T> MustAsync<T>(this Validator<T> validator, Func<T, Task<bool>> predicate, string message)
        => validator.AddRule(new ValidatorRule<T>((value, _) => new ValueTask<bool>(predicate(value)), message));

    /// <summary>Adds a rule from a custom asynchronous predicate and a message built from the failing value.</summary>
    /// <param name="validator">The validator to add the rule to.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="messageFunc">Builds the message when the predicate fails; invoked only on failure, with the value that failed.</param>
    public static Validator<T> MustAsync<T>(this Validator<T> validator, Func<T, Task<bool>> predicate, Func<T, string> messageFunc)
        => validator.AddRule(new ValidatorRule<T>((value, _) => new ValueTask<bool>(predicate(value)), messageFunc));
}
