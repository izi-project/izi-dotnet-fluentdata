using System.Linq.Expressions;
using System.Reflection;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Configures and runs property transformers for one object. This is the only transformer type.
/// </summary>
/// <typeparam name="T">The object being transformed. Only its properties change.</typeparam>
/// <remarks>
/// <para>
/// Property transformers run in the order they were registered. Rules added to the returned
/// <see cref="PropertyTransformer{T, TProperty}"/> stay inside that binding.
/// Configuration stays mutable between executions. <see cref="TransformAsync"/> reuses a compiled runner
/// taken when the configuration last changed. A registration on another thread does not change
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
    private readonly List<IPropertyConfiguration<T>> _properties = [];
    private volatile ImmutableTransformer<T>? _compiled;

    /// <summary>
    /// Registers a same-type transformer for one property. The property is read, the rules run, and the
    /// result is written back to that property.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">A direct property access, for example <c>customer =&gt; customer.Name</c>.</param>
    /// <returns>The property transformer, so same-type steps can be added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="property"/> is not a simple settable property access.
    /// </exception>
    public PropertyTransformer<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> property)
    {
        var (name, read, write, requiresClone) = PropertyAccess.Compile(property);
        var propertyTransformer = new PropertyTransformer<T, TProperty>(this, name, read, write, requiresClone);
        lock (_gate)
        {
            _properties.Add(propertyTransformer);
            InvalidateCompiled();
        }

        return propertyTransformer;
    }

    /// <summary>Runs the registered property transformers in registration order.</summary>
    /// <param name="instance">
    /// The object to transform. A class with settable properties is updated on this instance. A struct is not
    /// mutated, and neither is a record or other object whose properties are init-only: those are cloned and
    /// the clone is returned.
    /// </param>
    /// <param name="cancellationToken">Checked before the run and between properties and rules.</param>
    /// <returns>
    /// The transformed value. Callers must reassign it (<c>value = await transformer.TransformAsync(value)</c>).
    /// That assignment is required for structs and for records or other init-only properties, because the input
    /// is not mutated. When every rule finishes successfully without waiting, the returned
    /// <see cref="ValueTask{TResult}"/> is already completed.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is cancelled before a rule runs. Cancellation is not wrapped.
    /// </exception>
    /// <exception cref="Izi.FluentData.Transformer.Exceptions.PropertyTransformationException">A property rule threw.</exception>
    public ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Compiled().TransformAsync(instance, cancellationToken);
    }

    /// <summary>Drops the cached runner. Callers already hold <see cref="Gate"/>.</summary>
    internal void InvalidateCompiled() => _compiled = null;

    internal object Gate => _gate;

    private ImmutableTransformer<T> Compiled()
    {
        // The volatile read stays off the lock once a runner has been published.
        var compiled = _compiled;
        if (compiled is not null)
        {
            return compiled;
        }

        lock (_gate)
        {
            // Another thread may have compiled while this one waited.
            compiled = _compiled;
            if (compiled is null)
            {
                compiled = Compile();
                _compiled = compiled;
            }

            return compiled;
        }
    }

    private ImmutableTransformer<T> Compile()
    {
        var count = _properties.Count;
        if (count == 0)
        {
            // The empty array is shared. The runner object itself is cached by Compiled.
            return new ImmutableTransformer<T>(Array.Empty<ICompiledPropertyTransformer<T>>());
        }

        // Copy the property list. A registration after this returns does not change the runner.
        var steps = new ICompiledPropertyTransformer<T>[count];
        for (var i = 0; i < count; i++)
        {
            steps[i] = _properties[i].Compile();
        }

        return new ImmutableTransformer<T>(steps);
    }
}

/// <summary>The compiled property runners for one <see cref="Transformer{T}"/> configuration.</summary>
/// <typeparam name="T">The object being transformed.</typeparam>
internal sealed class ImmutableTransformer<T>
{
    private readonly ICompiledPropertyTransformer<T>[] _properties;

