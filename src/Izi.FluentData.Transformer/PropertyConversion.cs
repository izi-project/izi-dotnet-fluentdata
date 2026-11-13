using System.Linq.Expressions;

namespace Izi.FluentData.Transformer;

/// <summary>
/// A property conversion that has not yet chosen its destination. Call Then to
/// append steps, then <see cref="Into"/> on the latest chain.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TIn">The property type that was read.</typeparam>
/// <typeparam name="TOut">The type produced by the conversion so far.</typeparam>
public sealed class PropertyConversion<T, TIn, TOut> : IPendingConversion<T, TIn>
{
    private readonly Transformer<T> _owner;
    private readonly PropertyRule<T, TIn> _rule;
    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _run;
    private Func<T, TOut, T>? _write;

    internal PropertyConversion(
        Transformer<T> owner,
        PropertyRule<T, TIn> rule,
        Func<TIn, CancellationToken, ValueTask<TOut>> run)
    {
        _owner = owner;
        _rule = rule;
        _run = run;
    }

    /// <summary>Appends a step. <see cref="Into"/> must be called on the returned chain.</summary>
    /// <typeparam name="TNext">The type <paramref name="next"/> produces.</typeparam>
    /// <param name="next">The next rule.</param>
    /// <returns>The continued chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="next"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">This chain is stale or <see cref="Into"/> was already called.</exception>
    public PropertyConversion<T, TIn, TNext> Then<TNext>(ITransformerRule<TOut, TNext> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        lock (_owner.Gate)
        {
            EnsureCurrent();
            var previous = _run;
            var continued = new PropertyConversion<T, TIn, TNext>(
                _owner,
                _rule,
                (value, token) => Append(previous, next, value, token));
            _rule.ReplaceConversion(continued);
            return continued;
        }
    }

