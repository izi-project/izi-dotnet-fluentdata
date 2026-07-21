using System.Linq.Expressions;
using System.Reflection;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Base class for transformers: subclass it and, in the constructor, add whole-instance steps directly via
/// <see cref="AddTransformer"/> (or the fluent extensions in <c>TransformerExtensions</c>), and/or declare
/// per-property steps with <see cref="RuleFor{TProperty}"/>. Use <c>RuleFor(x =&gt; x)</c> to transform the whole
/// instance.
/// </summary>
/// <remarks>
/// Unlike a validator, a transformer produces a value rather than a verdict, so there is no pass/fail gating:
/// every registered step runs, in the order it was added, and each step's output becomes the next step's input.
/// A <see cref="RuleFor{TProperty}"/> step reads the selected member, runs its own nested pipeline over that value,
/// then writes the result back into the instance — which is why its selector must be an assignable
/// property/field access (or the identity selector <c>x =&gt; x</c>), not an arbitrary expression.
/// </remarks>
/// <typeparam name="T">The type whose instances are transformed.</typeparam>
public class Transformer<T> : ITransformer<T>
{
    private readonly List<ITransformer<T>> _transformers = [];

    /// <summary>Adds one or more steps that run directly against this transformer's own value, in order.</summary>
    /// <param name="transformers">The steps to append.</param>
    /// <returns>The same transformer, for chaining.</returns>
    public Transformer<T> AddTransformer(params ITransformer<T>[] transformers)
    {
        _transformers.AddRange(transformers);
        return this;
    }

    /// <summary>
    /// Declares steps for the member selected by <paramref name="propertySelector"/>. A fresh
    /// <see cref="Transformer{TProperty}"/> is created, wired in as a step that reads the member, runs the nested
    /// pipeline, and writes the result back, and returned so steps can be chained directly onto it, e.g.
    /// <c>RuleFor(x =&gt; x.Name).Trim().DefaultIfNull("N/A")</c>. Pass <c>x =&gt; x</c> to transform the whole
    /// instance.
    /// </summary>
    /// <typeparam name="TProperty">The member type.</typeparam>
    /// <param name="propertySelector">Selects an assignable member, e.g. <c>x =&gt; x.Name</c>, or <c>x =&gt; x</c>.</param>
    /// <returns>The nested transformer to chain steps onto.</returns>
    public Transformer<TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelector)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);
        var getter = propertySelector.Compile();
        var setter = BuildSetter(propertySelector);
        var nested = new Transformer<TProperty>();
        AddTransformer(new PropertyAdapter<TProperty>(getter, setter, nested));
        return nested;
    }

    /// <summary>
    /// Runs every registered step in order, feeding each step's output into the next, and returns the final value.
    /// </summary>
    /// <param name="instance">The instance to transform.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The transformed value.</returns>
    public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        for (int i = 0; i < _transformers.Count; i++)
        {
            var pending = _transformers[i].TransformAsync(instance, cancellationToken);
            if (pending.IsCompletedSuccessfully)
            {
                instance = pending.Result;
            }
            else
            {
                return TransformSlowAsync(pending, i, cancellationToken);
            }
        }

        return ValueTask.FromResult(instance);
    }

    // Async tail resumed once a step yields; finishes awaiting the current one then drains the rest,
    // threading each result into the next call (mirroring the synchronous path above).
    private async ValueTask<T> TransformSlowAsync(ValueTask<T> pending, int currentIndex, CancellationToken cancellationToken)
    {
        var instance = await pending.ConfigureAwait(false);

        for (int i = currentIndex + 1; i < _transformers.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            instance = await _transformers[i].TransformAsync(instance, cancellationToken).ConfigureAwait(false);
        }

        return instance;
    }

    // Bridges a Transformer<TProperty> into the parent's ITransformer<T> list: reads the member, runs the nested
    // pipeline over it, then writes the result back and returns the (same, for classes) instance. This is what
    // lets RuleFor erase TProperty back to T.
    private sealed class PropertyAdapter<TProperty>(Func<T, TProperty> getter, Func<T, TProperty, T> setter, ITransformer<TProperty> transformer) : ITransformer<T>
    {
        public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
        {
            var pending = transformer.TransformAsync(getter(instance), cancellationToken);
            return pending.IsCompletedSuccessfully
                ? ValueTask.FromResult(setter(instance, pending.Result))
                : SetSlowAsync(instance, pending);
        }

        private async ValueTask<T> SetSlowAsync(T instance, ValueTask<TProperty> pending)
            => setter(instance, await pending.ConfigureAwait(false));
    }

    // Compiles (instance, value) => { instance.Member = value; return instance; } from the selector. Returning the
    // instance (rather than a void setter) keeps the mutable-struct case correct: for a value type the block
    // assigns to and returns the local copy, so the new member value survives.
    private static Func<T, TProperty, T> BuildSetter<TProperty>(Expression<Func<T, TProperty>> selector)
    {
        var body = selector.Body;

        if (body is MemberExpression { Member: PropertyInfo { CanWrite: false } property })
        {
            throw new ArgumentException(
                $"Property '{property.Name}' has no setter, so RuleFor cannot write the transformed value back.",
                nameof(selector));
        }

        if (body is not (ParameterExpression or MemberExpression { Member: PropertyInfo or FieldInfo }))
        {
            throw new ArgumentException(
                $"RuleFor requires an assignable property/field access or the identity selector (x => x); '{selector}' is not assignable.",
                nameof(selector));
        }

        var instanceParam = selector.Parameters[0];
        var valueParam = Expression.Parameter(typeof(TProperty), "value");
        var assign = Expression.Assign(body, valueParam);
        var block = Expression.Block(assign, instanceParam);
        return Expression.Lambda<Func<T, TProperty, T>>(block, instanceParam, valueParam).Compile();
    }
}
