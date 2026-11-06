namespace Izi.FluentData.Transformer;

public sealed class Transformer<TIn, TOut>
{
    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _transform;

    public Transformer(ITransformerRule<TIn, TOut> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _transform = rule.TransformAsync;
    }

    private Transformer(Func<TIn, CancellationToken, ValueTask<TOut>> transform)
    {
        _transform = transform;
    }

    public ValueTask<TOut> TransformAsync(TIn value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _transform(value, cancellationToken);
    }

    public Transformer<TIn, TNext> Then<TNext>(ITransformerRule<TOut, TNext> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return new Transformer<TIn, TNext>(async (value, token) =>
        {
            var intermediate = await TransformAsync(value, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            return await next.TransformAsync(intermediate, token).ConfigureAwait(false);
        });
    }
}