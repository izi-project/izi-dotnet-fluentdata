using System.Linq.Expressions;
using System.Reflection;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Configures and runs property rules for one object. This is the only transformer type.
/// </summary>
/// <typeparam name="T">The object being transformed. Only its properties change.</typeparam>
/// <remarks>
/// <para>
/// Property rules run in the order they were registered. Rules added to the returned
/// <see cref="PropertyRule{T, TProperty}"/> stay inside that binding.
/// Configuration stays mutable between executions. <see cref="TransformAsync"/> reuses a snapshot of the
/// step list taken when the configuration last changed. A registration on another thread does not change
/// a pipeline that is already running, and the next run sees the new rules.
/// </para>
/// <para>
/// <see cref="TransformAsync"/> returns the updated value. A class with ordinary setters is changed in place,
/// and that same instance is returned. A struct is not mutated: the updated copy is the return value, so the
/// caller must reassign it (<c>value = await transformer.TransformAsync(value)</c>). Records and other
/// init-only properties are cloned before they are written; the input is left unchanged and must be replaced
/// with the returned clone the same way.
/// </para>
/// <para>
/// A type with no settable or init property, such as <see cref="string"/>, cannot use <see cref="RuleFor{TProperty}"/>.
/// <see cref="TransformAsync"/> with no rules returns the input unchanged.
/// </para>
/// <para>
/// <see cref="ITransformer{T}"/> is the type to inject or mock. This class is the implementation.
/// The library does not reference a dependency-injection package.
/// </para>
/// </remarks>
public sealed class Transformer<T> : ITransformer<T>
{
    private readonly object _gate = new();
    private readonly List<ITransformerStep<T>> _steps = [];
    private ITransformerStep<T>[]? _cachedSteps;

    /// <summary>
    /// Registers a same-type rule for one property. The property is read, the rules run, and the
    /// result is written back to that property.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">A direct property access, for example <c>customer =&gt; customer.Name</c>.</param>
    /// <returns>The property rule, so same-type steps can be added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="property"/> is not a simple settable property access.
    /// </exception>
    public PropertyRule<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> property)
    {
        var (read, write) = PropertyAccess.Compile(property);
        var rule = new PropertyRule<T, TProperty>(this, read, write);
        lock (_gate)
        {
            _steps.Add(rule);
            _cachedSteps = null;
        }

        return rule;
    }

    /// <summary>Runs the registered property rules in registration order.</summary>
    /// <param name="instance">
    /// The object to transform. A class with settable properties is updated on this instance. A struct is not
    /// mutated, and neither is a record or other object whose properties are init-only: those are cloned and
    /// the clone is returned.
    /// </param>
    /// <param name="cancellationToken">Checked before the run and between steps.</param>
    /// <returns>
    /// The transformed value. Callers must reassign it (<c>value = await transformer.TransformAsync(value)</c>).
    /// That assignment is required for structs and for records or other init-only properties, because the input
    /// is not mutated. When every step finishes successfully without waiting, the returned
    /// <see cref="ValueTask{TResult}"/> is already completed.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is cancelled before a step runs.
    /// </exception>
    public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ITransformerStep<T>[] steps;
        lock (_gate)
        {
            steps = _cachedSteps ??= SnapshotSteps();
        }

        // Successes that are already complete stay on this call. Faulted, cancelled, and incomplete
        // tasks are awaited so their exceptions surface from the returned ValueTask.
        for (var i = 0; i < steps.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = steps[i].ApplyAsync(instance, cancellationToken);
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueStepsAsync(pending, steps, i + 1, cancellationToken);
            }

            instance = pending.Result;
        }

        return new ValueTask<T>(instance);
    }

    /// <summary>Drops the cached pipeline. Callers already hold <see cref="Gate"/>.</summary>
    internal void InvalidateSnapshot() => _cachedSteps = null;

    private ITransformerStep<T>[] SnapshotSteps()
    {
        var count = _steps.Count;
        if (count == 0)
        {
            return Array.Empty<ITransformerStep<T>>();
        }

        var steps = new ITransformerStep<T>[count];
        for (var i = 0; i < count; i++)
        {
            steps[i] = _steps[i].Snapshot();
        }

        return steps;
    }

    private static async ValueTask<T> ContinueStepsAsync(
        ValueTask<T> pending,
        ITransformerStep<T>[] steps,
        int index,
        CancellationToken cancellationToken)
    {
        var instance = await pending.ConfigureAwait(false);
        for (; index < steps.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            instance = await steps[index].ApplyAsync(instance, cancellationToken).ConfigureAwait(false);
        }

        return instance;
    }

    internal object Gate => _gate;
}

internal interface ITransformerStep<T>
{
    ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken);

    ITransformerStep<T> Snapshot();
}

internal static class PropertyAccess
{
    private static readonly Func<object, object> CloneInstance = CreateMemberwiseClone();

    public static (Func<T, TProperty> Read, Func<T, TProperty, T> Write) Compile<T, TProperty>(
        Expression<Func<T, TProperty>> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.Body is not MemberExpression { Member: PropertyInfo info, Expression: ParameterExpression })
        {
            throw new ArgumentException(
                "RuleFor requires a simple property access, for example customer => customer.Name.",
                nameof(property));
        }

        if (info.GetIndexParameters().Length > 0)
        {
            throw new ArgumentException("Indexers are not supported.", nameof(property));
        }

        var getter = info.GetMethod;
        var setter = info.SetMethod;
        if (getter is null || !getter.IsPublic || getter.IsStatic)
        {
            throw new ArgumentException(
                $"Property '{info.Name}' must have a public instance getter.",
                nameof(property));
        }

        if (setter is null || !setter.IsPublic || setter.IsStatic)
        {
            throw new ArgumentException(
                $"Property '{info.Name}' has no public settable or init accessor. " +
                "RuleFor needs a settable or init property. Transformer<T> only changes properties.",
                nameof(property));
        }

        var read = property.Compile();
        if (IsInitOnly(setter))
        {
            return (read, InitOnlyWrite<T, TProperty>(info));
        }

        var instance = Expression.Parameter(typeof(T), "instance");
        var value = Expression.Parameter(typeof(TProperty), "value");
        var assign = Expression.Assign(Expression.Property(instance, info), value);
        var write = Expression.Lambda<Func<T, TProperty, T>>(
            Expression.Block(assign, instance),
            instance,
            value).Compile();
        return (read, write);
    }

    /// <summary>
    /// Init-only properties cannot be assigned in an expression tree.
    /// Classes, including records, are shallow-cloned and then set so the caller's instance is left
    /// unchanged. Structs are boxed, set, and unboxed, which returns the updated copy.
    /// </summary>
    private static Func<T, TProperty, T> InitOnlyWrite<T, TProperty>(PropertyInfo property)
    {
        return (instance, value) =>
        {
            if (typeof(T).IsValueType)
            {
                object box = instance!;
                property.SetValue(box, value);
                return (T)box;
            }

            var clone = (T)CloneInstance(instance!);
            property.SetValue(clone, value);
            return clone;
        };
    }

    private static bool IsInitOnly(MethodInfo setter)
    {
        foreach (var modifier in setter.ReturnParameter.GetRequiredCustomModifiers())
        {
            if (modifier.FullName == "System.Runtime.CompilerServices.IsExternalInit")
            {
                return true;
            }
        }

        return false;
    }

    private static Func<object, object> CreateMemberwiseClone()
    {
        var method = typeof(object).GetMethod(
            "MemberwiseClone",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (Func<object, object>)method!.CreateDelegate(typeof(Func<object, object>));
    }
}
