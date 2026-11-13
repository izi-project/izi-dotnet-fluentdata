using System.Linq.Expressions;
using System.Reflection;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Configures and runs property rules and whole-value steps for one object.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <remarks>
/// <para>
/// Steps run in the order they were registered. A <see cref="RuleFor{TProperty}"/> binding occupies
/// that position; rules added to the returned <see cref="PropertyRule{T, TProperty}"/> stay inside it.
/// Configuration stays mutable between executions. <see cref="TransformAsync"/> copies the step list
/// and each property's rules before it runs, so a registration on another thread does not change the
/// pipeline that is already running.
/// </para>
/// <para>
/// This type is separate from <see cref="Transformer{TIn, TOut}"/>, which remains a single immutable
/// value-to-value pipeline.
/// </para>
/// </remarks>
public sealed class Transformer<T>
{
    private readonly object _gate = new();
    private readonly List<ITransformerStep<T>> _steps = [];

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
        }

        return rule;
    }

    /// <summary>Appends a whole-value step. The function returns the next instance.</summary>
    /// <param name="transform">The step. It receives the current instance and the cancellation token.</param>
    /// <returns>This transformer, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public Transformer<T> Transform(Func<T, CancellationToken, ValueTask<T>> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        lock (_gate)
        {
            _steps.Add(new WholeValueStep<T>(transform));
        }

        return this;
    }

    /// <summary>Runs the registered steps in registration order.</summary>
    /// <param name="instance">The object to transform. Struct updates are returned; they are not written back to the caller.</param>
    /// <param name="cancellationToken">Checked before the run and between steps.</param>
    /// <returns>The transformed instance.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> is cancelled before a step runs.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// A <see cref="PropertyRule{T, TProperty}.Convert{TOut}"/> was not finished with
    /// <see cref="PropertyConversion{T, TIn, TOut}.Into"/>.
    /// </exception>
    public async ValueTask<T> TransformAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ITransformerStep<T>[] steps;
        lock (_gate)
        {
            steps = new ITransformerStep<T>[_steps.Count];
            for (var i = 0; i < _steps.Count; i++)
            {
                steps[i] = _steps[i].Snapshot();
            }
        }

        foreach (var step in steps)
        {
            cancellationToken.ThrowIfCancellationRequested();
            instance = await step.ApplyAsync(instance, cancellationToken).ConfigureAwait(false);
        }

        return instance;
    }

    internal object Gate => _gate;
}

