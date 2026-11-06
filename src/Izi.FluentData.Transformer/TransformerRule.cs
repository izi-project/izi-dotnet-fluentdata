namespace Izi.FluentData.Transformer;

/// <summary>
/// A transformation step backed by a delegate.
/// </summary>
/// <typeparam name="Tin">The type the step reads.</typeparam>
/// <typeparam name="Tout">The type the step produces.</typeparam>
/// <remarks>
/// <see cref="TransformAsync"/> checks the token once, then returns the delegate's
/// <see cref="ValueTask{TResult}"/> directly. A delegate that finishes synchronously does not allocate
/// an async state machine inside this type.
/// </remarks>
public sealed class TransformerRule<Tin, Tout> : ITransformerRule<Tin, Tout>
{
    private readonly Func<Tin, CancellationToken, ValueTask<Tout>> _transform;

    /// <summary>Creates a rule that runs <paramref name="transform"/>.</summary>
    /// <param name="transform">
    /// The function that maps a value. It receives the cancellation token after this rule has checked it.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public TransformerRule(Func<Tin, CancellationToken, ValueTask<Tout>> transform)
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
    public ValueTask<Tout> TransformAsync(Tin value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _transform(value, cancellationToken);
    }
}
