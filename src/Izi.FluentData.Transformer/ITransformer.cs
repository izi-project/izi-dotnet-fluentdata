namespace Izi.FluentData.Transformer;

/// <summary>
/// Transforms (normalises) an instance of <typeparamref name="T"/> in place by running a
/// configured pipeline over its properties.
/// </summary>
/// <typeparam name="T">The type whose instances are transformed.</typeparam>
public interface ITransformer<T>
{
    /// <summary>Applies every configured property pipeline to <paramref name="instance"/>.</summary>
    /// <param name="instance">The instance to transform.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The same instance, with its properties transformed.</returns>
    ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default);
}