/// <summary>
/// Same-type rules for one property. <see cref="Convert{TOut}"/> switches the value to another type
/// and must be finished with <see cref="PropertyConversion{T, TIn, TOut}.Into"/>.
/// </summary>
/// <typeparam name="T">The object being transformed.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public sealed class PropertyRule<T, TProperty> : ITransformerStep<T>
{
    private readonly Transformer<T> _owner;
    private readonly Func<T, TProperty> _read;
    private readonly Func<T, TProperty, T> _write;
    private readonly List<ITransformerRule<TProperty, TProperty>> _rules = [];
    private IPendingConversion<T, TProperty>? _conversion;

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
    /// <exception cref="InvalidOperationException"><see cref="Convert{TOut}"/> was already called.</exception>
    public PropertyRule<T, TProperty> Add(ITransformerRule<TProperty, TProperty> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            if (_conversion is not null)
            {
                throw new InvalidOperationException(
                    "Add preprocesses the property before Convert. This property already has a conversion.");
            }

            _rules.Add(rule);
        }

        return this;
    }

    /// <summary>Appends a same-type delegate, wrapped in a <see cref="TransformerRule{TIn, TOut}"/>.</summary>
    /// <param name="transform">The function that maps the property value.</param>
    /// <returns>This property rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="transform"/> is <see langword="null"/>.</exception>
    public PropertyRule<T, TProperty> Add(Func<TProperty, CancellationToken, ValueTask<TProperty>> transform)
    {
        return Add(new TransformerRule<TProperty, TProperty>(transform));
    }

    /// <summary>
    /// Converts the property value. The same-type rules already added become the conversion input and are
    /// not written back. <see cref="PropertyConversion{T, TIn, TOut}.Into"/> must record the destination
    /// before <see cref="Transformer{T}.TransformAsync"/>; otherwise that call throws
    /// <see cref="InvalidOperationException"/> and no step runs.
    /// </summary>
    /// <typeparam name="TOut">The converted type.</typeparam>
    /// <param name="rule">The conversion.</param>
    /// <returns>The conversion chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">This property was already converted.</exception>
    public PropertyConversion<T, TProperty, TOut> Convert<TOut>(ITransformerRule<TProperty, TOut> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_owner.Gate)
        {
            if (_conversion is not null)
            {
                throw new InvalidOperationException("Convert was already called for this property.");
            }

            var conversion = new PropertyConversion<T, TProperty, TOut>(
                _owner,
                this,
                (value, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return rule.TransformAsync(value, token);
                });
            _conversion = conversion;
            return conversion;
        }
    }

    internal IPendingConversion<T, TProperty>? CurrentConversion => _conversion;

    internal void ReplaceConversion(IPendingConversion<T, TProperty> conversion)
    {
        _conversion = conversion;
    }

    /// <summary>Copies the rules configured at this moment.</summary>
    /// <returns>A step that does not observe later <see cref="Add(ITransformerRule{TProperty, TProperty})"/> calls.</returns>
    internal ITransformerStep<T> Snapshot()
    {
        var rules = _rules.ToArray();
        if (_conversion is not null)
        {
            return _conversion.Snapshot(_read, rules);
        }

        var read = _read;
        var write = _write;
        return new DelegateStep<T>(async (instance, token) =>
        {
            token.ThrowIfCancellationRequested();
            var value = read(instance);
            foreach (var rule in rules)
            {
                token.ThrowIfCancellationRequested();
                value = await rule.TransformAsync(value, token).ConfigureAwait(false);
            }

            token.ThrowIfCancellationRequested();
            return write(instance, value);
        });
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

        var run = _run;
        var write = _write;
        return new DelegateStep<T>(async (instance, token) =>
        {
            token.ThrowIfCancellationRequested();
            var value = read(instance);
            foreach (var rule in sourceRules)
            {
                token.ThrowIfCancellationRequested();
                value = await rule.TransformAsync(value, token).ConfigureAwait(false);
            }

            token.ThrowIfCancellationRequested();
            var converted = await run(value, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            return write(instance, converted);
        });
    }

    private static async ValueTask<TNext> Append<TNext>(
        Func<TIn, CancellationToken, ValueTask<TOut>> previous,
        ITransformerRule<TOut, TNext> next,
        TIn value,
        CancellationToken cancellationToken)
    {
        var current = await previous(value, cancellationToken).ConfigureAwait(false);
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
}

internal interface ITransformerStep<T>
{
    ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken);

    ITransformerStep<T> Snapshot();
}

internal interface IPendingConversion<T, TProperty>
{
    ITransformerStep<T> Snapshot(
        Func<T, TProperty> read,
        ITransformerRule<TProperty, TProperty>[] sourceRules);
}

internal sealed class DelegateStep<T> : ITransformerStep<T>
{
    private readonly Func<T, CancellationToken, ValueTask<T>> _apply;

    public DelegateStep(Func<T, CancellationToken, ValueTask<T>> apply)
    {
        _apply = apply;
    }

    public ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
    {
        return _apply(instance, cancellationToken);
    }

    public ITransformerStep<T> Snapshot()
    {
        return this;
    }
}

internal sealed class WholeValueStep<T> : ITransformerStep<T>
{
    private readonly Func<T, CancellationToken, ValueTask<T>> _transform;

    public WholeValueStep(Func<T, CancellationToken, ValueTask<T>> transform)
    {
        _transform = transform;
    }

    public async ValueTask<T> ApplyAsync(T instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _transform(instance, cancellationToken).ConfigureAwait(false);
    }

    public ITransformerStep<T> Snapshot()
    {
        return this;
    }
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
                "RuleFor and Into require a simple property access, for example customer => customer.Name.",
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
                $"Property '{info.Name}' has no public instance setter. " +
                "Whole-value Transform can still change values that have no settable property.",
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
