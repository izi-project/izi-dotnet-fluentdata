using System.Globalization;
using System.Text.RegularExpressions;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a catalogue rule and return the builder.
/// Same-type steps call <see cref="PropertyRule{T, TProperty}.Add(ITransformerRule{TProperty, TProperty})"/>
/// or <see cref="PropertyConversion{T, TIn, TOut}.Then{TNext}(ITransformerRule{TOut, TNext})"/>.
/// Steps that change type call <see cref="PropertyRule{T, TProperty}.Convert{TOut}(ITransformerRule{TProperty, TOut})"/>
/// or <c>Then</c>.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue Trim rule.</summary>
    public static PropertyRule<T, string> Trim<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.Trim);

    /// <summary>Appends the catalogue Trim rule.</summary>
    public static PropertyConversion<T, TIn, string> Trim<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.Trim);

    /// <summary>Appends the catalogue TrimStart rule.</summary>
    public static PropertyRule<T, string> TrimStart<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.TrimStart);

    /// <summary>Appends the catalogue TrimStart rule.</summary>
    public static PropertyConversion<T, TIn, string> TrimStart<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.TrimStart);

    /// <summary>Appends the catalogue TrimEnd rule.</summary>
    public static PropertyRule<T, string> TrimEnd<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.TrimEnd);

    /// <summary>Appends the catalogue TrimEnd rule.</summary>
    public static PropertyConversion<T, TIn, string> TrimEnd<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.TrimEnd);

    /// <summary>Appends the catalogue invariant ToUpper rule.</summary>
    public static PropertyRule<T, string> ToUpper<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.ToUpper);

    /// <summary>Appends the catalogue invariant ToUpper rule.</summary>
    public static PropertyConversion<T, TIn, string> ToUpper<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.ToUpper);

    /// <summary>Appends the catalogue invariant ToLower rule.</summary>
    public static PropertyRule<T, string> ToLower<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.ToLower);

    /// <summary>Appends the catalogue invariant ToLower rule.</summary>
    public static PropertyConversion<T, TIn, string> ToLower<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.ToLower);

    /// <summary>Appends the catalogue invariant ToTitleCase rule.</summary>
    public static PropertyRule<T, string> ToTitleCase<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.ToTitleCase);

    /// <summary>Appends the catalogue invariant ToTitleCase rule.</summary>
    public static PropertyConversion<T, TIn, string> ToTitleCase<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.ToTitleCase);

    /// <summary>Appends the catalogue RemoveWhitespace rule.</summary>
    public static PropertyRule<T, string> RemoveWhitespace<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.RemoveWhitespace);

    /// <summary>Appends the catalogue RemoveWhitespace rule.</summary>
    public static PropertyConversion<T, TIn, string> RemoveWhitespace<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.RemoveWhitespace);

    /// <summary>Appends the catalogue CollapseWhitespace rule.</summary>
    public static PropertyRule<T, string> CollapseWhitespace<T>(this PropertyRule<T, string> rule)
        => rule.Add(StringRules.CollapseWhitespace);

    /// <summary>Appends the catalogue CollapseWhitespace rule.</summary>
    public static PropertyConversion<T, TIn, string> CollapseWhitespace<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(StringRules.CollapseWhitespace);

    /// <summary>Appends the catalogue ToUpper rule for a culture.</summary>
    public static PropertyRule<T, string> ToUpper<T>(this PropertyRule<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToUpper(culture));

    /// <summary>Appends the catalogue ToUpper rule for a culture.</summary>
    public static PropertyConversion<T, TIn, string> ToUpper<T, TIn>(this PropertyConversion<T, TIn, string> conversion, CultureInfo culture)
        => conversion.Then(TransformerRules.ToUpper(culture));

    /// <summary>Appends the catalogue ToLower rule for a culture.</summary>
    public static PropertyRule<T, string> ToLower<T>(this PropertyRule<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToLower(culture));

    /// <summary>Appends the catalogue ToLower rule for a culture.</summary>
    public static PropertyConversion<T, TIn, string> ToLower<T, TIn>(this PropertyConversion<T, TIn, string> conversion, CultureInfo culture)
        => conversion.Then(TransformerRules.ToLower(culture));

    /// <summary>Appends the catalogue ToTitleCase rule for a culture.</summary>
    public static PropertyRule<T, string> ToTitleCase<T>(this PropertyRule<T, string> rule, CultureInfo culture)
        => rule.Add(TransformerRules.ToTitleCase(culture));

    /// <summary>Appends the catalogue ToTitleCase rule for a culture.</summary>
    public static PropertyConversion<T, TIn, string> ToTitleCase<T, TIn>(this PropertyConversion<T, TIn, string> conversion, CultureInfo culture)
        => conversion.Then(TransformerRules.ToTitleCase(culture));

    /// <summary>Appends the catalogue Replace rule.</summary>
    public static PropertyRule<T, string> Replace<T>(this PropertyRule<T, string> rule, string oldValue, string newValue)
        => rule.Add(TransformerRules.Replace(oldValue, newValue));

    /// <summary>Appends the catalogue Replace rule.</summary>
    public static PropertyConversion<T, TIn, string> Replace<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string oldValue, string newValue)
        => conversion.Then(TransformerRules.Replace(oldValue, newValue));

    /// <summary>Appends the catalogue ReplaceRegex rule for a compiled expression.</summary>
    public static PropertyRule<T, string> ReplaceRegex<T>(this PropertyRule<T, string> rule, Regex regex, string replacement)
        => rule.Add(TransformerRules.ReplaceRegex(regex, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a compiled expression.</summary>
    public static PropertyConversion<T, TIn, string> ReplaceRegex<T, TIn>(this PropertyConversion<T, TIn, string> conversion, Regex regex, string replacement)
        => conversion.Then(TransformerRules.ReplaceRegex(regex, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern.</summary>
    public static PropertyRule<T, string> ReplaceRegex<T>(this PropertyRule<T, string> rule, string pattern, string replacement)
        => rule.Add(TransformerRules.ReplaceRegex(pattern, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern.</summary>
    public static PropertyConversion<T, TIn, string> ReplaceRegex<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string pattern, string replacement)
        => conversion.Then(TransformerRules.ReplaceRegex(pattern, replacement));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern and options.</summary>
    public static PropertyRule<T, string> ReplaceRegex<T>(this PropertyRule<T, string> rule, string pattern, string replacement, RegexOptions options)
        => rule.Add(TransformerRules.ReplaceRegex(pattern, replacement, options));

    /// <summary>Appends the catalogue ReplaceRegex rule for a pattern and options.</summary>
    public static PropertyConversion<T, TIn, string> ReplaceRegex<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string pattern, string replacement, RegexOptions options)
        => conversion.Then(TransformerRules.ReplaceRegex(pattern, replacement, options));

    /// <summary>Appends the catalogue Substring rule from an index.</summary>
    public static PropertyRule<T, string> Substring<T>(this PropertyRule<T, string> rule, int startIndex)
        => rule.Add(TransformerRules.Substring(startIndex));

    /// <summary>Appends the catalogue Substring rule from an index.</summary>
    public static PropertyConversion<T, TIn, string> Substring<T, TIn>(this PropertyConversion<T, TIn, string> conversion, int startIndex)
        => conversion.Then(TransformerRules.Substring(startIndex));

    /// <summary>Appends the catalogue Substring rule for an index and length.</summary>
    public static PropertyRule<T, string> Substring<T>(this PropertyRule<T, string> rule, int startIndex, int length)
        => rule.Add(TransformerRules.Substring(startIndex, length));

    /// <summary>Appends the catalogue Substring rule for an index and length.</summary>
    public static PropertyConversion<T, TIn, string> Substring<T, TIn>(this PropertyConversion<T, TIn, string> conversion, int startIndex, int length)
        => conversion.Then(TransformerRules.Substring(startIndex, length));

    /// <summary>Appends the catalogue string Truncate rule.</summary>
    public static PropertyRule<T, string> Truncate<T>(this PropertyRule<T, string> rule, int maxLength)
        => rule.Add(TransformerRules.Truncate(maxLength));

    /// <summary>Appends the catalogue string Truncate rule.</summary>
    public static PropertyConversion<T, TIn, string> Truncate<T, TIn>(this PropertyConversion<T, TIn, string> conversion, int maxLength)
        => conversion.Then(TransformerRules.Truncate(maxLength));

    /// <summary>Appends the catalogue PadLeft rule.</summary>
    public static PropertyRule<T, string> PadLeft<T>(this PropertyRule<T, string> rule, int totalWidth, char paddingChar)
        => rule.Add(TransformerRules.PadLeft(totalWidth, paddingChar));

    /// <summary>Appends the catalogue PadLeft rule.</summary>
    public static PropertyConversion<T, TIn, string> PadLeft<T, TIn>(this PropertyConversion<T, TIn, string> conversion, int totalWidth, char paddingChar)
        => conversion.Then(TransformerRules.PadLeft(totalWidth, paddingChar));

    /// <summary>Appends the catalogue PadRight rule.</summary>
    public static PropertyRule<T, string> PadRight<T>(this PropertyRule<T, string> rule, int totalWidth, char paddingChar)
        => rule.Add(TransformerRules.PadRight(totalWidth, paddingChar));

    /// <summary>Appends the catalogue PadRight rule.</summary>
    public static PropertyConversion<T, TIn, string> PadRight<T, TIn>(this PropertyConversion<T, TIn, string> conversion, int totalWidth, char paddingChar)
        => conversion.Then(TransformerRules.PadRight(totalWidth, paddingChar));

    /// <summary>Appends the catalogue Prepend rule.</summary>
    public static PropertyRule<T, string> Prepend<T>(this PropertyRule<T, string> rule, string? prefix)
        => rule.Add(TransformerRules.Prepend(prefix));

    /// <summary>Appends the catalogue Prepend rule.</summary>
    public static PropertyConversion<T, TIn, string> Prepend<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string? prefix)
        => conversion.Then(TransformerRules.Prepend(prefix));

    /// <summary>Appends the catalogue Append rule.</summary>
    public static PropertyRule<T, string> Append<T>(this PropertyRule<T, string> rule, string? suffix)
        => rule.Add(TransformerRules.Append(suffix));

    /// <summary>Appends the catalogue Append rule.</summary>
    public static PropertyConversion<T, TIn, string> Append<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string? suffix)
        => conversion.Then(TransformerRules.Append(suffix));

    private static class StringRules
    {
        public static readonly TransformerRule<string, string> Trim = TransformerRules.Trim();
        public static readonly TransformerRule<string, string> TrimStart = TransformerRules.TrimStart();
        public static readonly TransformerRule<string, string> TrimEnd = TransformerRules.TrimEnd();
        public static readonly TransformerRule<string, string> ToUpper = TransformerRules.ToUpper();
        public static readonly TransformerRule<string, string> ToLower = TransformerRules.ToLower();
        public static readonly TransformerRule<string, string> ToTitleCase = TransformerRules.ToTitleCase();
        public static readonly TransformerRule<string, string> RemoveWhitespace = TransformerRules.RemoveWhitespace();
        public static readonly TransformerRule<string, string> CollapseWhitespace = TransformerRules.CollapseWhitespace();
    }
}
