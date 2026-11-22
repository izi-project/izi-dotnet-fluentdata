namespace Izi.FluentData.Validation;

/// <summary>Fluent wrappers for the null, emptiness, and custom predicate rules.</summary>
public static class GeneralRuleExtensions
{
    /// <summary>Appends a rule that requires a non-null value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotNull<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.NotNull<TProperty>());

    /// <summary>Appends a rule that requires a non-null value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the value is null.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotNull<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.NotNull<TProperty>(message));

    /// <summary>Appends a rule that requires a null value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Null<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.Null<TProperty>());

    /// <summary>Appends a rule that requires a null value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the value is not null.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Null<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.Null<TProperty>(message));

    /// <summary>Appends a rule that requires a non-empty value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotEmpty<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.NotEmpty<TProperty>());

    /// <summary>Appends a rule that requires a non-empty value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the value is empty.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> NotEmpty<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.NotEmpty<TProperty>(message));

    /// <summary>Appends a rule that requires an empty value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Empty<T, TProperty>(this PropertyValidator<T, TProperty> property)
        => property.AddRule(ValidatorRules.Empty<TProperty>());

    /// <summary>Appends a rule that requires an empty value.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="message">The message used when the value is not empty.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Empty<T, TProperty>(this PropertyValidator<T, TProperty> property, string message)
        => property.AddRule(ValidatorRules.Empty<TProperty>(message));

    /// <summary>Appends a custom predicate.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> Must<T, TProperty>(this PropertyValidator<T, TProperty> property, Func<TProperty, bool> predicate, string message)
        => property.AddRule(ValidatorRules.Must(predicate, message));

    /// <summary>Appends a custom asynchronous predicate.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MustAsync<T, TProperty>(this PropertyValidator<T, TProperty> property, Func<TProperty, CancellationToken, Task<bool>> predicate, string message)
        => property.AddRule(ValidatorRules.MustAsync(predicate, message));

    /// <summary>Appends a custom asynchronous predicate.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">The property the rule is appended to.</param>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the predicate returns false.</param>
    /// <returns>The same property validator.</returns>
    public static PropertyValidator<T, TProperty> MustAsync<T, TProperty>(this PropertyValidator<T, TProperty> property, Func<TProperty, Task<bool>> predicate, string message)
        => property.AddRule(ValidatorRules.MustAsync(predicate, message));
}
