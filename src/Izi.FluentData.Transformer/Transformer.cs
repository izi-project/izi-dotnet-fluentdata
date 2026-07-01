using System.Linq.Expressions;
using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Base class for transformers: subclass it and declare per-property pipelines with
/// <see cref="RuleFor{TProperty}"/> in the constructor. Each property is read, run through
/// its pipeline, and the result written back to the same instance.
/// </summary>
/// <typeparam name="T">The (mutable reference) type whose instances are transformed.</typeparam>
public class Transformer<T> : ITransformer<T>
{
    // Each entry reads a property, runs its pipeline, and writes the result back.
    private readonly List<Func<T, CancellationToken, ValueTask>> _propertyTransformers = [];

    protected void RuleFor(Func<TransformerRuleBuilder<T, T>, TransformerRuleBuilder<T, T>> configure)
    {
        RuleFor(x => x, configure);
    }

    /// <summary>
    /// Configures a transformation pipeline for a single property. The pipeline is built up
    /// front via <paramref name="configure"/> (the builder is immutable, so it cannot be
    /// chained after <c>RuleFor</c> returns) and must end at the property's own type so the
    /// result can be assigned back.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="selector">A member-access selector identifying the property, e.g. <c>x =&gt; x.Name</c>.</param>
    /// <param name="configure">Builds the pipeline; must return to the property's own type.</param>
    protected void RuleFor<TProperty>(Expression<Func<T, TProperty>> selector, Func<TransformerRuleBuilder<TProperty, TProperty>, TransformerRuleBuilder<TProperty, TProperty>> configure)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(configure);

        var getter = selector.Compile();
        var setter = BuildSetter(selector);

        // Identity seed so the callback always has a builder to chain onto.
        var seed = new TransformerRuleBuilder<TProperty>();
        var rule = configure(seed).Build();

        _propertyTransformers.Add((instance, ct) =>
        {
            var value = getter(instance);
            var pending = rule.TransformAsync(value, ct);
            // Write back synchronously when the pipeline completed synchronously.
            if (pending.IsCompletedSuccessfully)
            {
                setter(instance, pending.Result);
                return ValueTask.CompletedTask;
            }
            return CompleteWriteBack(pending, setter, instance);
        });

        static async ValueTask CompleteWriteBack(ValueTask<TProperty> pending, Action<T, TProperty> setter, T instance) => setter(instance, await pending);
    }

    /// <inheritdoc />
    public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        var transformers = _propertyTransformers;
        for (var i = 0; i < transformers.Count; i++)
        {
            var pending = transformers[i](instance, cancellationToken);
            // Stay synchronous until a property pipeline actually yields.
            if (!pending.IsCompletedSuccessfully)
                return CompleteTransform(pending, transformers, i + 1, instance, cancellationToken);
        }
        return new ValueTask<T>(instance);
    }

    /// <summary>Resumes the property loop asynchronously once a pipeline has yielded.</summary>
    private static async ValueTask<T> CompleteTransform(ValueTask pending, List<Func<T, CancellationToken, ValueTask>> transformers, int next, T instance, CancellationToken cancellationToken)
    {
        await pending;
        for (var i = next; i < transformers.Count; i++)
            await transformers[i](instance, cancellationToken);
        return instance;
    }

    /// <summary>
    /// Compiles the member-access selector into a setter so the transformed value can be
    /// written back. Targets the same property/field the selector reads.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="selector">A member-access selector.</param>
    /// <returns>A compiled setter for the selected member.</returns>
    /// <exception cref="ArgumentException">The selector is not a direct property/field access.</exception>
    private static Action<T, TProperty> BuildSetter<TProperty>(Expression<Func<T, TProperty>> selector)
    {
        if (selector.Body is ParameterExpression) return static (_,_) => { }; // No-op for the whole-instance rule.

        if (selector.Body is not MemberExpression member)
            throw new ArgumentException(
                "RuleFor selector must target a settable property or field, e.g. x => x.Name.",
                nameof(selector));

        var instance = Expression.Parameter(typeof(T), "instance");
        var value = Expression.Parameter(typeof(TProperty), "value");
        var assignment = Expression.Assign(Expression.MakeMemberAccess(instance, member.Member), value);

        return Expression.Lambda<Action<T, TProperty>>(assignment, instance, value).Compile();
    }
}
