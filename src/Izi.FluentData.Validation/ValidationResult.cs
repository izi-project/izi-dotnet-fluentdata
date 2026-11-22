namespace Izi.FluentData.Validation;

/// <summary>The outcome of validating one object. Only failed properties are listed.</summary>
public sealed class ValidationResult
{
    private static readonly PropertyValidationResult[] NoFailures = [];

    /// <summary>A validation run in which every property passed. Shared by every fully valid run.</summary>
    public static ValidationResult Success { get; } = new(NoFailures);

    private readonly IReadOnlyList<PropertyValidationResult> _errors;

    /// <summary>Creates a result from failed properties only.</summary>
    /// <param name="errors">The properties that failed, in registration order.</param>
    internal ValidationResult(IReadOnlyList<PropertyValidationResult> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        _errors = errors;
    }

    /// <summary>True when no property failed.</summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>The properties that failed, in registration order.</summary>
    public IReadOnlyList<PropertyValidationResult> Errors => _errors;

    /// <summary>Joins each failed property with a new line.</summary>
    /// <returns>One block per failed property.</returns>
    public override string ToString() => ToString(Environment.NewLine);

    /// <summary>Joins each failed property with <paramref name="separator"/>.</summary>
    /// <param name="separator">The text placed between property blocks and between messages.</param>
    /// <returns>The failed properties.</returns>
    public string ToString(string separator)
    {
        ArgumentNullException.ThrowIfNull(separator);
        var count = _errors.Count;
        if (count == 0)
        {
            return string.Empty;
        }

        if (count == 1)
        {
            return _errors[0].ToString(separator);
        }

        var parts = new string[count];
        for (var i = 0; i < count; i++)
        {
            parts[i] = _errors[i].ToString(separator);
        }

        return string.Join(separator, parts);
    }
}
