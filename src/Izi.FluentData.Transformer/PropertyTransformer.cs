using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Same-type rules for one property. The property is read, each rule runs in order, and the result is written back.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public sealed class PropertyTransformer<T, TProperty> : IPropertyConfiguration<T>
{
    private readonly Transformer<T> _owner;
    private readonly Func<T, TProperty> _read;
    private readonly Func<T, TProperty, T> _write;
    private readonly List<TransformerRule<TProperty>> _rules = [];
    private volatile ImmutablePropertyTransformer<T, TProperty>? _compiled;

    internal PropertyTransformer(
        Transformer<T> owner,
        string propertyName,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write)
    {
        _owner = owner;
        PropertyName = propertyName;
        _read = read;
        _write = write;
    }

    /// <summary>The property this transformer reads and writes.</summary>
    public string PropertyName { get; }

    /// <summary>Appends a same-type rule. Further calls stay on this property.</summary>
    /// <param name="rule">The rule to run on the property value.</param>
    /// <returns>This property transformer.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public PropertyTransformer<T, TProperty> Add(ITransformerRule<TProperty> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (rule is TransformerRule<TProperty> concrete)
        {
            return AddCore(concrete);
        }

        return AddCore(new TransformerRule<TProperty>((value, token) => rule.TransformAsync(value, token)));
    }

    /// <summary>Appends a same-type delegate, wrapped in a <see cref="TransformerRule{T}"/>.</summary>
    /// <param name="transform">The function that maps the property value.</param>
    /// <returns>This property transformer.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public PropertyTransformer<T, TProperty> Add(Func<TProperty, CancellationToken, ValueTask<TProperty>> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        return AddCore(new TransformerRule<TProperty>(transform));
    }

    /// <summary>
    /// Returns the compiled runner for the rules currently registered.
    /// Callers already hold the owner gate. The runner is reused until a rule is added.
    /// </summary>
    /// <returns>The immutable runner for this property.</returns>
    internal ImmutablePropertyTransformer<T, TProperty> Compile()
    {
        var compiled = _compiled;
        if (compiled is not null)
        {
            return compiled;
        }

        lock (_owner.Gate)
        {
            compiled = _compiled;
            if (compiled is null)
            {
                compiled = new ImmutablePropertyTransformer<T, TProperty>(
                    PropertyName,
                    _read,
                    _write,
                    _rules.ToArray());
                _compiled = compiled;
            }

            return compiled;
        }
    }

    ICompiledPropertyTransformer<T> IPropertyConfiguration<T>.Compile() => Compile();

    private PropertyTransformer<T, TProperty> AddCore(TransformerRule<TProperty> rule)
    {
        lock (_owner.Gate)
        {
            _rules.Add(rule);
            _compiled = null;
            _owner.InvalidateCompiled();
        }

        return this;
    }
}

/// <summary>Reads one property, runs its compiled rules, and writes the result back.</summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
internal sealed class ImmutablePropertyTransformer<T, TProperty> : ICompiledPropertyTransformer<T>
{
    private readonly Func<T, TProperty> _read;
    private readonly Func<T, TProperty, T> _write;
    private readonly TransformerRule<TProperty>[] _rules;

    internal ImmutablePropertyTransformer(
        string propertyName,
        Func<T, TProperty> read,
        Func<T, TProperty, T> write,
        TransformerRule<TProperty>[] rules)
    {
        PropertyName = propertyName;
        _read = read;
        _write = write;
        _rules = rules;
    }

    internal string PropertyName { get; }

    public ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = _read(instance);
        var rules = _rules;
        try
        {
            for (var i = 0; i < rules.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pending = rules[i].TransformAsync(value, cancellationToken);
                if (!pending.IsCompletedSuccessfully)
                {
                    return ContinueAsync(pending, rules, i + 1, instance, cancellationToken);
                }

                value = pending.Result;
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new PropertyTransformationException(PropertyName, ex);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<T>(_write(instance, value));
    }

    private async ValueTask<T> ContinueAsync(
        ValueTask<TProperty> pending,
        TransformerRule<TProperty>[] rules,
        int index,
        T instance,
        CancellationToken cancellationToken)
    {
        TProperty value;
        try
        {
            value = await pending.ConfigureAwait(false);
            for (; index < rules.Length; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                value = await rules[index].TransformAsync(value, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new PropertyTransformationException(PropertyName, ex);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return _write(instance, value);
    }
}

/// <summary>
/// Mutable property configuration stored on <see cref="Transformer{T}"/>.
/// The property type stays on <see cref="PropertyTransformer{T, TProperty}"/>, so the list needs one non-generic handle.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
internal interface IPropertyConfiguration<T>
{
    ICompiledPropertyTransformer<T> Compile();
}

/// <summary>One compiled property runner inside <see cref="ImmutableTransformer{T}"/>.</summary>
/// <typeparam name="T">The object being transformed.</typeparam>
internal interface ICompiledPropertyTransformer<T>
{
    ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken);
}