    internal ImmutableTransformer(ICompiledPropertyTransformer<T>[] properties)
    {
        _properties = properties;
    }

    internal ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var steps = _properties;
        var cloned = false;
        for (var i = 0; i < steps.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (steps[i].RequiresClone && !cloned)
            {
                instance = PropertyAccess.ShallowClone(instance);
                cloned = true;
            }

            var pending = steps[i].ApplyAsync(instance, cancellationToken);

            // Read a completed step here. ContinueAsync is separate so the state machine
            // is built only when a step actually waits.
            if (!pending.IsCompletedSuccessfully)
            {
                return ContinueAsync(pending, steps, i + 1, cloned, cancellationToken);
            }

            instance = pending.Result;
        }

        return new ValueTask<T>(instance);
    }

    private static async ValueTask<T> ContinueAsync(
        ValueTask<T> pending,
        ICompiledPropertyTransformer<T>[] steps,
        int index,
        bool cloned,
        CancellationToken cancellationToken)
    {
        var instance = await pending.ConfigureAwait(false);
        for (; index < steps.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (steps[index].RequiresClone && !cloned)
            {
                instance = PropertyAccess.ShallowClone(instance);
                cloned = true;
            }

            instance = await steps[index].ApplyAsync(instance, cancellationToken).ConfigureAwait(false);
        }

        return instance;
    }
}

internal static class PropertyAccess
{
    private static readonly Func<object, object> CloneInstance = CreateMemberwiseClone();

    internal static T ShallowClone<T>(T instance) => (T)CloneInstance(instance!);

    public static (string Name, Func<T, TProperty> Read, Func<T, TProperty, T> Write, bool RequiresClone) Compile<T, TProperty>(
        Expression<Func<T, TProperty>> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.Body is IndexExpression
            || property.Body is MethodCallExpression { Method: { IsSpecialName: true, Name: "get_Item" } })
        {
            throw new ArgumentException("Indexers are not supported.", nameof(property));
        }

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
            if (typeof(T).IsValueType)
            {
                return (info.Name, read, StructInitWrite<T, TProperty>(setter), false);
            }

            return (info.Name, read, ClassInitWrite<T, TProperty>(setter), true);
        }

        var instance = Expression.Parameter(typeof(T), "instance");
        var value = Expression.Parameter(typeof(TProperty), "value");
        var assign = Expression.Assign(Expression.Property(instance, info), value);
        var write = Expression.Lambda<Func<T, TProperty, T>>(
            Expression.Block(assign, instance),
            instance,
            value).Compile();
        return (info.Name, read, write, false);
    }

    /// <summary>
    /// Init accessors are ordinary methods. Calling the compiled method avoids PropertyInfo.SetValue,
    /// which boxes value-type arguments. The runner clones a class once before the first init write.
    /// </summary>
    private static Func<T, TProperty, T> ClassInitWrite<T, TProperty>(MethodInfo setter)
    {
        var assign = CompileSetter<T, TProperty>(setter);
        return (instance, value) =>
        {
            assign(instance!, value);
            return instance;
        };
    }

    private static Func<T, TProperty, T> StructInitWrite<T, TProperty>(MethodInfo setter)
    {
        var instance = Expression.Parameter(typeof(T), "instance");
        var value = Expression.Parameter(typeof(TProperty), "value");
        var copy = Expression.Variable(typeof(T), "copy");
        var body = Expression.Block(
            [copy],
            Expression.Assign(copy, instance),
            Expression.Call(copy, setter, value),
            copy);
        return Expression.Lambda<Func<T, TProperty, T>>(body, instance, value).Compile();
    }

    private static Action<T, TProperty> CompileSetter<T, TProperty>(MethodInfo setter)
    {
        var instance = Expression.Parameter(typeof(T), "instance");
        var value = Expression.Parameter(typeof(TProperty), "value");
        var call = Expression.Call(instance, setter, value);
        return Expression.Lambda<Action<T, TProperty>>(call, instance, value).Compile();
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
