using System.Runtime.CompilerServices;

namespace Izi.FluentData.Validation;

/// <summary>
/// Ordering for the comparison and range rules.
/// Null and NaN fail. Strings are compared ordinally. The checks do not allocate.
/// </summary>
internal static class Order
{
    /// <summary>Rejects a null or NaN bound when the rule is created.</summary>
    /// <typeparam name="T">The bound type.</typeparam>
    /// <param name="bound">The bound.</param>
    /// <param name="name">The parameter name.</param>
    internal static void RejectBound<T>(T bound, string name)
    {
        if (bound is null)
        {
            throw new ArgumentNullException(name);
        }

        if (IsNaN(bound))
        {
            throw new ArgumentOutOfRangeException(name, "NaN is not a valid bound.");
        }
    }

    /// <summary>True when <paramref name="value"/> is strictly less than <paramref name="threshold"/>.</summary>
    internal static bool LessThan<T>(T value, T threshold)
        where T : IComparable<T>
        => TryCompare(value, threshold, out var result) && result < 0;

    /// <summary>True when <paramref name="value"/> is less than or equal to <paramref name="threshold"/>.</summary>
    internal static bool LessThanOrEqual<T>(T value, T threshold)
        where T : IComparable<T>
        => TryCompare(value, threshold, out var result) && result <= 0;

    /// <summary>True when <paramref name="value"/> is strictly greater than <paramref name="threshold"/>.</summary>
    internal static bool GreaterThan<T>(T value, T threshold)
        where T : IComparable<T>
        => TryCompare(value, threshold, out var result) && result > 0;

    /// <summary>True when <paramref name="value"/> is greater than or equal to <paramref name="threshold"/>.</summary>
    internal static bool GreaterThanOrEqual<T>(T value, T threshold)
        where T : IComparable<T>
        => TryCompare(value, threshold, out var result) && result >= 0;

    /// <summary>True when <paramref name="value"/> is inside the inclusive range.</summary>
    internal static bool InRange<T>(T value, T min, T max)
        where T : IComparable<T>
        => TryCompare(value, min, out var low) && low >= 0 && TryCompare(value, max, out var high) && high <= 0;

    /// <summary>True when <paramref name="value"/> is outside the inclusive range.</summary>
    internal static bool Outside<T>(T value, T min, T max)
        where T : IComparable<T>
        => TryCompare(value, min, out var low) && TryCompare(value, max, out var high) && (low < 0 || high > 0);

    /// <summary>Compares two bounds that have already been rejected if null or NaN.</summary>
    internal static int CompareBounds<T>(T left, T right)
        where T : IComparable<T>
        => CompareNonNull(ref left, ref right);

    private static bool TryCompare<T>(T value, T threshold, out int result)
        where T : IComparable<T>
    {
        if (value is null || IsNaN(value))
        {
            result = 0;
            return false;
        }

        result = CompareNonNull(ref value, ref threshold);
        return true;
    }

    private static int CompareNonNull<T>(ref T value, ref T threshold)
        where T : IComparable<T>
    {
        if (typeof(T) == typeof(string))
        {
            return string.CompareOrdinal(Unsafe.As<T, string>(ref value), Unsafe.As<T, string>(ref threshold));
        }

        return value.CompareTo(threshold);
    }

    private static bool IsNaN<T>(T value)
    {
        if (typeof(T) == typeof(double))
        {
            return double.IsNaN(Unsafe.As<T, double>(ref value));
        }

        if (typeof(T) == typeof(float))
        {
            return float.IsNaN(Unsafe.As<T, float>(ref value));
        }

        if (typeof(T) == typeof(Half))
        {
            return Half.IsNaN(Unsafe.As<T, Half>(ref value));
        }

        return false;
    }
}
