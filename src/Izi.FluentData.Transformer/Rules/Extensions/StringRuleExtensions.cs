using System.Globalization;
using System.Text.RegularExpressions;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a same-type catalogue rule and return the property transformer.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue Trim rule.</summary>
    public static PropertyTransformer<T, string> Trim<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.Trim);

    /// <summary>Appends the catalogue TrimStart rule.</summary>
    public static PropertyTransformer<T, string> TrimStart<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.TrimStart);

    /// <summary>Appends the catalogue TrimEnd rule.</summary>
    public static PropertyTransformer<T, string> TrimEnd<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.TrimEnd);

    /// <summary>Appends the catalogue invariant ToUpper rule.</summary>
    public static PropertyTransformer<T, string> ToUpper<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.ToUpper);

    /// <summary>Appends the catalogue invariant ToLower rule.</summary>
    public static PropertyTransformer<T, string> ToLower<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.ToLower);

    /// <summary>Appends the catalogue invariant ToTitleCase rule.</summary>
    public static PropertyTransformer<T, string> ToTitleCase<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.ToTitleCase);

    /// <summary>Appends the catalogue RemoveWhitespace rule.</summary>
    public static PropertyTransformer<T, string> RemoveWhitespace<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.RemoveWhitespace);

    /// <summary>Appends the catalogue CollapseWhitespace rule.</summary>
    public static PropertyTransformer<T, string> CollapseWhitespace<T>(this PropertyTransformer<T, string> rule)
        => rule.Add(StringRules.CollapseWhitespace);

    /// <summary>Appends the catalogue ToUpper rule for a culture.</summary>
    public static PropertyTransformer<T, string> ToUpper<T>(this PropertyTransformer<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToUpper(culture));

    /// <summary>Appends the catalogue ToLower rule for a culture.</summary>
    public static PropertyTransformer<T, string> ToLower<T>(this PropertyTransformer<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToLower(culture));

    /// <summary>Appends the catalogue ToTitleCase rule for a culture.</summary>
    public static PropertyTransformer<T, string> ToTitleCase<T>(this PropertyTransformer<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToTitleCase(culture));

    /// <summary>Appends the catalogue Replace rule.</summary>
    public static PropertyTransformer<T, string> Replace<T>(this PropertyTransformer<T, string> rule, string oldValue, string newValue)
        => rule.Add(TransformerRules.Replace(oldValue, newValue));

    /// <summary>Appends the catalogue ReplaceRegex rule for a compiled expression.</summary>
    public static PropertyTransformer<T, string> ReplaceRegex<T>(this PropertyTransformer<T, string> rule, Regex regex, string replacement)
        => rule.Add(TransformerRules.ReplaceRegex(regex, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern.</summary>
    public static PropertyTransformer<T, string> ReplaceRegex<T>(this PropertyTransformer<T, string> rule, string pattern, string replacement)
        => rule.Add(TransformerRules.ReplaceRegex(pattern, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern and options.</summary>
    public static PropertyTransformer<T, string> ReplaceRegex<T>(this PropertyTransformer<T, string> rule, string pattern, string replacement, RegexOptions options)
        => rule.Add(TransformerRules.ReplaceRegex(pattern, replacement, options));

    /// <summary>Appends the catalogue Substring rule from an index.</summary>
    public static PropertyTransformer<T, string> Substring<T>(this PropertyTransformer<T, string> rule, int startIndex)
        => rule.Add(TransformerRules.Substring(startIndex));

    /// <summary>Appends the catalogue Substring rule for an index and length.</summary>
    public static PropertyTransformer<T, string> Substring<T>(this PropertyTransformer<T, string> rule, int startIndex, int length)
        => rule.Add(TransformerRules.Substring(startIndex, length));

    /// <summary>Appends the catalogue string Truncate rule.</summary>
    public static PropertyTransformer<T, string> Truncate<T>(this PropertyTransformer<T, string> rule, int maxLength)
        => rule.Add(TransformerRules.Truncate(maxLength));

    /// <summary>Appends the catalogue PadLeft rule.</summary>
    public static PropertyTransformer<T, string> PadLeft<T>(this PropertyTransformer<T, string> rule, int totalWidth, char paddingChar)
        => rule.Add(TransformerRules.PadLeft(totalWidth, paddingChar));

    /// <summary>Appends the catalogue PadRight rule.</summary>
    public static PropertyTransformer<T, string> PadRight<T>(this PropertyTransformer<T, string> rule, int totalWidth, char paddingChar)
        => rule.Add(TransformerRules.PadRight(totalWidth, paddingChar));

    /// <summary>Appends the catalogue Prepend rule.</summary>
    public static PropertyTransformer<T, string> Prepend<T>(this PropertyTransformer<T, string> rule, string? prefix)
        => rule.Add(TransformerRules.Prepend(prefix));

    /// <summary>Appends the catalogue Append rule.</summary>
    public static PropertyTransformer<T, string> Append<T>(this PropertyTransformer<T, string> rule, string? suffix)
        => rule.Add(TransformerRules.Append(suffix));

    private static class StringRules
    {
        public static readonly TransformerRule<string> Trim = TransformerRules.Trim();
        public static readonly TransformerRule<string> TrimStart = TransformerRules.TrimStart();
        public static readonly TransformerRule<string> TrimEnd = TransformerRules.TrimEnd();
        public static readonly TransformerRule<string> ToUpper = TransformerRules.ToUpper();
        public static readonly TransformerRule<string> ToLower = TransformerRules.ToLower();
        public static readonly TransformerRule<string> ToTitleCase = TransformerRules.ToTitleCase();
        public static readonly TransformerRule<string> RemoveWhitespace = TransformerRules.RemoveWhitespace();
        public static readonly TransformerRule<string> CollapseWhitespace = TransformerRules.CollapseWhitespace();
    }
}
