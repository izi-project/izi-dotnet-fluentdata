using System.Numerics;

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
    /// <summary>Appends the catalogue DefaultIf rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIf<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue, Func<TProperty, bool> predicate)
        => rule.Add(TransformerRules.DefaultIf(defaultValue, predicate));

    /// <summary>Appends the catalogue DefaultIf rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIf<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty defaultValue, Func<TProperty, bool> predicate)
        => conversion.Then(TransformerRules.DefaultIf(defaultValue, predicate));

    /// <summary>Appends the catalogue DefaultIfNull rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIfNull<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        => rule.Add(TransformerRules.DefaultIfNull(defaultValue));

    /// <summary>Appends the catalogue DefaultIfNull rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfNull<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty defaultValue)
        => conversion.Then(TransformerRules.DefaultIfNull(defaultValue));

    /// <summary>Appends the catalogue DefaultIfEmpty rule.</summary>
    public static PropertyRule<T, TProperty> DefaultIfEmpty<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        => rule.Add(TransformerRules.DefaultIfEmpty(defaultValue));

    /// <summary>Appends the catalogue DefaultIfEmpty rule.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfEmpty<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty defaultValue)
        => conversion.Then(TransformerRules.DefaultIfEmpty(defaultValue));

    /// <summary>Appends the catalogue DefaultIfNullOrWhitespace rule.</summary>
    public static PropertyRule<T, string> DefaultIfNullOrWhitespace<T>(this PropertyRule<T, string> rule, string defaultValue)
        => rule.Add(TransformerRules.DefaultIfNullOrWhitespace(defaultValue));

    /// <summary>Appends the catalogue DefaultIfNullOrWhitespace rule.</summary>
    public static PropertyConversion<T, TIn, string> DefaultIfNullOrWhitespace<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string defaultValue)
        => conversion.Then(TransformerRules.DefaultIfNullOrWhitespace(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for the type minimum.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMinValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for the type minimum.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfMinValue<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for the type maximum.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMaxValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for the type maximum.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfMaxValue<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty defaultValue)
        where TProperty : IComparable<TProperty>, IMinMaxValue<TProperty>
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for a threshold.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMinValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty minimum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => rule.Add(TransformerRules.DefaultIfMinValue(minimum, defaultValue));

    /// <summary>Appends the catalogue DefaultIfMinValue rule for a threshold.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfMinValue<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty minimum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => conversion.Then(TransformerRules.DefaultIfMinValue(minimum, defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for a threshold.</summary>
    public static PropertyRule<T, TProperty> DefaultIfMaxValue<T, TProperty>(this PropertyRule<T, TProperty> rule, TProperty maximum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => rule.Add(TransformerRules.DefaultIfMaxValue(maximum, defaultValue));

    /// <summary>Appends the catalogue DefaultIfMaxValue rule for a threshold.</summary>
    public static PropertyConversion<T, TIn, TProperty> DefaultIfMaxValue<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, TProperty maximum, TProperty defaultValue)
        where TProperty : IComparable<TProperty>
        => conversion.Then(TransformerRules.DefaultIfMaxValue(maximum, defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateTime> DefaultIfMinValue<T>(this PropertyRule<T, DateTime> rule, DateTime defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMinValue rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> DefaultIfMinValue<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion, DateTime defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateTime> DefaultIfMaxValue<T>(this PropertyRule<T, DateTime> rule, DateTime defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateTime DefaultIfMaxValue rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> DefaultIfMaxValue<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion, DateTime defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateTimeOffset> DefaultIfMinValue<T>(this PropertyRule<T, DateTimeOffset> rule, DateTimeOffset defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMinValue rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> DefaultIfMinValue<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion, DateTimeOffset defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateTimeOffset> DefaultIfMaxValue<T>(this PropertyRule<T, DateTimeOffset> rule, DateTimeOffset defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateTimeOffset DefaultIfMaxValue rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> DefaultIfMaxValue<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion, DateTimeOffset defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, TimeSpan> DefaultIfMinValue<T>(this PropertyRule<T, TimeSpan> rule, TimeSpan defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMinValue rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> DefaultIfMinValue<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion, TimeSpan defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, TimeSpan> DefaultIfMaxValue<T>(this PropertyRule<T, TimeSpan> rule, TimeSpan defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeSpan DefaultIfMaxValue rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> DefaultIfMaxValue<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion, TimeSpan defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, DateOnly> DefaultIfMinValue<T>(this PropertyRule<T, DateOnly> rule, DateOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMinValue rule.</summary>
    public static PropertyConversion<T, TIn, DateOnly> DefaultIfMinValue<T, TIn>(this PropertyConversion<T, TIn, DateOnly> conversion, DateOnly defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, DateOnly> DefaultIfMaxValue<T>(this PropertyRule<T, DateOnly> rule, DateOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue DateOnly DefaultIfMaxValue rule.</summary>
    public static PropertyConversion<T, TIn, DateOnly> DefaultIfMaxValue<T, TIn>(this PropertyConversion<T, TIn, DateOnly> conversion, DateOnly defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMinValue rule.</summary>
    public static PropertyRule<T, TimeOnly> DefaultIfMinValue<T>(this PropertyRule<T, TimeOnly> rule, TimeOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMinValue rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> DefaultIfMinValue<T, TIn>(this PropertyConversion<T, TIn, TimeOnly> conversion, TimeOnly defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMinValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMaxValue rule.</summary>
    public static PropertyRule<T, TimeOnly> DefaultIfMaxValue<T>(this PropertyRule<T, TimeOnly> rule, TimeOnly defaultValue)
        => rule.Add(TransformerRules.DefaultIfMaxValue(defaultValue));

    /// <summary>Appends the catalogue TimeOnly DefaultIfMaxValue rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> DefaultIfMaxValue<T, TIn>(this PropertyConversion<T, TIn, TimeOnly> conversion, TimeOnly defaultValue)
        => conversion.Then(TransformerRules.DefaultIfMaxValue(defaultValue));
}
