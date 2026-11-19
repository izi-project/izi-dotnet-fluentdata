namespace Izi.FluentData.Transformer.Exceptions;

/// <summary>A rule failed while transforming one property.</summary>
public sealed class PropertyTransformationException : TransformationException
{
    /// <summary>
    /// Creates an exception whose message names <paramref name="propertyName"/> and whose
    /// <see cref="Exception.InnerException"/> is <paramref name="innerException"/>.
    /// </summary>
    /// <param name="propertyName">The property whose rule failed.</param>
    /// <param name="innerException">The exception thrown by the rule.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> or <paramref name="innerException"/> is <see langword="null"/>.
    /// </exception>
    public PropertyTransformationException(string propertyName, Exception innerException)
        : base(CreateMessage(propertyName), innerException)
    {
        ArgumentNullException.ThrowIfNull(innerException);
        PropertyName = propertyName;
    }

    /// <summary>The property whose rule failed.</summary>
    public string PropertyName { get; }

    private static string CreateMessage(string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        return $"Transformation failed for property '{propertyName}'.";
    }
}
