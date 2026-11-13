namespace Izi.FluentData.Transformer;

/// <summary>
/// A transformation step backed by a delegate.
/// </summary>
/// <typeparam name="TIn">The type the step reads.</typeparam>
/// <typeparam name="TOut">The type the step produces.</typeparam>
/// <remarks>
/// <see cref="TransformAsync"/> checks the token once, then returns the delegate's
/// <see cref="ValueTask{TResult}"/> directly. A delegate that finishes synchronously does not allocate
/// an async state machine inside this type.
/// </remarks>
public sealed class TransformerRule<TIn, TOut> : ITransformerRule<TIn, TOut>
{
    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _transform;

    /// <summary>Creates a rule that runs <paramref name="transform"/>.</summary>
    /// <param name="transform">
    /// The function that maps a value. It receives the cancellation token after this rule has checked it.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public TransformerRule(Func<TIn, CancellationToken, ValueTask<TOut>> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        _transform = transform;
    }

    /// <summary>Transforms <paramref name="value"/> by calling the delegate this rule was created with.</summary>
    /// <param name="value">The input value.</param>
    /// <param name="cancellationToken">Checked once before the delegate runs, then passed to the delegate.</param>
    /// <returns>The delegate's result.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is already cancelled.
    /// </exception>
    public ValueTask<TOut> TransformAsync(TIn value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _transform(value, cancellationToken);
    }
}
