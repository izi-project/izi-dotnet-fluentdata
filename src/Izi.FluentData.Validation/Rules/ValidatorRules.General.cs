namespace Izi.FluentData.Validation;

/// <summary>Null, emptiness, and custom predicate rules.</summary>
public static partial class ValidatorRules
{
    /// <summary>Requires the value to be non-null. The message is "Must not be null."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> NotNull<T>() => NotNull<T>("Must not be null.");

    /// <summary>Requires the value to be non-null.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the value is null.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> NotNull<T>(string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(static value => value is not null, message);
    }

    /// <summary>Requires the value to be null. The message is "Must be null."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> Null<T>() => Null<T>("Must be null.");

    /// <summary>Requires the value to be null.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the value is not null.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Null<T>(string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(static value => value is null, message);
    }

    /// <summary>Requires a non-blank string or a non-empty collection. The message is "Must not be empty."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> NotEmpty<T>() => NotEmpty<T>("Must not be empty.");

    /// <summary>Requires a non-blank string or a non-empty collection. Other types pass. Null fails.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the value is empty.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> NotEmpty<T>(string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(static value => !IsEmpty(value), message);
    }

    /// <summary>Requires a null, blank string, or empty collection. The message is "Must be empty."</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The rule.</returns>
    public static IValidatorRule<T> Empty<T>() => Empty<T>("Must be empty.");

    /// <summary>Requires a null, blank string, or empty collection. Other types fail.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The message used when the value is not empty.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Empty<T>(string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(static value => IsEmpty(value), message);
    }

    /// <summary>Requires <paramref name="predicate"/> to return true.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public static IValidatorRule<T> Must<T>(Func<T, bool> predicate, string message)
        => new ValidatorRule<T>(predicate, RuleText.Required(message));

    /// <summary>Requires <paramref name="predicate"/> to return true.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="predicate">Returns true when the value passes. It receives the cancellation token and returns a task.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public static IValidatorRule<T> MustAsync<T>(Func<T, CancellationToken, Task<bool>> predicate, string message)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return new ValidatorRule<T>((value, token) => new ValueTask<bool>(predicate(value, token)), RuleText.Required(message));
    }

    /// <summary>Requires <paramref name="predicate"/> to return true.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public static IValidatorRule<T> MustAsync<T>(Func<T, Task<bool>> predicate, string message)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return new ValidatorRule<T>((value, _) => new ValueTask<bool>(predicate(value)), RuleText.Required(message));
    }

    /// <summary>True when the value is null, a blank string, or an empty collection.</summary>
    /// <param name="value">The value to inspect.</param>
    /// <returns>True when the value is empty. Other types are not empty.</returns>
    internal static bool IsEmpty<T>(T value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        if (value is System.Collections.ICollection collection)
        {
            return collection.Count == 0;
        }

        if (value is System.Collections.IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        return false;
    }
}
