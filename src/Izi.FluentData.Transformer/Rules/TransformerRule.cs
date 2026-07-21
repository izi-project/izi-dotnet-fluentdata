namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// The leaf transformation step: adapts a plain <c>(value, token) =&gt; ValueTask&lt;value&gt;</c> delegate into an
/// <see cref="ITransformer{T}"/>. Every built-in rule on <see cref="TransformerRules"/> is one of these, and custom
/// steps can be created by constructing one directly.
/// </summary>
/// <remarks>
/// The token is checked once on entry, then the delegate runs. When it completes synchronously (the common case for
/// the built-ins) the underlying <see cref="ValueTask{TResult}"/> is returned as-is, so no async state machine is
/// built; the awaiting <see cref="TransformSlowAsync"/> tail is reached only when the delegate genuinely yields.
/// </remarks>
/// <typeparam name="T">The type transformed.</typeparam>
public class TransformerRule<T> : ITransformer<T>
{
    private readonly Func<T, CancellationToken, ValueTask<T>> _transformFunc;

    /// <summary>Creates a rule backed by <paramref name="transformFunc"/>.</summary>
    /// <param name="transformFunc">The transformation to run; receives the current value and a cancellation token.</param>
    public TransformerRule(Func<T, CancellationToken, ValueTask<T>> transformFunc)
    {
        _transformFunc = transformFunc;
    }

    /// <inheritdoc />
    public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pendingTask = _transformFunc(instance, cancellationToken);
        return pendingTask.IsCompletedSuccessfully ? pendingTask : TransformSlowAsync(pendingTask, cancellationToken);
    }

    // Async tail reached only when the delegate did not complete synchronously; kept separate so the common
    // synchronous path above never builds a state machine.
    private static async ValueTask<T> TransformSlowAsync(ValueTask<T> transformTask, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await transformTask.ConfigureAwait(false);
    }
}
