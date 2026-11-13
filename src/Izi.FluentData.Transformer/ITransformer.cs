namespace Izi.FluentData.Transformer;

/// <summary>
/// Runs property rules for <typeparamref name="T"/> and returns the updated value.
/// </summary>
/// <typeparam name="T">The object being transformed. Only its properties change.</typeparam>
/// <remarks>
/// <para>
/// This interface exists so a pipeline can be injected or mocked. <see cref="Transformer{T}"/> is the
/// implementation. Defining the abstraction here does not take a dependency on a dependency-injection package.
/// </para>
/// <para>
/// Structs are not mutated, and records or other init-only properties are cloned. Callers must use the
/// returned value (<c>value = await transformer.TransformAsync(value)</c>).
/// </para>
/// </remarks>
public interface ITransformer<T>
{
    /// <summary>Runs the configured property rules and returns the updated value.</summary>
    /// <param name="value">The object to transform.</param>
    /// <param name="cancellationToken">Checked before the run and between steps.</param>
    /// <returns>The transformed value. Structs and init-only records must be taken from this result.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is cancelled before a step runs.
    /// </exception>
    ValueTask<T> TransformAsync(T value, CancellationToken cancellationToken = default);
}
