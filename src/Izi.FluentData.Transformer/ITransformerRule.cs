namespace Izi.FluentData.Transformer;

/// <summary>
/// One transformation step from <typeparamref name="Tin"/> to <typeparamref name="Tout"/>.
/// </summary>
/// <typeparam name="Tin">The type the step reads.</typeparam>
/// <typeparam name="Tout">The type the step produces.</typeparam>
public interface ITransformerRule<Tin, Tout>
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
    ValueTask<Tout> TransformAsync(Tin value, CancellationToken cancellationToken = default);
}

/// <summary>
/// A same-type transformation step.
/// This is <see cref="ITransformerRule{Tin, Tout}"/> with both type arguments equal.
/// </summary>
/// <typeparam name="T">The type that is both read and produced.</typeparam>
public interface ITransformerRule<T> : ITransformerRule<T, T>
{
}
