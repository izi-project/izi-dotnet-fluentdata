namespace Izi.FluentData.Validation;

/// <summary>Rules for one property of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public sealed class PropertyValidator<T, TProperty> : IPropertyConfiguration<T>
{
    /// <summary>Creates a validator for <paramref name="propertyName"/>.</summary>
    /// <param name="propertyName">The property these rules read.</param>
    internal PropertyValidator(string propertyName)
    {
        PropertyName = propertyName;
    }

    /// <summary>The property these rules read.</summary>
    public string PropertyName { get; private set; }

    /// <summary>Appends <paramref name="rule"/>.</summary>
    /// <param name="rule">The rule to run on the property value.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> AddRule(IValidatorRule<TProperty> rule)
    {
        _ = rule;
        throw new NotImplementedException();
    }

    /// <summary>Replaces the message of the last rule.</summary>
    /// <param name="message">The message. It does not name a property.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> WithMessage(string message)
    {
        _ = message;
        throw new NotImplementedException();
    }

    /// <summary>Replaces the name used when this property is reported.</summary>
    /// <param name="name">The name to report.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> WithName(string name)
    {
        _ = name;
        throw new NotImplementedException();
    }

    /// <summary>Runs the rules only when <paramref name="predicate"/> is true.</summary>
    /// <param name="predicate">The condition, given the object being validated.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> When(Func<T, bool> predicate)
    {
        _ = predicate;
        throw new NotImplementedException();
    }

    /// <summary>Skips the rules when <paramref name="predicate"/> is true.</summary>
    /// <param name="predicate">The condition, given the object being validated.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> Unless(Func<T, bool> predicate)
    {
        _ = predicate;
        throw new NotImplementedException();
    }

    /// <summary>Validates the property value with <paramref name="validator"/>.</summary>
    /// <param name="validator">The child validator.</param>
    /// <returns>This property validator.</returns>
    public PropertyValidator<T, TProperty> SetValidator(IValidator<TProperty> validator)
    {
        _ = validator;
        throw new NotImplementedException();
    }

    /// <summary>Validates the property on <paramref name="instance"/>.</summary>
    /// <param name="instance">The object that owns the property.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The property result.</returns>
    public ValueTask<PropertyValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        _ = instance;
        _ = cancellationToken;
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    ICompiledPropertyValidator<T> IPropertyConfiguration<T>.Compile()
    {
        throw new NotImplementedException();
    }
}

/// <summary>The compiled rules for one property.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
internal sealed class ImmutablePropertyValidator<T, TProperty> : ICompiledPropertyValidator<T>
{
    /// <summary>Validates the property on <paramref name="instance"/>.</summary>
    /// <param name="instance">The object that owns the property.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The property result.</returns>
    public ValueTask<PropertyValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken)
    {
        _ = instance;
        _ = cancellationToken;
        throw new NotImplementedException();
    }
}
