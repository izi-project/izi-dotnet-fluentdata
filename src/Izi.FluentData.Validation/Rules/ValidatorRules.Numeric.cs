namespace Izi.FluentData.Validation;

/// <summary>Range and decimal scale rules.</summary>
public static partial class ValidatorRules
{
    /// <summary>Requires the value to fall inside the inclusive range.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <returns>The rule. The message is "Must be between {min} and {max}."</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static IValidatorRule<T> Range<T>(T min, T max)
        where T : IComparable<T>
        => Range(min, max, "Must be between " + RuleText.Format(min) + " and " + RuleText.Format(max) + ".");

    /// <summary>Requires the value to fall inside the inclusive range.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is outside the range. Null fails.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> Range<T>(T min, T max, string message)
        where T : IComparable<T>
    {
        RequireOrdered(min, max);
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.InRange(value, min, max), message);
    }

    /// <summary>Requires the value to fall outside the inclusive range.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <returns>The rule. The message is "Must not be between {min} and {max}."</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static IValidatorRule<T> NotRange<T>(T min, T max)
        where T : IComparable<T>
        => NotRange(min, max, "Must not be between " + RuleText.Format(min) + " and " + RuleText.Format(max) + ".");

    /// <summary>Requires the value to fall outside the inclusive range.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is inside the range. Null fails.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> NotRange<T>(T min, T max, string message)
        where T : IComparable<T>
    {
        RequireOrdered(min, max);
        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => Order.Outside(value, min, max), message);
    }

    /// <summary>
    /// Requires a decimal whose scale is at most <paramref name="maxScale"/> and whose integer-part
    /// digit count is at most <paramref name="maxPrecision"/>. This is the 1.x meaning: precision is not
    /// the SQL total. <c>12345.67</c> has five integer digits and a scale of 2. Values whose absolute
    /// value is below 1 count as one integer digit. A non-decimal fails.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="maxScale">The inclusive maximum number of digits after the decimal point.</param>
    /// <param name="maxPrecision">The inclusive maximum number of digits before the decimal point.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxScale"/> or <paramref name="maxPrecision"/> is negative.</exception>
    public static IValidatorRule<T> ScalePrecision<T>(int maxScale, int maxPrecision)
        => ScalePrecision<T>(
            maxScale,
            maxPrecision,
            "Must have a scale of at most " + maxScale.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + " and a precision of at most " + maxPrecision.ToString(System.Globalization.CultureInfo.InvariantCulture) + ".");

    /// <summary>Requires a decimal to respect <paramref name="maxScale"/> and <paramref name="maxPrecision"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="maxScale">The inclusive maximum number of digits after the decimal point.</param>
    /// <param name="maxPrecision">The inclusive maximum number of digits before the decimal point.</param>
    /// <param name="message">The message used when the decimal does not fit, or the value is not a decimal.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxScale"/> or <paramref name="maxPrecision"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static IValidatorRule<T> ScalePrecision<T>(int maxScale, int maxPrecision, string message)
    {
        if (maxScale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxScale), maxScale, "Scale must not be negative.");
        }

        if (maxPrecision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPrecision), maxPrecision, "Precision must not be negative.");
        }

        message = RuleText.Required(message);
        return new ValidatorRule<T>(value => value is decimal number && Fits(number, maxScale, maxPrecision), message);
    }

    private static void RequireOrdered<T>(T min, T max)
        where T : IComparable<T>
    {
        Order.RejectBound(min, nameof(min));
        Order.RejectBound(max, nameof(max));
        if (Order.CompareBounds(min, max) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "The minimum must not be greater than the maximum.");
        }
    }

    private static bool Fits(decimal number, int maxScale, int maxPrecision)
    {
        if (number.Scale > maxScale)
        {
            return false;
        }

        return IntegerDigits(number) <= maxPrecision;
    }

    private static int IntegerDigits(decimal number)
    {
        var value = decimal.Truncate(decimal.Abs(number));
        if (value < 1m)
        {
            return 1;
        }

        var digits = 0;
        while (value >= 1m)
        {
            value = decimal.Truncate(value / 10m);
            digits++;
        }

        return digits;
    }
}
