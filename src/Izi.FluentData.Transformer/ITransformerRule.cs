namespace Izi.FluentData.Transformer;

/// <summary>One same-type transformation step.</summary>
/// <typeparam name="T">The type the step reads and returns.</typeparam>
public interface ITransformerRule<T>
{
    /// <summary>Transforms <paramref name="value"/>.</summary>
    /// <param name="value">The input value.</param>
    /// <param name="cancellationToken">
    /// Observed before the step runs, and passed through when the step cooperates.
    /// </param>
    /// <returns>The transformed value, of the same type as <paramref name="value"/>.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is already cancelled.
    /// </exception>
    ValueTask<T> TransformAsync(T value, CancellationToken cancellationToken = default);
}
