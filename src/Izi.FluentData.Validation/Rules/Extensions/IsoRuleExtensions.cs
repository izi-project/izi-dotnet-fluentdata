namespace Izi.FluentData.Validation;

/// <summary>Fluent wrappers for the ISO country and currency rules.</summary>
public static class IsoRuleExtensions
{
    /// <summary>Appends a rule that requires an ISO 3166-1 alpha-2 country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIso2<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.CountryIso2<TProperty>());

    /// <summary>Appends a rule that requires an ISO 3166-1 alpha-2 country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the code is not recognised.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIso2<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.CountryIso2<TProperty>(message));

    /// <summary>Appends a rule that requires an ISO 3166-1 alpha-3 country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIso3<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.CountryIso3<TProperty>());

    /// <summary>Appends a rule that requires an ISO 3166-1 alpha-3 country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the code is not recognised.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIso3<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.CountryIso3<TProperty>(message));

    /// <summary>Appends a rule that requires an ISO 3166-1 numeric country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIsoNumeric<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.CountryIsoNumeric<TProperty>());

    /// <summary>Appends a rule that requires an ISO 3166-1 numeric country code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the code is not recognised.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CountryIsoNumeric<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.CountryIsoNumeric<TProperty>(message));

    /// <summary>Appends a rule that requires an ISO 4217 currency code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CurrencyIso<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.CurrencyIso<TProperty>());

    /// <summary>Appends a rule that requires an ISO 4217 currency code.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the code is not recognised.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CurrencyIso<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.CurrencyIso<TProperty>(message));
}
