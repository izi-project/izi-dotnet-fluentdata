namespace Izi.FluentData.Validation;

/// <summary>The outcome of validating one object. Only failed properties are listed.</summary>
public sealed class ValidationResult
{
    /// <summary>A validation run in which every property passed.</summary>
    public static ValidationResult Success => throw new NotImplementedException();

    /// <summary>Creates a result from <paramref name="errors"/>.</summary>
    /// <param name="errors">The property results. Passing ones are omitted.</param>
    internal ValidationResult(IReadOnlyList<PropertyValidationResult> errors)
    {
        _ = errors;
        throw new NotImplementedException();
    }

    /// <summary>True when no property failed.</summary>
    public bool IsValid => throw new NotImplementedException();

    /// <summary>The properties that failed, in registration order.</summary>
    public IReadOnlyList<PropertyValidationResult> Errors => throw new NotImplementedException();

    /// <summary>Joins each failed property with a new line.</summary>
    /// <returns>One block per failed property.</returns>
    public override string ToString() => throw new NotImplementedException();

    /// <summary>Joins each failed property with <paramref name="separator"/>.</summary>
    /// <param name="separator">The text placed between property blocks and between messages.</param>
    /// <returns>The failed properties.</returns>
    public string ToString(string separator)
    {
        _ = separator;
        throw new NotImplementedException();
    }
}
