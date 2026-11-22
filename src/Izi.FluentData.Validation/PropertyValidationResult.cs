namespace Izi.FluentData.Validation;

/// <summary>The outcome of the rules for one property.</summary>
public sealed class PropertyValidationResult
{
    /// <summary>Creates a result for <paramref name="propertyName"/>.</summary>
    /// <param name="propertyName">The property these messages belong to.</param>
    /// <param name="errors">The messages. They do not repeat the property name.</param>
    internal PropertyValidationResult(string propertyName, ValidationError errors)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(errors);
        PropertyName = propertyName;
        Errors = errors;
    }

    /// <summary>The property that was validated.</summary>
    public string PropertyName { get; }

    /// <summary>True when the property produced no messages.</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>The messages for this property.</summary>
    public ValidationError Errors { get; }

    /// <summary>Prints one <c>PropertyName: message</c> line per message.</summary>
    /// <returns>The lines joined with a new line.</returns>
    public override string ToString() => ToString(Environment.NewLine);

    /// <summary>Prints one <c>PropertyName: message</c> line per message.</summary>
    /// <param name="separator">The text placed between lines.</param>
    /// <returns>The lines joined with <paramref name="separator"/>.</returns>
    public string ToString(string separator)
    {
        ArgumentNullException.ThrowIfNull(separator);
        var count = Errors.Count;
        if (count == 0)
        {
            return string.Empty;
        }

        if (count == 1)
        {
            return PropertyName + ": " + Errors[0];
        }

        var lines = new string[count];
        for (var i = 0; i < count; i++)
        {
            lines[i] = PropertyName + ": " + Errors[i];
        }

        return string.Join(separator, lines);
    }
}
