using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation;

/// <summary>Length and pattern rules.</summary>
public static partial class ValidatorRules
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Requires a string or collection to have length <paramref name="expectedLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="expectedLength">The required length. Zero is allowed.</param>
    /// <returns>The rule. The message is "Must be {n} characters."</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expectedLength"/> is negative.</exception>
    public static IValidatorRule<T> Length<T>(int expectedLength)
        => Length<T>(expectedLength, "Must be " + expectedLength.ToString(System.Globalization.CultureInfo.InvariantCulture) + " characters.");

    /// <summary>Requires a string or collection to have length <paramref name="expectedLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="expectedLength">The required length. Zero is allowed.</param>
    /// <param name="message">The message used when the length differs. Null fails.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expectedLength"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Length<T>(int expectedLength, string message)
    {
        RequireLength(expectedLength, nameof(expectedLength));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => CountOf(value) == expectedLength, message);
    }

    /// <summary>Requires a string or collection to have length of at least <paramref name="minLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="minLength">The inclusive minimum. Zero is allowed.</param>
    /// <returns>The rule. The message is "Must be at least {n} characters."</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> is negative.</exception>
    public static IValidatorRule<T> MinLength<T>(int minLength)
        => MinLength<T>(minLength, "Must be at least " + minLength.ToString(System.Globalization.CultureInfo.InvariantCulture) + " characters.");

    /// <summary>Requires a string or collection to have length of at least <paramref name="minLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="minLength">The inclusive minimum. Zero is allowed.</param>
    /// <param name="message">The message used when the value is shorter. Null fails.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> MinLength<T>(int minLength, string message)
    {
        RequireLength(minLength, nameof(minLength));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => CountOf(value) >= minLength, message);
    }

    /// <summary>Requires a string or collection to have length of at most <paramref name="maxLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="maxLength">The inclusive maximum. Zero is allowed.</param>
    /// <returns>The rule. The message is "Must be at most {n} characters."</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    public static IValidatorRule<T> MaxLength<T>(int maxLength)
        => MaxLength<T>(maxLength, "Must be at most " + maxLength.ToString(System.Globalization.CultureInfo.InvariantCulture) + " characters.");

    /// <summary>Requires a string or collection to have length of at most <paramref name="maxLength"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="maxLength">The inclusive maximum. Zero is allowed.</param>
    /// <param name="message">The message used when the value is longer. Null fails.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> MaxLength<T>(int maxLength, string message)
    {
        RequireLength(maxLength, nameof(maxLength));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value =>
        {
            var count = CountOf(value);
            return count >= 0 && count <= maxLength;
        }, message);
    }

    /// <summary>Requires a string to match <paramref name="pattern"/>. The pattern is compiled with a one-second timeout.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pattern">The regular expression.</param>
    /// <returns>The rule. The message is "Must match the required pattern."</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="RegexParseException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    public static IValidatorRule<T> Matches<T>(string pattern)
        => Matches<T>(Compile(pattern), "Must match the required pattern.");

    /// <summary>Requires a string to match <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <param name="message">The message used when the string does not match. Null and non-strings fail.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="RegexParseException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    public static IValidatorRule<T> Matches<T>(string pattern, string message)
        => Matches<T>(Compile(pattern), message);

    /// <summary>Requires a string to match <paramref name="regex"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="regex">The expression. It must have a finite match timeout.</param>
    /// <param name="message">The message used when the string does not match.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="regex"/> has <see cref="Regex.InfiniteMatchTimeout"/>.</exception>
    /// <exception cref="RegexMatchTimeoutException">The match exceeded the timeout. This is a processing failure, not a validation result.</exception>
    public static IValidatorRule<T> Matches<T>(Regex regex, string message)
    {
        RequireTimeout(regex);
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => value is string text && regex.IsMatch(text), message);
    }

    /// <summary>Requires a string not to match <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <returns>The rule. The message is "Must not match the pattern."</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="RegexParseException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    public static IValidatorRule<T> NotMatches<T>(string pattern)
        => NotMatches<T>(Compile(pattern), "Must not match the pattern.");

    /// <summary>Requires a string not to match <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <param name="message">The message used when the string matches. Null and non-strings pass.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="RegexParseException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    public static IValidatorRule<T> NotMatches<T>(string pattern, string message)
        => NotMatches<T>(Compile(pattern), message);

    /// <summary>Requires a string not to match <paramref name="regex"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="regex">The expression. It must have a finite match timeout.</param>
    /// <param name="message">The message used when the string matches.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="regex"/> has <see cref="Regex.InfiniteMatchTimeout"/>.</exception>
    /// <exception cref="RegexMatchTimeoutException">The match exceeded the timeout. This is a processing failure, not a validation result.</exception>
    public static IValidatorRule<T> NotMatches<T>(Regex regex, string message)
    {
        RequireTimeout(regex);
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => value is not string text || !regex.IsMatch(text), message);
    }

    /// <summary>Requires a string to be an email address. The message is "Must be a valid email address."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> Email<T>() => Email<T>("Must be a valid email address.");

    /// <summary>Requires a string to be an email address.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the string is not an email address.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Email<T>(string message) => Matches<T>(ValidationRegex.Email(), message);

    /// <summary>Requires a string to be a credit-card number. The message is "Must be a valid credit card number."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> CreditCard<T>() => CreditCard<T>("Must be a valid credit card number.");

    /// <summary>Requires a string to be a credit-card number.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the string is not a credit-card number.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> CreditCard<T>(string message) => Matches<T>(ValidationRegex.CreditCard(), message);

    private static void RequireLength(int length, string parameterName)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, length, "Length must not be negative.");
        }
    }

    private static Regex Compile(string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        return new Regex(pattern, RegexOptions.None, MatchTimeout);
    }

    private static void RequireTimeout(Regex regex)
    {
        ArgumentNullException.ThrowIfNull(regex);
        if (regex.MatchTimeout == Regex.InfiniteMatchTimeout)
        {
            throw new ArgumentException(
                "The regular expression must have a match timeout. Set MatchTimeout or pass a timeout to the Regex constructor.",
                nameof(regex));
        }
    }

    /// <summary>The length of a string or collection, or -1 when the value is null or neither.</summary>
    private static int CountOf<T>(T value)
    {
        if (value is null)
        {
            return -1;
        }

        if (value is string text)
        {
            return text.Length;
        }

        if (value is System.Collections.ICollection collection)
        {
            return collection.Count;
        }

        if (value is System.Collections.IEnumerable enumerable)
        {
            var count = 0;
            var enumerator = enumerable.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            return count;
        }

        return -1;
    }
}

/// <summary>Source-generated patterns reused by the format rules. Each pattern has a one-second timeout.</summary>
internal static partial class ValidationRegex
{
    /// <summary>A simple email-shaped pattern.</summary>
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+\z", RegexOptions.None, 1000)]
    internal static partial Regex Email();

    /// <summary>A pattern matching the common credit-card number formats.</summary>
    [GeneratedRegex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})\z", RegexOptions.None, 1000)]
    internal static partial Regex CreditCard();
}
