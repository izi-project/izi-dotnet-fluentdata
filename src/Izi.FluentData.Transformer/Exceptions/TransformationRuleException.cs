namespace Izi.FluentData.Transformer.Exceptions;

/// <summary>A transformation rule failed. The message describes that failure.</summary>
public sealed class TransformationRuleException : TransformationException
{
    /// <summary>Creates an exception with <paramref name="message"/>.</summary>
    /// <param name="message">The message that describes the rule failure.</param>
    public TransformationRuleException(string? message)
        : base(message)
    {
    }

    /// <summary>Creates an exception with <paramref name="message"/> and <paramref name="innerException"/>.</summary>
    /// <param name="message">The message that describes the rule failure.</param>
    /// <param name="innerException">The exception the rule threw.</param>
    public TransformationRuleException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Decides which exceptions a transformer reports unchanged.</summary>
internal static class ExceptionClassification
{
    /// <summary>True when <paramref name="exception"/> should be wrapped.</summary>
    /// <param name="exception">The exception that was thrown.</param>
    /// <returns>False for cancellation and fatal exceptions.</returns>
    internal static bool ShouldWrap(Exception exception)
        => exception is not OperationCanceledException && !IsFatal(exception);

    private static bool IsFatal(Exception exception)
    {
#pragma warning disable CS0618 // Obsolete on .NET: ThreadAbortException is not thrown. A thrown instance is still left unwrapped.
        return exception is OutOfMemoryException
            or StackOverflowException
            or InsufficientExecutionStackException
            or ThreadAbortException
            or AccessViolationException;
#pragma warning restore CS0618
    }
}
