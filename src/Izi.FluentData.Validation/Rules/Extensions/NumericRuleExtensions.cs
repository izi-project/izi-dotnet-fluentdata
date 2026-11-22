namespace Izi.FluentData.Validation;

/// <summary>Fluent wrappers for the range and decimal scale rules.</summary>
public static class NumericRuleExtensions
{
    /// <summary>Appends a rule that requires the inclusive range.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Range<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty min, TProperty max)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.Range(min, max));

    /// <summary>Appends a rule that requires the inclusive range.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is outside the range.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Range<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty min, TProperty max, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.Range(min, max, message));

    /// <summary>Appends a rule that rejects the inclusive range.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotRange<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty min, TProperty max)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.NotRange(min, max));

    /// <summary>Appends a rule that rejects the inclusive range.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is inside the range.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotRange<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty min, TProperty max, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.NotRange(min, max, message));

    /// <summary>Appends a rule that requires a decimal scale and precision.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="maxScale">The inclusive maximum scale.</param>
    /// <param name="maxPrecision">The inclusive maximum number of integer digits.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> ScalePrecision<T, TProperty>(this PropertyValidator<T, TProperty> property, int maxScale, int maxPrecision)
        => property.AddRule(ValidatorRules.ScalePrecision<TProperty>(maxScale, maxPrecision));

    /// <summary>Appends a rule that requires a decimal scale and precision.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="maxScale">The inclusive maximum scale.</param>
    /// <param name="maxPrecision">The inclusive maximum number of integer digits.</param>
    /// <param name="message">The message used when the value does not fit.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> ScalePrecision<T, TProperty>(this PropertyValidator<T, TProperty> property, int maxScale, int maxPrecision, string message)
        => property.AddRule(ValidatorRules.ScalePrecision<TProperty>(maxScale, maxPrecision, message));
}
