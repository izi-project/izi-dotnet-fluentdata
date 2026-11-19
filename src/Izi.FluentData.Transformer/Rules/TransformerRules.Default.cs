using System.Collections;
using System.Numerics;
using System.Reflection;

namespace Izi.FluentData.Transformer;

public static partial class TransformerRules
{
    /// <summary>
    /// Returns <paramref name="defaultValue"/> when <paramref name="predicate"/> is true, otherwise the
    /// current value.
    /// </summary>
    /// <typeparam name="TSource">The type flowing through the rule.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <param name="predicate">Returns <see langword="true"/> when the current value should be replaced.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static TransformerRule<TSource> DefaultIf<TSource>(
        TSource defaultValue,
        Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Same<TSource>(source => predicate(source) ? defaultValue : source);
    }

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the current value is null. Value types other than
    /// <see cref="Nullable{T}"/> are unchanged.
    /// </summary>
    /// <typeparam name="TSource">The type flowing through the rule.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TSource> DefaultIfNull<TSource>(TSource defaultValue)
        => DefaultIf(defaultValue, source => source is null);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> for null, an empty string, or an empty collection.
    /// Emptiness is taken from <see cref="ICollection"/>, <see cref="ICollection{T}"/>,
    /// <see cref="IReadOnlyCollection{T}"/>, <see cref="string"/>, or an array. Any other
    /// <see cref="IEnumerable"/> is treated as not empty and is never enumerated.
    /// </summary>
    /// <typeparam name="TSource">The type flowing through the rule.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TSource> DefaultIfEmpty<TSource>(TSource defaultValue)
        => DefaultIf(defaultValue, IsEmpty);

    /// <summary>Returns <paramref name="defaultValue"/> when the string is null, empty, or whitespace.</summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<string> DefaultIfNullOrWhitespace(string defaultValue)
        => DefaultIf(defaultValue, string.IsNullOrWhiteSpace);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is at or below
    /// <see cref="IMinMaxValue{TSelf}.MinValue"/>.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T> DefaultIfMinValue<T>(T defaultValue)
        where T : IComparable<T>, IMinMaxValue<T>
        => DefaultIfMinValue(T.MinValue, defaultValue);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is at or above
    /// <see cref="IMinMaxValue{TSelf}.MaxValue"/>.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T> DefaultIfMaxValue<T>(T defaultValue)
        where T : IComparable<T>, IMinMaxValue<T>
        => DefaultIfMaxValue(T.MaxValue, defaultValue);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is less than or equal to <paramref name="minimum"/>.
    /// Null is left unchanged.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="minimum">
    /// Inclusive lower threshold. Smaller non-null values are replaced. Null is left unchanged.
    /// </param>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T> DefaultIfMinValue<T>(T minimum, T defaultValue)
        where T : IComparable<T>
        => DefaultIf(defaultValue, source => source is not null && source.CompareTo(minimum) <= 0);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is greater than or equal to <paramref name="maximum"/>.
    /// Null is left unchanged.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="maximum">
    /// Inclusive upper threshold. Larger non-null values are replaced. Null is left unchanged.
    /// </param>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T> DefaultIfMaxValue<T>(T maximum, T defaultValue)
        where T : IComparable<T>
        => DefaultIf(defaultValue, source => source is not null && source.CompareTo(maximum) >= 0);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTime.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTime> DefaultIfMinValue(DateTime defaultValue)
        => DefaultIfMinValue<DateTime>(DateTime.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTimeOffset.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTimeOffset> DefaultIfMinValue(DateTimeOffset defaultValue)
        => DefaultIfMinValue<DateTimeOffset>(DateTimeOffset.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeSpan.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeSpan> DefaultIfMinValue(TimeSpan defaultValue)
        => DefaultIfMinValue<TimeSpan>(TimeSpan.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateOnly.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateOnly> DefaultIfMinValue(DateOnly defaultValue)
        => DefaultIfMinValue<DateOnly>(DateOnly.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeOnly.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeOnly> DefaultIfMinValue(TimeOnly defaultValue)
        => DefaultIfMinValue<TimeOnly>(TimeOnly.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTime.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTime> DefaultIfMaxValue(DateTime defaultValue)
        => DefaultIfMaxValue<DateTime>(DateTime.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTimeOffset.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTimeOffset> DefaultIfMaxValue(DateTimeOffset defaultValue)
        => DefaultIfMaxValue<DateTimeOffset>(DateTimeOffset.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeSpan.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeSpan> DefaultIfMaxValue(TimeSpan defaultValue)
        => DefaultIfMaxValue<TimeSpan>(TimeSpan.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateOnly.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateOnly> DefaultIfMaxValue(DateOnly defaultValue)
        => DefaultIfMaxValue<DateOnly>(DateOnly.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeOnly.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeOnly> DefaultIfMaxValue(TimeOnly defaultValue)
        => DefaultIfMaxValue<TimeOnly>(TimeOnly.MaxValue, defaultValue);

    private static bool IsEmpty<TSource>(TSource source)
    {
        if (source is null)
        {
            return true;
        }

        if (source is string text)
        {
            return text.Length == 0;
        }

        if (source is ICollection collection)
        {
            return collection.Count == 0;
        }

        if (source is Array array)
        {
            return array.Length == 0;
        }

        return CountedEmpty<TSource>.IsEmpty(source);
    }

    /// <summary>
    /// Resolves <see cref="ICollection{T}"/> or <see cref="IReadOnlyCollection{T}"/> once per source type.
    /// Later calls only read <c>Count</c>. A type with no count is not empty, so a lazy sequence is never walked.
    /// </summary>
    private static class CountedEmpty<TSource>
    {
        public static readonly Func<TSource, bool> IsEmpty = Create();

        private static Func<TSource, bool> Create()
        {
            var counted = FindCountedInterface(typeof(TSource));
            if (counted is null)
            {
                return static _ => false;
            }

            var found = counted.Value;
            var element = found.Interface.GetGenericArguments()[0];
            var helper = (found.Mutable
                ? typeof(MutableCounted<,>)
                : typeof(ReadOnlyCounted<,>)).MakeGenericType(typeof(TSource), element);
            var method = helper.GetMethod(nameof(ReadOnlyCounted<IReadOnlyCollection<int>, int>.IsEmpty), BindingFlags.Public | BindingFlags.Static)!;
            return (Func<TSource, bool>)Delegate.CreateDelegate(typeof(Func<TSource, bool>), method);
        }

        private static (Type Interface, bool Mutable)? FindCountedInterface(Type type)
        {
            if (TryMatch(type, out var matched))
            {
                return matched;
            }

            foreach (var implemented in type.GetInterfaces())
            {
                if (TryMatch(implemented, out matched))
                {
                    return matched;
                }
            }

            return null;
        }

        private static bool TryMatch(Type candidate, out (Type Interface, bool Mutable) matched)
        {
            if (candidate.IsGenericType)
            {
                var definition = candidate.GetGenericTypeDefinition();
                if (definition == typeof(ICollection<>))
                {
                    matched = (candidate, true);
                    return true;
                }

                if (definition == typeof(IReadOnlyCollection<>))
                {
                    matched = (candidate, false);
                    return true;
                }
            }

            matched = default;
            return false;
        }
    }

    private static class ReadOnlyCounted<TCollection, TElement>
        where TCollection : IReadOnlyCollection<TElement>
    {
        public static bool IsEmpty(TCollection value) => value.Count == 0;
    }

    private static class MutableCounted<TCollection, TElement>
        where TCollection : ICollection<TElement>
    {
        public static bool IsEmpty(TCollection value) => value.Count == 0;
    }
}
