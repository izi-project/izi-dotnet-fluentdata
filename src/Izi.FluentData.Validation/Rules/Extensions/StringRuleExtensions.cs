using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation;

/// <summary>Fluent wrappers for the length and pattern rules.</summary>
public static class StringRuleExtensions
{
    /// <summary>Appends a rule that requires an exact length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="expectedLength">The required length.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Length<T, TProperty>(this PropertyValidator<T, TProperty> property, int expectedLength)
        => property.AddRule(ValidatorRules.Length<TProperty>(expectedLength));

    /// <summary>Appends a rule that requires an exact length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="expectedLength">The required length.</param>
    /// <param name="message">The message used when the length differs.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Length<T, TProperty>(this PropertyValidator<T, TProperty> property, int expectedLength, string message)
        => property.AddRule(ValidatorRules.Length<TProperty>(expectedLength, message));

    /// <summary>Appends a rule that requires a minimum length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="minLength">The inclusive minimum.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MinLength<T, TProperty>(this PropertyValidator<T, TProperty> property, int minLength)
        => property.AddRule(ValidatorRules.MinLength<TProperty>(minLength));

    /// <summary>Appends a rule that requires a minimum length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="minLength">The inclusive minimum.</param>
    /// <param name="message">The message used when the value is shorter.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MinLength<T, TProperty>(this PropertyValidator<T, TProperty> property, int minLength, string message)
        => property.AddRule(ValidatorRules.MinLength<TProperty>(minLength, message));

    /// <summary>Appends a rule that requires a maximum length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="maxLength">The inclusive maximum.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MaxLength<T, TProperty>(this PropertyValidator<T, TProperty> property, int maxLength)
        => property.AddRule(ValidatorRules.MaxLength<TProperty>(maxLength));

    /// <summary>Appends a rule that requires a maximum length.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="maxLength">The inclusive maximum.</param>
    /// <param name="message">The message used when the value is longer.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MaxLength<T, TProperty>(this PropertyValidator<T, TProperty> property, int maxLength, string message)
        => property.AddRule(ValidatorRules.MaxLength<TProperty>(maxLength, message));

    /// <summary>Appends a rule that requires a match of <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Matches<T, TProperty>(this PropertyValidator<T, TProperty> property, string pattern)
        => property.AddRule(ValidatorRules.Matches<TProperty>(pattern));

    /// <summary>Appends a rule that requires a match of <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <param name="message">The message used when the string does not match.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Matches<T, TProperty>(this PropertyValidator<T, TProperty> property, string pattern, string message)
        => property.AddRule(ValidatorRules.Matches<TProperty>(pattern, message));

    /// <summary>Appends a rule that requires a match of <paramref name="regex"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="regex">The expression. It must have a finite match timeout.</param>
    /// <param name="message">The message used when the string does not match.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Matches<T, TProperty>(this PropertyValidator<T, TProperty> property, Regex regex, string message)
        => property.AddRule(ValidatorRules.Matches<TProperty>(regex, message));

    /// <summary>Appends a rule that rejects a match of <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotMatches<T, TProperty>(this PropertyValidator<T, TProperty> property, string pattern)
        => property.AddRule(ValidatorRules.NotMatches<TProperty>(pattern));

    /// <summary>Appends a rule that rejects a match of <paramref name="pattern"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="pattern">The regular expression. It is compiled with a one-second timeout.</param>
    /// <param name="message">The message used when the string matches.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotMatches<T, TProperty>(this PropertyValidator<T, TProperty> property, string pattern, string message)
        => property.AddRule(ValidatorRules.NotMatches<TProperty>(pattern, message));

    /// <summary>Appends a rule that rejects a match of <paramref name="regex"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="regex">The expression. It must have a finite match timeout.</param>
    /// <param name="message">The message used when the string matches.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotMatches<T, TProperty>(this PropertyValidator<T, TProperty> property, Regex regex, string message)
        => property.AddRule(ValidatorRules.NotMatches<TProperty>(regex, message));

    /// <summary>Appends a rule that requires an email address.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Email<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.Email<TProperty>());

    /// <summary>Appends a rule that requires an email address.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the string is not an email address.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Email<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.Email<TProperty>(message));

    /// <summary>Appends a rule that requires a credit-card number.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CreditCard<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.CreditCard<TProperty>());

    /// <summary>Appends a rule that requires a credit-card number.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the string is not a credit-card number.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> CreditCard<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.CreditCard<TProperty>(message));
}
