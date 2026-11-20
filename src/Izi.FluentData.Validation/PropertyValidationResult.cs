namespace Izi.FluentData.Validation;

/// <summary>The outcome of the rules for one property.</summary>
public sealed class PropertyValidationResult
{
    /// <summary>Creates a result for <paramref name="propertyName"/>.</summary>
    /// <param name="propertyName">The property these messages belong to.</param>
    /// <param name="errors">The messages. They do not repeat the property name.</param>
    internal PropertyValidationResult(string propertyName, ValidationError errors)
    {
        _ = propertyName;
        _ = errors;
        throw new NotImplementedException();
    }

    /// <summary>The property that was validated.</summary>
    public string PropertyName => throw new NotImplementedException();

    /// <summary>True when the property produced no messages.</summary>
    public bool IsValid => throw new NotImplementedException();

    /// <summary>The messages for this property.</summary>
    public ValidationError Errors => throw new NotImplementedException();

    /// <summary>Prints one <c>PropertyName: message</c> line per message.</summary>
    /// <returns>The lines joined with a new line.</returns>
    public override string ToString() => throw new NotImplementedException();

    /// <summary>Prints one <c>PropertyName: message</c> line per message.</summary>
    /// <param name="separator">The text placed between lines.</param>
    /// <returns>The lines joined with <paramref name="separator"/>.</returns>
    public string ToString(string separator)
    {
        _ = separator;
        throw new NotImplementedException();
    }
}
