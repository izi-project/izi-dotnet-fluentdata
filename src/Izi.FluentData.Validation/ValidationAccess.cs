using System.Linq.Expressions;
using System.Reflection;

namespace Izi.FluentData.Validation;

/// <summary>A null check for one intermediate member of a property path.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal readonly struct ParentNullCheck<T>
{
    /// <summary>Creates a check that reads one parent.</summary>
    /// <param name="name">The parent path, for example <c>Address</c>.</param>
    /// <param name="isNull">Returns true when that parent is null.</param>
    internal ParentNullCheck(string name, Func<T, bool> isNull)
    {
        Name = name;
        IsNull = isNull;
    }

    /// <summary>The parent path reported when this member is null.</summary>
    internal string Name { get; }

    /// <summary>Returns true when this parent is null.</summary>
    internal Func<T, bool> IsNull { get; }
}

/// <summary>The compiled read of one member path.</summary>
/// <typeparam name="T">The object being validated.</typeparam>
/// <typeparam name="TValue">The value the path reads.</typeparam>
internal readonly struct CompiledAccess<T, TValue>
{
    /// <summary>Creates a compiled path.</summary>
    /// <param name="name">The member chain joined with <c>.</c>.</param>
    /// <param name="read">Reads the leaf. Callers check parents first.</param>
    /// <param name="parents">Intermediate members that can be null, from the root outward.</param>
    internal CompiledAccess(string name, Func<T, TValue> read, ParentNullCheck<T>[] parents)
    {
        Name = name;
        Read = read;
        Parents = parents;
    }

    /// <summary>The leaf path, for example <c>Address.Street</c>.</summary>
    internal string Name { get; }

    /// <summary>Reads the leaf after the parents have been checked.</summary>
    internal Func<T, TValue> Read { get; }

    /// <summary>Intermediate members that can be null.</summary>
    internal ParentNullCheck<T>[] Parents { get; }
}

/// <summary>Compiles a member chain into a name, a reader, and null checks for the parents.</summary>
internal static class ValidationAccess
{
    /// <summary>Compiles <paramref name="selector"/>.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <typeparam name="TValue">The value the selector reads.</typeparam>
    /// <param name="selector">A chain of properties or fields.</param>
    /// <param name="parameterName">The argument name used when the selector is rejected.</param>
    /// <returns>The compiled path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The selector is not a property or field chain.</exception>
    internal static CompiledAccess<T, TValue> Compile<T, TValue>(
        Expression<Func<T, TValue>> selector,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(selector);
        RejectUnsupported(selector.Body, parameterName);

        var chain = new List<MemberInfo>();
        var cursor = Unwrap(selector.Body);
        while (cursor is MemberExpression member)
        {
            chain.Add(member.Member);
            cursor = Unwrap(member.Expression);
        }

        if (cursor is not ParameterExpression || chain.Count == 0)
        {
            throw NotAMember(parameterName);
        }

        chain.Reverse();
        var parameter = selector.Parameters[0];
        Expression access = parameter;
        var names = new string[chain.Count];
        var parents = new List<ParentNullCheck<T>>();
        for (var i = 0; i < chain.Count; i++)
        {
            var member = chain[i];
            names[i] = member.Name;
            access = Expression.MakeMemberAccess(access, member);
            if (i < chain.Count - 1 && CanBeNull(access.Type))
            {
                var parentName = string.Join(".", names, 0, i + 1);
                var isNull = Expression.Lambda<Func<T, bool>>(NullTest(access), parameter).Compile();
                parents.Add(new ParentNullCheck<T>(parentName, isNull));
            }
        }

        if (access.Type != typeof(TValue))
        {
            access = Expression.Convert(access, typeof(TValue));
        }

        var read = Expression.Lambda<Func<T, TValue>>(access, parameter).Compile();
        return new CompiledAccess<T, TValue>(string.Join(".", names), read, parents.ToArray());
    }

    private static void RejectUnsupported(Expression body, string parameterName)
    {
        var cursor = body;
        while (cursor is not null)
        {
            if (cursor is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            {
                cursor = unary.Operand;
                continue;
            }

            if (cursor is IndexExpression
                || cursor is MethodCallExpression { Method.Name: "get_Item" })
            {
                throw new ArgumentException("Indexers are not supported.", parameterName);
            }

            if (cursor is MethodCallExpression)
            {
                throw new ArgumentException("Method calls are not supported.", parameterName);
            }

            if (cursor is not MemberExpression member)
            {
                if (cursor is ParameterExpression)
                {
                    return;
                }

                throw NotAMember(parameterName);
            }

            if (member.Member is PropertyInfo property)
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    throw new ArgumentException("Indexers are not supported.", parameterName);
                }

                var getter = property.GetMethod;
                if (getter is null || getter.IsStatic)
                {
                    throw NotAMember(parameterName);
                }
            }
            else if (member.Member is not FieldInfo { IsStatic: false })
            {
                throw NotAMember(parameterName);
            }

            cursor = member.Expression;
        }
    }

    private static Expression? Unwrap(Expression? expression)
    {
        while (expression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
        {
            expression = unary.Operand;
        }

        return expression;
    }

    private static bool CanBeNull(Type type)
        => !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    /// <summary>
    /// Null test that does not call a user-defined equality operator.
    /// Reference types use reference equality. Nullable value types use <c>HasValue</c>.
    /// </summary>
    private static Expression NullTest(Expression access)
    {
        if (Nullable.GetUnderlyingType(access.Type) is not null)
        {
            return Expression.Not(Expression.Property(access, "HasValue"));
        }

        return Expression.ReferenceEqual(access, Expression.Constant(null, access.Type));
    }

    private static ArgumentException NotAMember(string parameterName)
        => new(
            "RuleFor requires a property or field access, for example order => order.Address.Street.",
            parameterName);
}
