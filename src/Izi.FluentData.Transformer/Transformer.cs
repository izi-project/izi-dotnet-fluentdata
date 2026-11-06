namespace Izi.FluentData.Transformer;

/// <summary>
/// Runs one transformation step and can append further steps with <see cref="Then{TNext}"/>.
/// </summary>
/// <typeparam name="TIn">The type accepted by the first step.</typeparam>
/// <typeparam name="TOut">The type produced by the last step.</typeparam>
/// <remarks>
/// <see cref="TransformAsync"/> checks the token, then returns the step's <see cref="ValueTask{TResult}"/>
/// directly, so a synchronous step does not allocate an async state machine here.
/// </remarks>
public sealed class Transformer<TIn, TOut>
{
    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _transform;

    /// <summary>Creates a pipeline whose only step is <paramref name="rule"/>.</summary>
    /// <param name="rule">The step to run.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public Transformer(ITransformerRule<TIn, TOut> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _transform = rule.TransformAsync;
    }

    private Transformer(Func<TIn, CancellationToken, ValueTask<TOut>> transform)
    {
        _transform = transform;
    }

    /// <summary>Transforms <paramref name="value"/>.</summary>
    /// <param name="value">The input value.</param>
    /// <param name="cancellationToken">Observed before the step runs, and by the step when it cooperates.</param>
    /// <returns>The transformed value.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is already cancelled.
    /// </exception>
    public ValueTask<TOut> TransformAsync(TIn value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _transform(value, cancellationToken);
    }

    /// <summary>Appends <paramref name="next"/> after this pipeline.</summary>
    /// <typeparam name="TNext">The type produced by <paramref name="next"/>.</typeparam>
    /// <param name="next">The step to run on this pipeline's output.</param>
    /// <returns>A pipeline from <typeparamref name="TIn"/> to <typeparamref name="TNext"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="next"/> is <see langword="null"/>.</exception>
    public Transformer<TIn, TNext> Then<TNext>(ITransformerRule<TOut, TNext> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        // Check the token between steps. A step that ignores cancellation must not hide a token
        // that was cancelled while that step ran.
        return new Transformer<TIn, TNext>(async (value, token) =>
        {
            var intermediate = await TransformAsync(value, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            return await next.TransformAsync(intermediate, token).ConfigureAwait(false);
        });
    }
}
