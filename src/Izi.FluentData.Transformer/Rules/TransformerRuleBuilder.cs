namespace Izi.FluentData.Transformer.Rules;

/// <summary>
/// A fluent builder for a transformation pipeline. Unlike <c>ValidatorRuleBuilder&lt;T&gt;</c> — whose
/// rules all share the single type <c>T</c> and accumulate into a list — every transformer step can
/// change the running type, so this builder is immutable: each <see cref="AddRule{TNext}(TransformerRule{TCurrent, TNext})"/>
/// call returns a new builder retyped to the step's output, keeping the running type tracked at compile time.
/// The built-in steps are exposed as extension methods (see <c>TransformerRuleBuilderExtensions</c>).
/// </summary>
/// <typeparam name="TSource">The pipeline's original input type.</typeparam>
/// <typeparam name="TCurrent">The current running type after the steps added so far.</typeparam>
public class TransformerRuleBuilder<TSource, TCurrent>
{
    private readonly CompositeTransformerRule<TSource, TCurrent> _pipeline;

    internal TransformerRuleBuilder(CompositeTransformerRule<TSource, TCurrent> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>Appends <paramref name="rule"/> to the pipeline, advancing the running type to <typeparamref name="TNext"/>.</summary>
    /// <typeparam name="TNext">The output type of the appended step.</typeparam>
    /// <param name="rule">The step to append.</param>
    /// <returns>A new builder whose current type is <typeparamref name="TNext"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public TransformerRuleBuilder<TSource, TNext> AddRule<TNext>(TransformerRule<TCurrent, TNext> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        return new TransformerRuleBuilder<TSource, TNext>(_pipeline.Then(rule));
    }

    /// <summary>Appends a step built from a transformation delegate, advancing the running type to <typeparamref name="TNext"/>.</summary>
    /// <typeparam name="TNext">The output type of the appended step.</typeparam>
    /// <param name="transformFunc">The delegate that maps the current value to a <typeparamref name="TNext"/> value.</param>
    /// <returns>A new builder whose current type is <typeparamref name="TNext"/>.</returns>
    public TransformerRuleBuilder<TSource, TNext> AddRule<TNext>(Func<TCurrent, CancellationToken, ValueTask<TNext>> transformFunc)
        => AddRule(new TransformerRule<TCurrent, TNext>(transformFunc));

    /// <summary>Finalises the builder into a single composed rule.</summary>
    /// <returns>The composed <see cref="CompositeTransformerRule{TSource, TCurrent}"/>.</returns>
    public CompositeTransformerRule<TSource, TCurrent> Build() => _pipeline;
}

/// <summary>
/// A <see cref="TransformerRuleBuilder{TSource, TCurrent}"/> whose running type still matches its input
/// type. New builders start here (via the identity-seeded constructor) and may diverge from
/// <typeparamref name="TSource"/> as type-changing steps are appended.
/// </summary>
/// <typeparam name="TSource">The pipeline's input (and, until a step changes it, current) type.</typeparam>
public class TransformerRuleBuilder<TSource> : TransformerRuleBuilder<TSource, TSource>
{
    /// <summary>Creates a builder seeded with an identity step, ready for steps to be appended.</summary>
    public TransformerRuleBuilder() : base(new CompositeTransformerRule<TSource>(new TransformerRule<TSource>(static (value, _) => new ValueTask<TSource>(value))))
    {
    }

    internal TransformerRuleBuilder(CompositeTransformerRule<TSource, TSource> pipeline) : base(pipeline)
    {
    }
}
