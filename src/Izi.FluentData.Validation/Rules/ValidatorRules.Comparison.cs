namespace Izi.FluentData.Validation;

/// <summary>Equality and ordering rules.</summary>
public static partial class ValidatorRules
{
    /// <summary>Requires the value to equal <paramref name="expected"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="expected">The required value.</param>
    /// <returns>The rule. The message is "Must be equal to {expected}."</returns>
    public static IValidatorRule<T> Equal<T>(T expected)
        => Equal(expected, "Must be equal to " + RuleText.Format(expected) + ".");

    /// <summary>Requires the value to equal <paramref name="expected"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="expected">The required value.</param>
    /// <param name="message">The message used when the value differs.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Equal<T>(T expected, string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => EqualityComparer<T>.Default.Equals(value, expected), message);
    }

    /// <summary>Requires the value to differ from <paramref name="unexpected"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="unexpected">The rejected value.</param>
    /// <returns>The rule. The message is "Must not be equal to {unexpected}."</returns>
    public static IValidatorRule<T> NotEqual<T>(T unexpected)
        => NotEqual(unexpected, "Must not be equal to " + RuleText.Format(unexpected) + ".");

    /// <summary>Requires the value to differ from <paramref name="unexpected"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="unexpected">The rejected value.</param>
    /// <param name="message">The message used when the value matches.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> NotEqual<T>(T unexpected, string message)
    {
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => !EqualityComparer<T>.Default.Equals(value, unexpected), message);
    }

    /// <summary>Requires the value to be strictly less than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The exclusive upper bound.</param>
    /// <returns>The rule. The message is "Must be less than {threshold}."</returns>
    public static IValidatorRule<T> LessThan<T>(T threshold)
        where T : IComparable<T>
        => LessThan(threshold, "Must be less than " + RuleText.Format(threshold) + ".");

    /// <summary>Requires the value to be strictly less than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The exclusive upper bound.</param>
    /// <param name="message">The message used when the value is not less.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> LessThan<T>(T threshold, string message)
        where T : IComparable<T>
    {
        Order.RejectBound(threshold, nameof(threshold));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.LessThan(value, threshold), message);
    }

    /// <summary>Requires the value to be less than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The inclusive upper bound.</param>
    /// <returns>The rule. The message is "Must be less than or equal to {threshold}."</returns>
    public static IValidatorRule<T> LessThanOrEqual<T>(T threshold)
        where T : IComparable<T>
        => LessThanOrEqual(threshold, "Must be less than or equal to " + RuleText.Format(threshold) + ".");

    /// <summary>Requires the value to be less than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is greater.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> LessThanOrEqual<T>(T threshold, string message)
        where T : IComparable<T>
    {
        Order.RejectBound(threshold, nameof(threshold));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.LessThanOrEqual(value, threshold), message);
    }

    /// <summary>Requires the value to be strictly greater than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <returns>The rule. The message is "Must be greater than {threshold}."</returns>
    public static IValidatorRule<T> GreaterThan<T>(T threshold)
        where T : IComparable<T>
        => GreaterThan(threshold, "Must be greater than " + RuleText.Format(threshold) + ".");

    /// <summary>Requires the value to be strictly greater than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <param name="message">The message used when the value is not greater.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> GreaterThan<T>(T threshold, string message)
        where T : IComparable<T>
    {
        Order.RejectBound(threshold, nameof(threshold));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.GreaterThan(value, threshold), message);
    }

    /// <summary>Requires the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The inclusive lower bound.</param>
    /// <returns>The rule. The message is "Must be greater than or equal to {threshold}."</returns>
    public static IValidatorRule<T> GreaterThanOrEqual<T>(T threshold)
        where T : IComparable<T>
        => GreaterThanOrEqual(threshold, "Must be greater than or equal to " + RuleText.Format(threshold) + ".");

    /// <summary>Requires the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="threshold">The inclusive lower bound.</param>
    /// <param name="message">The message used when the value is smaller.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> GreaterThanOrEqual<T>(T threshold, string message)
        where T : IComparable<T>
    {
        Order.RejectBound(threshold, nameof(threshold));
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.GreaterThanOrEqual(value, threshold), message);
    }
}
