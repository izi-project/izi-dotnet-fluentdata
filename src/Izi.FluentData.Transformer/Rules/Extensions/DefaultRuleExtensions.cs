using System.Numerics;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a same-type catalogue rule and return the property rule.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue DefaultIf rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIf<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue, Func<TProperty, bool> predicate)
        => rule.Add(TransformerRules.DefaultIf(defaultValue, predicate));

    /// <summary>Appends the catalogue DefaultIfNull rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIfNull<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        => rule.Add(TransformerRules.DefaultIfNull(defaultValue));

    /// <summary>Appends the catalogue DefaultIfEmpty rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIfEmpty<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        => rule.Add(TransformerRules.DefaultIfEmpty(defaultValue));

    /// <summary>Appends the catalogue DefaultIfNullOrWhitespace rule.</summary>
    public static PropertyRule<T, string> DefaultIfNullOrWhitespace<T>(this PropertyRule<T, string> rule, string defaultValue)
        => rule.Add(TransformerRules.DefaultIfNullOrWhitespace(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for the type minimum.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMinValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for the type maximum.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMaxValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for a threshold.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMinValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty minimum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => rule.Add(TransformerRules.DefaultIfMinValue(minimum, defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for a threshold.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMaxValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty maximum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => rule.Add(TransformerRules.DefaultIfMaxValue(maximum, defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateTime> DefaultIfMinValue<T>(this PropertyRule<T, DateTime> rule, DateTime defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateTime> DefaultIfMaxValue<T>(this PropertyRule<T, DateTime> rule, DateTime defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateTimeOffset> DefaultIfMinValue<T>(this PropertyRule<T, DateTimeOffset> rule, DateTimeOffset defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateTimeOffset> DefaultIfMaxValue<T>(this PropertyRule<T, DateTimeOffset> rule, DateTimeOffset defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, TimeSpan> DefaultIfMinValue<T>(this PropertyRule<T, TimeSpan> rule, TimeSpan defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, TimeSpan> DefaultIfMaxValue<T>(this PropertyRule<T, TimeSpan> rule, TimeSpan defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateOnly> DefaultIfMinValue<T>(this PropertyRule<T, DateOnly> rule, DateOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateOnly> DefaultIfMaxValue<T>(this PropertyRule<T, DateOnly> rule, DateOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, TimeOnly> DefaultIfMinValue<T>(this PropertyRule<T, TimeOnly> rule, TimeOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, TimeOnly> DefaultIfMaxValue<T>(this PropertyRule<T, TimeOnly> rule, TimeOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

}
