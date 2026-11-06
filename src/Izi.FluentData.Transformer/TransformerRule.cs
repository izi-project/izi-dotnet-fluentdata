namespace Izi.FluentData.Transformer;

public interface ITransformerRule<T> : ITransformerRule<T, T> {}
public interface ITransformerRule<Tin, Tout>
{
    ValueTask<Tout> TransformAsync(Tin value, CancellationToken cancellationToken = default);
}

public sealed class TransformerRule<Tin, Tout> : ITransformerRule<Tin, Tout>
{
    private readonly Func<Tin, CancellationToken, ValueTask<Tout>> _transform;

    public TransformerRule(Func<Tin, CancellationToken, ValueTask<Tout>> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        _transform = transform;
    }

    public ValueTask<Tout> TransformAsync(Tin value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _transform(value, cancellationToken);
    }
}
