using System.Globalization;

namespace Izi.FluentData.Validation;

/// <summary>Builds the fixed messages the catalogue stores on a rule.</summary>
internal static class RuleText
{
    /// <summary>Rejects a null message.</summary>
    /// <param name="message">The message supplied by the caller.</param>
    /// <returns>The same message.</returns>
    internal static string Required(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return message;
    }

    /// <summary>Formats a bound with the invariant culture so the message does not depend on the current culture.</summary>
    /// <param name="value">The bound stored in the message.</param>
    /// <returns>The text of the bound.</returns>
    internal static string Format(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        return value.ToString() ?? string.Empty;
    }
}
