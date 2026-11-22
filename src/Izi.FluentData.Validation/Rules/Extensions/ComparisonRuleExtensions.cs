namespace Izi.FluentData.Validation;

/// <summary>Fluent wrappers for the equality and ordering rules.</summary>
public static class ComparisonRuleExtensions
{
    /// <summary>Appends a rule that requires the value to equal <paramref name="expected"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="expected">The required value.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Equal<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty expected)
        => property.AddRule(ValidatorRules.Equal(expected));

    /// <summary>Appends a rule that requires the value to equal <paramref name="expected"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="expected">The required value.</param>
    /// <param name="message">The message used when the value differs.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Equal<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty expected, string message)
        => property.AddRule(ValidatorRules.Equal(expected, message));

    /// <summary>Appends a rule that requires the value to differ from <paramref name="unexpected"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="unexpected">The rejected value.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty unexpected)
        => property.AddRule(ValidatorRules.NotEqual(unexpected));

    /// <summary>Appends a rule that requires the value to differ from <paramref name="unexpected"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="unexpected">The rejected value.</param>
    /// <param name="message">The message used when the value matches.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty unexpected, string message)
        => property.AddRule(ValidatorRules.NotEqual(unexpected, message));

    /// <summary>Appends a rule that requires the value to be strictly less than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The exclusive upper bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> LessThan<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.LessThan(threshold));

    /// <summary>Appends a rule that requires the value to be strictly less than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The exclusive upper bound.</param>
    /// <param name="message">The message used when the value is not less.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> LessThan<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.LessThan(threshold, message));

    /// <summary>Appends a rule that requires the value to be less than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The inclusive upper bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> LessThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.LessThanOrEqual(threshold));

    /// <summary>Appends a rule that requires the value to be less than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The inclusive upper bound.</param>
    /// <param name="message">The message used when the value is greater.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> LessThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.LessThanOrEqual(threshold, message));

    /// <summary>Appends a rule that requires the value to be strictly greater than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> GreaterThan<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.GreaterThan(threshold));

    /// <summary>Appends a rule that requires the value to be strictly greater than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <param name="message">The message used when the value is not greater.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> GreaterThan<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.GreaterThan(threshold, message));

    /// <summary>Appends a rule that requires the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The inclusive lower bound.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> GreaterThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.GreaterThanOrEqual(threshold));

    /// <summary>Appends a rule that requires the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="threshold">The inclusive lower bound.</param>
    /// <param name="message">The message used when the value is smaller.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> GreaterThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> property, TProperty threshold, string message)
        where TProperty : IComparable<TProperty>
        => property.AddRule(ValidatorRules.GreaterThanOrEqual(threshold, message));
}