    /// <summary>Appends a delegate step. <see cref="Into"/> must be called on the returned chain.</summary>
    /// <typeparam name="TNext">The type <paramref name="next"/> produces.</typeparam>
    /// <param name="next">The next function.</param>
    /// <returns>The continued chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="next"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TIn, TNext> Then<TNext>(
        Func<TOut, CancellationToken, ValueTask<TNext>> next)
    {
        return Then(new TransformerRule<TOut, TNext>(next));
    }

    /// <summary>Writes the converted value to <paramref name="target"/>.</summary>
    /// <param name="target">A direct property access of type <typeparamref name="TOut"/>.</param>
    /// <returns>The parent transformer, so further steps can be registered.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="target"/> is not a simple settable property.</exception>
    /// <exception cref="InvalidOperationException">This chain is stale or <see cref="Into"/> was already called.</exception>
    public Transformer<T> Into(Expression<Func<T, TOut>> target)
    {
        var (_, write) = PropertyAccess.Compile(target);
        lock (_owner.Gate)
        {
            EnsureCurrent();
            _write = write;
            _owner.InvalidateSnapshot();
        }

        return _owner;
    }

    /// <summary>Copies this conversion. Throws if <see cref="Into"/> has not been called.</summary>
    /// <param name="read">The source property reader.</param>
    /// <param name="sourceRules">Same-type rules copied from the property.</param>
    /// <returns>A step that writes the converted value.</returns>
    /// <exception cref="InvalidOperationException"><see cref="Into"/> has not been called.</exception>
    ITransformerStep<T> IPendingConversion<T, TIn>.Snapshot(
        Func<T, TIn> read,
        ITransformerRule<TIn, TIn>[] sourceRules)
    {
        if (_write is null)
        {
            throw new InvalidOperationException(
                "Convert must be finished with Into before TransformAsync. The conversion was not run.");
        }

        return new ConversionStep(read, sourceRules, _run, _write);
    }

    private sealed class ConversionStep : ITransformerStep<T>
    {
        private readonly Func<T, TIn> _read;
        private readonly ITransformerRule<TIn, TIn>[] _sourceRules;
        private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _run;
        private readonly Func<T, TOut, T> _write;

        public ConversionStep(
            Func<T, TIn> read,
            ITransformerRule<TIn, TIn>[] sourceRules,
            Func<TIn, CancellationToken, ValueTask<TOut>> run,
            Func<T, TOut, T> write)
        {
            _read = read;
            _sourceRules = sourceRules;
            _run = run;
            _write = write;
        }

        public ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
            => ApplyConversion(instance, _read, _sourceRules, _run, _write, cancellationToken);

        public ITransformerStep<T> Snapshot() => this;
    }

    private static ValueTask<T> ApplyConversion(
        T instance,
        Func<T, TIn> read,
        ITransformerRule<TIn, TIn>[] sourceRules,
        Func<TIn, CancellationToken, ValueTask<TOut>> run,
        Func<T, TOut, T> write,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = read(instance);
        for (var i = 0; i < sourceRules.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = sourceRules[i].TransformAsync(value, cancellationToken);
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueSourceAsync(
                    pending,
                    sourceRules,
                    i + 1,
                    instance,
                    run,
                    write,
                    cancellationToken);
            }

            value = pending.Result;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var converted = run(value, cancellationToken);
        if (!converted.IsCompletedSuccessfully)
        {
            return ContinueConvertedAsync(converted, instance, write, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<T>(write(instance, converted.Result));
    }

    private static async ValueTask<T> ContinueSourceAsync(
        ValueTask<TIn> pending,
        ITransformerRule<TIn, TIn>[] sourceRules,
        int index,
        T instance,
        Func<TIn, CancellationToken, ValueTask<TOut>> run,
        Func<T, TOut, T> write,
        CancellationToken cancellationToken)
    {
        var value = await pending.ConfigureAwait(false);
        for (; index < sourceRules.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            value = await sourceRules[index].TransformAsync(value, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var converted = await run(value, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return write(instance, converted);
    }

    private static async ValueTask<T> ContinueConvertedAsync(
        ValueTask<TOut> pending,
        T instance,
        Func<T, TOut, T> write,
        CancellationToken cancellationToken)
    {
        var converted = await pending.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return write(instance, converted);
    }

    private static ValueTask<TNext> Append<TNext>(
        Func<TIn, CancellationToken, ValueTask<TOut>> previous,
        ITransformerRule<TOut, TNext> next,
        TIn value,
        CancellationToken cancellationToken)
    {
        var pending = previous(value, cancellationToken);
        if (!pending.IsCompletedSuccessfully)
        {
            return ContinueThenAsync(pending, next, cancellationToken);
        }

        var current = pending.Result;
        cancellationToken.ThrowIfCancellationRequested();
        return next.TransformAsync(current, cancellationToken);
    }

    private static async ValueTask<TNext> ContinueThenAsync<TNext>(
        ValueTask<TOut> pending,
        ITransformerRule<TOut, TNext> next,
        CancellationToken cancellationToken)
    {
        var current = await pending.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return await next.TransformAsync(current, cancellationToken).ConfigureAwait(false);
    }

    private void EnsureCurrent()
    {
        if (!ReferenceEquals(_rule.CurrentConversion, this))
        {
            throw new InvalidOperationException(
                "This conversion chain is stale. Call Then and Into on the chain returned by the latest Then.");
        }

        if (_write is not null)
        {
            throw new InvalidOperationException("Into has already completed this conversion.");
        }
    }

    /// <summary>Continues with a type-changing <see cref="TransformerRules.Map{TIn, TOut}"/>.</summary>
    /// <typeparam name="TNext">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the current value.</param>
    /// <returns>The continued conversion.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TIn, TNext> MapTo<TNext>(Func<TOut, TNext> map)
        => Then(TransformerRules.Map(map));

    /// <summary>Continues with a type-changing <see cref="TransformerRules.MapAsync{TIn, TOut}"/>.</summary>
    /// <typeparam name="TNext">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the current value.</param>
    /// <returns>The continued conversion.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TIn, TNext> MapToAsync<TNext>(
        Func<TOut, CancellationToken, ValueTask<TNext>> map)
        => Then(TransformerRules.MapAsync(map));

}
