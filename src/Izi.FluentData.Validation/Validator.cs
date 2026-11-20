using System.Linq.Expressions;

namespace Izi.FluentData.Validation;

/// <summary>Configures and runs property rules for <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
public sealed class Validator<T> : IValidator<T>
{
    private readonly object _gate = new();
    private readonly List<IPropertyConfiguration<T>> _properties = [];
    private volatile ImmutableValidator<T>? _compiled = null;

    /// <summary>Registers rules for one property.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">A direct property access.</param>
    /// <returns>The property validator.</returns>
    public PropertyValidator<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> property)
    {
        _ = _gate;
        _ = _properties;
        _ = _compiled;
        _ = property;
        throw new NotImplementedException();
    }

    /// <summary>Registers rules for each item of a collection property.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="collection">A direct access of an enumerable property.</param>
    /// <returns>The item validator.</returns>
    public PropertyValidator<T, TItem> RuleForEach<TItem>(Expression<Func<T, IEnumerable<TItem>>> collection)
    {
        _ = collection;
        throw new NotImplementedException();
    }

    /// <summary>Includes another validator for the same type.</summary>
    /// <param name="validator">The validator to run as part of this one.</param>
    /// <returns>This validator.</returns>
    public Validator<T> Include(IValidator<T> validator)
    {
        _ = validator;
        throw new NotImplementedException();
    }

    /// <summary>Validates <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The validation result.</returns>
    public ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        _ = instance;
        _ = cancellationToken;
        throw new NotImplementedException();
    }
}

/// <summary>The compiled rules for one <see cref="Validator{T}"/> configuration.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal sealed class ImmutableValidator<T>
{
    private readonly ICompiledPropertyValidator<T>[] _properties;

    /// <summary>Creates a runner for <paramref name="properties"/>.</summary>
    /// <param name="properties">The compiled property validators, in registration order.</param>
    internal ImmutableValidator(ICompiledPropertyValidator<T>[] properties)
    {
        _properties = properties;
    }

    /// <summary>Validates <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">Checked while validation runs.</param>
    /// <returns>The validation result.</returns>
    internal ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken)
    {
        _ = _properties;
        _ = instance;
        _ = cancellationToken;
        throw new NotImplementedException();
    }
}
