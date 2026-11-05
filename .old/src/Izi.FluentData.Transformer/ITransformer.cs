namespace Izi.FluentData.Transformer;

/// <summary>
/// A single transformation step: maps a value of type <typeparamref name="T"/> to another value of the same type.
/// Everything in the pipeline — an individual rule, a nested per-property transformer, and the top-level
/// <see cref="Transformer{T}"/> itself — implements this interface, so steps and whole pipelines are
/// interchangeable and compose freely.
/// </summary>
/// <remarks>
/// The return type is <see cref="ValueTask{TResult}"/> so a synchronous step (which every built-in rule is) can
/// complete without allocating an async state machine; only a genuinely asynchronous custom step pays that cost.
/// Implementations are expected to be stateless and therefore safe to share across threads.
/// </remarks>
/// <typeparam name="T">The type transformed. Input and output are the same type — pipelines are not type-changing.</typeparam>
public interface ITransformer<T>
{
    /// <summary>Transforms <paramref name="instance"/> and returns the result.</summary>
    /// <param name="instance">The value to transform.</param>
    /// <param name="cancellationToken">A token to observe while transforming.</param>
    /// <returns>The transformed value.</returns>
    ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default);
}
