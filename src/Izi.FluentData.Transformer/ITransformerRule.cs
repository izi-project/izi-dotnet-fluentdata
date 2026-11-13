namespace Izi.FluentData.Transformer;

/// <summary>
/// One transformation step from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
/// </summary>
/// <typeparam name="TIn">The type the step reads.</typeparam>
/// <typeparam name="TOut">The type the step produces.</typeparam>
public interface ITransformerRule<TIn, TOut>
{
    /// <summary>Transforms <paramref name="value"/>.</summary>
    /// <param name="value">The input value.</param>
    /// <param name="cancellationToken">
    /// Observed before the step runs, and passed through when the step cooperates.
    /// </param>
    /// <returns>The transformed value.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is already cancelled.
    /// </exception>
    ValueTask<TOut> TransformAsync(TIn value, CancellationToken cancellationToken = default);
}

/// <summary>
/// A same-type transformation step.
/// This is <see cref="ITransformerRule{TIn, TOut}"/> with both type arguments equal.
/// </summary>
/// <typeparam name="T">The type that is both read and produced.</typeparam>
public interface ITransformerRule<T> : ITransformerRule<T, T>
{
}
