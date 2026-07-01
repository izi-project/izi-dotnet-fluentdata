namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// A transformation pipeline modelled as a single rule: it chains steps end-to-end, threading each step's
/// output into the next. The chain is captured as a single composed delegate (built by <see cref="Then{TNew}"/>),
/// so the whole pipeline executes as one <see cref="TransformerRule{TSource, TDestination}"/>.
/// </summary>
/// <typeparam name="TSource">The pipeline's input type.</typeparam>
/// <typeparam name="TDestination">The pipeline's output type after the steps composed so far.</typeparam>
public class CompositeTransformerRule<TSource, TDestination> : TransformerRule<TSource, TDestination>
{
    // Private: used by Then to wrap an already-composed pipeline delegate without re-guarding it.
    private CompositeTransformerRule(Func<TSource, CancellationToken, ValueTask<TDestination>> pipeline) : base(pipeline)
    {
    }

    /// <summary>Seeds a pipeline with its first step.</summary>
    /// <param name="initialRule">The first step of the pipeline.</param>
    /// <remarks>
    /// The null check is deferred into the pipeline delegate (it runs on first invocation rather than at
    /// construction), so passing a null initial rule surfaces as an <see cref="ArgumentNullException"/> when
    /// the pipeline is executed.
    /// </remarks>
    public CompositeTransformerRule(TransformerRule<TSource, TDestination> initialRule) : base((source, cancellationToken) => (initialRule ?? throw new ArgumentNullException(nameof(initialRule))).TransformAsync(source, cancellationToken))
    {
    }

    /// <summary>
    /// Appends <paramref name="nextRule"/> to the pipeline, advancing the output type to <typeparamref name="TNew"/>.
    /// </summary>
    /// <typeparam name="TNew">The output type of the appended step.</typeparam>
    /// <param name="nextRule">The step to run after the current pipeline.</param>
    /// <returns>A new composite whose output type is <typeparamref name="TNew"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="nextRule"/> is <see langword="null"/>.</exception>
    public CompositeTransformerRule<TSource, TNew> Then<TNew>(TransformerRule<TDestination, TNew> nextRule)
    {
        ArgumentNullException.ThrowIfNull(nextRule);
        return new CompositeTransformerRule<TSource, TNew>((source, cancellationToken) =>
        {
            var pending = TransformAsync(source, cancellationToken);
            // Short-circuit: while every step in the chain completes synchronously the whole pipeline stays
            // synchronous, feeding each result straight into the next step with no async state machine.
            return pending.IsCompletedSuccessfully
                ? nextRule.TransformAsync(pending.Result, cancellationToken)
                : TransformSlowAsync(pending, nextRule, cancellationToken);
        });
    }

    // Static so the awaiting state machine captures no `this`; reached only once a preceding step yields.
    private static async ValueTask<TNew> TransformSlowAsync<TNew>(ValueTask<TDestination> pending, TransformerRule<TDestination, TNew> next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await pending;
        return await next.TransformAsync(result, cancellationToken);
    }
}

/// <summary>
/// A pipeline whose output type still matches its input type — the starting shape for a new builder before
/// any type-changing step is appended.
/// </summary>
/// <typeparam name="TSource">The pipeline's input (and, until a step changes it, output) type.</typeparam>
public class CompositeTransformerRule<TSource> : CompositeTransformerRule<TSource, TSource>
{
    /// <summary>Seeds a same-type pipeline with its first step.</summary>
    /// <param name="initialRule">The first step of the pipeline.</param>
    public CompositeTransformerRule(TransformerRule<TSource> initialRule) : base(initialRule)
    {
    }
}
