using System.Collections;
using System.Numerics;

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
    public static TransformerRule<TSource, TSource> DefaultIf<TSource>(
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
    public static TransformerRule<TSource, TSource> DefaultIfNull<TSource>(TSource defaultValue)
        => DefaultIf(defaultValue, source => source is null);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> for null, an empty string, an empty <see cref="ICollection"/>,
    /// or an <see cref="IEnumerable"/> with no elements. The enumerator is disposed. Other values pass through.
    /// A collection that implements <see cref="ICollection"/> uses <see cref="ICollection.Count"/> and
    /// is not enumerated.
    /// </summary>
    /// <typeparam name="TSource">The type flowing through the rule.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TSource, TSource> DefaultIfEmpty<TSource>(TSource defaultValue)
        => DefaultIf(defaultValue, IsEmpty);

    /// <summary>Returns <paramref name="defaultValue"/> when the string is null, empty, or whitespace.</summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<string, string> DefaultIfNullOrWhitespace(string defaultValue)
        => DefaultIf(defaultValue, string.IsNullOrWhiteSpace);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is at or below
    /// <see cref="IMinMaxValue{TSelf}.MinValue"/>.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T, T> DefaultIfMinValue<T>(T defaultValue)
        where T : IComparable<T>, IMinMaxValue<T>
        => DefaultIfMinValue(T.MinValue, defaultValue);

    /// <summary>
    /// Returns <paramref name="defaultValue"/> when the value is at or above
    /// <see cref="IMinMaxValue{TSelf}.MaxValue"/>.
    /// </summary>
    /// <typeparam name="T">The type the rule reads and returns.</typeparam>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<T, T> DefaultIfMaxValue<T>(T defaultValue)
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
    public static TransformerRule<T, T> DefaultIfMinValue<T>(T minimum, T defaultValue)
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
    public static TransformerRule<T, T> DefaultIfMaxValue<T>(T maximum, T defaultValue)
        where T : IComparable<T>
        => DefaultIf(defaultValue, source => source is not null && source.CompareTo(maximum) >= 0);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTime.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTime, DateTime> DefaultIfMinValue(DateTime defaultValue)
        => DefaultIfMinValue<DateTime>(DateTime.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTimeOffset.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTimeOffset, DateTimeOffset> DefaultIfMinValue(DateTimeOffset defaultValue)
        => DefaultIfMinValue<DateTimeOffset>(DateTimeOffset.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeSpan.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeSpan, TimeSpan> DefaultIfMinValue(TimeSpan defaultValue)
        => DefaultIfMinValue<TimeSpan>(TimeSpan.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateOnly.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateOnly, DateOnly> DefaultIfMinValue(DateOnly defaultValue)
        => DefaultIfMinValue<DateOnly>(DateOnly.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeOnly.MinValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeOnly, TimeOnly> DefaultIfMinValue(TimeOnly defaultValue)
        => DefaultIfMinValue<TimeOnly>(TimeOnly.MinValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTime.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTime, DateTime> DefaultIfMaxValue(DateTime defaultValue)
        => DefaultIfMaxValue<DateTime>(DateTime.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateTimeOffset.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateTimeOffset, DateTimeOffset> DefaultIfMaxValue(DateTimeOffset defaultValue)
        => DefaultIfMaxValue<DateTimeOffset>(DateTimeOffset.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeSpan.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeSpan, TimeSpan> DefaultIfMaxValue(TimeSpan defaultValue)
        => DefaultIfMaxValue<TimeSpan>(TimeSpan.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="DateOnly.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<DateOnly, DateOnly> DefaultIfMaxValue(DateOnly defaultValue)
        => DefaultIfMaxValue<DateOnly>(DateOnly.MaxValue, defaultValue);

    /// <summary>
    /// Substitutes <paramref name="defaultValue"/> when the value is <see cref="TimeOnly.MaxValue"/>.
    /// </summary>
    /// <param name="defaultValue">Value returned when the condition matches.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TimeOnly, TimeOnly> DefaultIfMaxValue(TimeOnly defaultValue)
        => DefaultIfMaxValue<TimeOnly>(TimeOnly.MaxValue, defaultValue);

    private static bool IsEmpty<TSource>(TSource source)
    {
        // string is enumerable, so it has to be answered from its length before the sequence branch.
        // ICollection.Count avoids moving an enumerator that may throw when it is walked.
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

        if (source is not IEnumerable enumerable)
        {
            return false;
        }

        var enumerator = enumerable.GetEnumerator();
        try
        {
            return !enumerator.MoveNext();
        }
        finally
        {
            // A non-generic IEnumerator is not disposed by a using declaration.
            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
