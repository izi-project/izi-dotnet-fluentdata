namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// The atomic unit of a transformation pipeline: a single step that maps a <typeparamref name="TSource"/>
/// value to a <typeparamref name="TDestination"/> value, optionally asynchronously. Steps are composed into
/// a pipeline by <see cref="CompositeTransformerRule{TSource, TDestination}"/>.
/// </summary>
/// <typeparam name="TSource">The input type of this step.</typeparam>
/// <typeparam name="TDestination">The output type produced by this step.</typeparam>
public class TransformerRule<TSource, TDestination>
{
    private readonly Func<TSource, CancellationToken, ValueTask<TDestination>> _transformFunc;

    /// <summary>Creates a step backed by <paramref name="transformFunc"/>.</summary>
    /// <param name="transformFunc">The delegate that maps a source value to a destination value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="transformFunc"/> is <see langword="null"/>.</exception>
    public TransformerRule(Func<TSource, CancellationToken, ValueTask<TDestination>> transformFunc)
    {
        _transformFunc = transformFunc ?? throw new ArgumentNullException(nameof(transformFunc));
    }

    /// <summary>Runs the step against <paramref name="source"/>.</summary>
    /// <param name="source">The value to transform.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The transformed value.</returns>
    public ValueTask<TDestination> TransformAsync(TSource source, CancellationToken cancellationToken = default)
    {
        var pending = _transformFunc(source, cancellationToken);
        // Fast path: the vast majority of built-in steps complete synchronously. Returning the
        // already-completed ValueTask directly avoids building an async state machine for them.
        if (pending.IsCompletedSuccessfully)
        {
            return pending;
        }
        return TransformSlowAsync(_transformFunc, source, cancellationToken);
    }

    // Static so the awaiting state machine captures no `this`; reached only when the delegate genuinely yields.
    private static async ValueTask<TDestination> TransformSlowAsync(Func<TSource, CancellationToken, ValueTask<TDestination>> transformFunc, TSource source, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await transformFunc(source, cancellationToken);
    }
}

/// <summary>
/// A transformation step whose output type matches its input type — the common case for in-place
/// normalisation steps (trim, round, clamp, …) that refine a value without changing its type.
/// </summary>
/// <typeparam name="TSource">The input and output type of this step.</typeparam>
public class TransformerRule<TSource> : TransformerRule<TSource, TSource>
{
    /// <summary>Creates a same-type step backed by <paramref name="transformFunc"/>.</summary>
    /// <param name="transformFunc">The delegate that refines a value of type <typeparamref name="TSource"/>.</param>
    public TransformerRule(Func<TSource, CancellationToken, ValueTask<TSource>> transformFunc) : base(transformFunc)
    {
    }
}
