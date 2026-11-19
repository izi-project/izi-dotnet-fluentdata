namespace Izi.FluentData.Transformer;

/// <summary>Same-type rules for one property. The property is read, transformed, and written back.</summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public sealed class PropertyRule<T, TProperty> : ITransformerStep<T>
{
    private readonly Transformer<T> _owner;
    private readonly Func<T, TProperty> _read;
    private readonly Func<T, TProperty, T> _write;
    private readonly List<ITransformerRule<TProperty>> _rules = [];

    internal PropertyRule(
        Transformer<T> owner,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write)
    {
        _owner = owner;
        _read = read;
        _write = write;
    }

    /// <summary>Appends a same-type rule. Further calls stay on this property, ahead of steps registered later.</summary>
    /// <param name="rule">The rule to run on the property value.</param>
    /// <returns>This property rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public PropertyRule<T, TProperty> Add(ITransformerRule<TProperty> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            _rules.Add(rule);
            _owner.InvalidateSnapshot();
        }

        return this;
    }

    /// <summary>Appends a same-type delegate, wrapped in a <see cref="TransformerRule{T}"/>.</summary>
    /// <param name="transform">The function that maps the property value.</param>
    /// <returns>This property rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public PropertyRule<T, TProperty> Add(Func<TProperty, CancellationToken, ValueTask<TProperty>> transform)
    {
        return Add(new TransformerRule<TProperty>(transform));
    }

    /// <summary>Copies the rules configured at this moment.</summary>
    /// <returns>A step that does not observe later <see cref="Add(ITransformerRule{TProperty})"/> calls.</returns>
    internal ITransformerStep<T> Snapshot()
    {
        return new SameTypeStep(_read, _write, _rules.ToArray());
    }

    private sealed class SameTypeStep : ITransformerStep<T>
    {
        private readonly Func<T, TProperty> _read;
        private readonly Func<T, TProperty, T> _write;
        private readonly ITransformerRule<TProperty>[] _rules;

        public SameTypeStep(
            Func<T, TProperty> read,
            Func<T, TProperty, T> write,
            ITransformerRule<TProperty>[] rules)
        {
            _read = read;
            _write = write;
            _rules = rules;
        }

        public ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
            => ApplyRules(instance, _read, _write, _rules, cancellationToken);

        public ITransformerStep<T> Snapshot() => this;
    }

    private static ValueTask<T> ApplyRules(
        T instance,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write,
        ITransformerRule<TProperty>[] rules,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = read(instance);
        for (var i = 0; i < rules.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = rules[i].TransformAsync(value, cancellationToken);
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueRulesAsync(pending, rules, i + 1, instance, write, cancellationToken);
            }

            value = pending.Result;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<T>(write(instance, value));
    }

    private static async ValueTask<T> ContinueRulesAsync(
        ValueTask<TProperty> pending,
        ITransformerRule<TProperty>[] rules,
        int index,
        T instance,
        Func<T, TProperty, T> write,
        CancellationToken cancellationToken)
    {
        var value = await pending.ConfigureAwait(false);
        for (; index < rules.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            value = await rules[index].TransformAsync(value, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return write(instance, value);
    }

    ValueTask<T> ITransformerStep<T>.ApplyAsync(T instance, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("A property rule must be snapshotted before it runs.");
    }

    ITransformerStep<T> ITransformerStep<T>.Snapshot()
    {
        return Snapshot();
    }
}
