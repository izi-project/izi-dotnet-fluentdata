using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer;

/// <summary>A same-type transformation step backed by a delegate.</summary>
/// <typeparam name="T">The type the step reads and returns.</typeparam>
/// <remarks>
/// <see cref="TransformAsync"/> checks the token once. A delegate that finishes successfully is returned
/// without an async state machine. A failure becomes <see cref="TransformationRuleException"/>, except
/// cancellation and fatal exceptions, which propagate unchanged. A delegate that throws before returning
/// a <see cref="ValueTask{TResult}"/> throws here. A faulted <see cref="ValueTask{TResult}"/> is returned
/// faulted, so the caller still awaits it.
/// </remarks>
public sealed class TransformerRule<T> : ITransformerRule<T>
{
    private readonly Func<T, CancellationToken, ValueTask<T>> _transform;

    /// <summary>Creates a rule that runs <paramref name="transform"/>.</summary>
    /// <param name="transform">
    /// The function that maps a value to the same type. It receives the cancellation token after this rule has checked it.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public TransformerRule(Func<T, CancellationToken, ValueTask<T>> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        _transform = transform;
    }

    /// <summary>Transforms <paramref name="value"/> by calling the delegate this rule was created with.</summary>
    /// <param name="value">The input value.</param>
    /// <param name="cancellationToken">Checked once before the delegate runs, then passed to the delegate.</param>
    /// <returns>The delegate's result.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is already cancelled, or the delegate was cancelled.
    /// </exception>
    /// <exception cref="TransformationRuleException">The delegate failed for another reason.</exception>
    public ValueTask<T> TransformAsync(T value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValueTask<T> pending;
        try
        {
            pending = _transform(value, cancellationToken);
        }
        catch (Exception ex) when (ExceptionClassification.ShouldWrap(ex))
        {
            throw new TransformationRuleException(ex.Message, ex);
        }

        if (pending.IsCompletedSuccessfully)
        {
            return pending;
        }

        if (!pending.IsCompleted)
        {
            return TransformSlowAsync(pending);
        }

        try
        {
            pending.GetAwaiter().GetResult();
        }
        catch (Exception ex) when (ExceptionClassification.ShouldWrap(ex))
        {
            return ValueTask.FromException<T>(new TransformationRuleException(ex.Message, ex));
        }
        catch (Exception ex)
        {
            return ValueTask.FromException<T>(ex);
        }

        return pending;
    }

    private static async ValueTask<T> TransformSlowAsync(ValueTask<T> pending)
    {
        try
        {
            return await pending.ConfigureAwait(false);
        }
        catch (Exception ex) when (ExceptionClassification.ShouldWrap(ex))
        {
            throw new TransformationRuleException(ex.Message, ex);
        }
    }
}
