using System.Globalization;
using System.Text.RegularExpressions;

namespace Izi.FluentData.Transformer;

public static partial class TransformerRules
{
    /// <summary>
    /// Timeout applied to <see cref="Regex"/> instances this class creates from a pattern.
    /// A <see cref="Regex"/> supplied by the caller keeps that instance's own timeout.
    /// </summary>
    private static readonly TimeSpan CreatedRegexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Trims both ends. A null string becomes empty before trimming, so the result is empty.</summary>
    public static TransformerRule<string, string> Trim() => Text(value => value.Trim());

    /// <summary>Trims the start. A null string becomes empty.</summary>
    public static TransformerRule<string, string> TrimStart() => Text(value => value.TrimStart());

    /// <summary>Trims the end. A null string becomes empty.</summary>
    public static TransformerRule<string, string> TrimEnd() => Text(value => value.TrimEnd());

    /// <summary>Uppercases with <see cref="CultureInfo.InvariantCulture"/>.</summary>
    public static TransformerRule<string, string> ToUpper() => ToUpper(CultureInfo.InvariantCulture);

    /// <summary>Uppercases with <paramref name="culture"/>. A null string becomes empty.</summary>
    public static TransformerRule<string, string> ToUpper(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return Text(value => value.ToUpper(culture));
    }

    /// <summary>Lowercases with <see cref="CultureInfo.InvariantCulture"/>.</summary>
    public static TransformerRule<string, string> ToLower() => ToLower(CultureInfo.InvariantCulture);

    /// <summary>Lowercases with <paramref name="culture"/>. A null string becomes empty.</summary>
    public static TransformerRule<string, string> ToLower(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return Text(value => value.ToLower(culture));
    }

    /// <summary>
    /// Title-cases with <see cref="CultureInfo.InvariantCulture"/>.
    /// This is <see cref="TextInfo.ToTitleCase"/>, which does not lowercase characters that are already upper.
    /// </summary>
    public static TransformerRule<string, string> ToTitleCase() => ToTitleCase(CultureInfo.InvariantCulture);

    /// <summary>Title-cases with <paramref name="culture"/>. A null string becomes empty.</summary>
    public static TransformerRule<string, string> ToTitleCase(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return Text(culture.TextInfo.ToTitleCase);
    }

    /// <summary>Replaces every <paramref name="oldValue"/> with <paramref name="newValue"/>. A null string becomes empty.</summary>
    public static TransformerRule<string, string> Replace(string oldValue, string newValue)
    {
        ArgumentNullException.ThrowIfNull(oldValue);
        ArgumentNullException.ThrowIfNull(newValue);
        return Text(value => value.Replace(oldValue, newValue, StringComparison.Ordinal));
    }

    /// <summary>Replaces matches of a caller-supplied <paramref name="regex"/>. That regex keeps its own timeout and options.</summary>
    public static TransformerRule<string, string> ReplaceRegex(Regex regex, string replacement)
    {
        ArgumentNullException.ThrowIfNull(regex);
        ArgumentNullException.ThrowIfNull(replacement);
        return Text(value => regex.Replace(value, replacement));
    }

    /// <summary>
    /// Compiles <paramref name="pattern"/> with a one-second match timeout and replaces its matches.
    /// A null string becomes empty.
    /// </summary>
    public static TransformerRule<string, string> ReplaceRegex(string pattern, string replacement)
        => ReplaceRegex(pattern, replacement, RegexOptions.None);

    /// <summary>Compiles <paramref name="pattern"/> with <paramref name="options"/> and a one-second match timeout.</summary>
    public static TransformerRule<string, string> ReplaceRegex(string pattern, string replacement, RegexOptions options)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(replacement);
        var regex = new Regex(pattern, options, CreatedRegexTimeout);
        return ReplaceRegex(regex, replacement);
    }

    /// <summary>Removes every whitespace run, including runs in the middle of the text. A null string becomes empty.</summary>
    public static TransformerRule<string, string> RemoveWhitespace()
        => Text(value => WhitespacePattern().Replace(value, string.Empty));

    /// <summary>
    /// Collapses every whitespace run to a single space. Leading and trailing runs become one space;
    /// this does not trim. A null string becomes empty.
    /// </summary>
    public static TransformerRule<string, string> CollapseWhitespace()
        => Text(value => WhitespacePattern().Replace(value, " "));

    /// <summary>Returns the tail starting at <paramref name="startIndex"/>. A null string becomes empty, then the range is applied.</summary>
    public static TransformerRule<string, string> Substring(int startIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        return Text(value => value[startIndex..]);
    }

    /// <summary>Returns <paramref name="length"/> characters from <paramref name="startIndex"/>. A null string becomes empty first.</summary>
    public static TransformerRule<string, string> Substring(int startIndex, int length)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        return Text(value => value.Substring(startIndex, length));
    }

    /// <summary>
    /// Keeps at most <paramref name="maxLength"/> characters. A null or empty string stays empty.
    /// A negative length is rejected when the rule is created. The previous catalogue returned empty instead.
    /// </summary>
    public static TransformerRule<string, string> Truncate(int maxLength)
    {
        if (maxLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength));

        return Text(value => value.Length <= maxLength ? value : value[..maxLength]);
    }

    /// <summary>Pads on the left. A null string becomes empty. A negative width is rejected when the rule is created.</summary>
    public static TransformerRule<string, string> PadLeft(int totalWidth, char paddingChar)
    {
        if (totalWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(totalWidth));

        return Text(value => value.PadLeft(totalWidth, paddingChar));
    }

    /// <summary>Pads on the right. A null string becomes empty. A negative width is rejected when the rule is created.</summary>
    public static TransformerRule<string, string> PadRight(int totalWidth, char paddingChar)
    {
        if (totalWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(totalWidth));

        return Text(value => value.PadRight(totalWidth, paddingChar));
    }

    /// <summary>Inserts <paramref name="prefix"/> before the text. A null source or prefix is treated as empty.</summary>
    public static TransformerRule<string, string> Prepend(string? prefix)
    {
        var captured = prefix ?? string.Empty;
        return Text(value => captured + value);
    }

    /// <summary>Appends <paramref name="suffix"/>. A null source or suffix is treated as empty.</summary>
    public static TransformerRule<string, string> Append(string? suffix)
    {
        var captured = suffix ?? string.Empty;
        return Text(value => value + captured);
    }

    [GeneratedRegex(@"\s+", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex WhitespacePattern();

    private static TransformerRule<string, string> Text(Func<string, string> map)
        => Same<string>(value => map(value ?? string.Empty));
}
