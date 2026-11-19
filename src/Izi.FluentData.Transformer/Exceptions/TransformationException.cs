namespace Izi.FluentData.Transformer.Exceptions;

/// <summary>A transformation failed. The message describes the failure.</summary>
public class TransformationException : Exception
{
    /// <summary>Creates an exception with no message.</summary>
    public TransformationException()
    {
    }

    /// <summary>Creates an exception with <paramref name="message"/>.</summary>
    /// <param name="message">The message that describes the failure.</param>
    public TransformationException(string? message)
        : base(message)
    {
    }

    /// <summary>Creates an exception with <paramref name="message"/> and <paramref name="innerException"/>.</summary>
    /// <param name="message">The message that describes the failure.</param>
    /// <param name="innerException">The exception that caused this failure.</param>
    public TransformationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
