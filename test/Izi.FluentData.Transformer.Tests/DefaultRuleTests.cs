using System.Collections;

namespace Izi.FluentData.Transformer.Tests;

public class DefaultRuleTests
{
    [Fact]
    public async Task DefaultIf_and_null_rules_substitute_only_on_a_match()
    {
        Assert.Equal(
            "fallback",
            await TransformerRules.DefaultIf("fallback", string.IsNullOrWhiteSpace).TransformAsync(" "));
        Assert.Equal(
            "kept",
            await TransformerRules.DefaultIf("fallback", string.IsNullOrWhiteSpace).TransformAsync("kept"));
        Assert.Equal("fallback", await TransformerRules.DefaultIfNull("fallback").TransformAsync(null!));
        Assert.Equal("kept", await TransformerRules.DefaultIfNull("fallback").TransformAsync("kept"));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.DefaultIf("x", (Func<string, bool>)null!));
    }

    [Fact]
    public async Task DefaultIfEmpty_uses_collection_count_and_disposes_enumerators()
    {
        Assert.Equal("fallback", await TransformerRules.DefaultIfEmpty("fallback").TransformAsync(null!));
        Assert.Equal("fallback", await TransformerRules.DefaultIfEmpty("fallback").TransformAsync(string.Empty));
        Assert.Equal(" ", await TransformerRules.DefaultIfEmpty("fallback").TransformAsync(" "));
        Assert.Equal("fallback", await TransformerRules.DefaultIfNullOrWhitespace("fallback").TransformAsync(" \t"));
        Assert.Equal("kept", await TransformerRules.DefaultIfNullOrWhitespace("fallback").TransformAsync("kept"));

        Assert.Equal(new[] { 1 }, await TransformerRules.DefaultIfEmpty(new[] { 9 }).TransformAsync(new[] { 1 }));
        Assert.Equal(
            new[] { 9 },
            await TransformerRules.DefaultIfEmpty(new[] { 9 }).TransformAsync(Array.Empty<int>()));

        var counted = new CountOnlyCollection(0);
        var emptyFallback = new CountOnlyCollection(-1);
        Assert.Same(emptyFallback, await TransformerRules.DefaultIfEmpty(emptyFallback).TransformAsync(counted));
        Assert.Equal(0, counted.Enumerations);

        var filled = new CountOnlyCollection(2);
        Assert.Same(filled, await TransformerRules.DefaultIfEmpty(emptyFallback).TransformAsync(filled));
        Assert.Equal(0, filled.Enumerations);

        DisposableSequence.Disposals = 0;
        await TransformerRules.DefaultIfEmpty(new DisposableSequence()).TransformAsync(new DisposableSequence());
        Assert.Equal(1, DisposableSequence.Disposals);

        Assert.Equal(8, await TransformerRules.DefaultIfEmpty(4).TransformAsync(8));
    }

    [Fact]
    public async Task Threshold_and_sentinel_defaults_leave_null_alone()
    {
        Assert.Equal(5, await TransformerRules.DefaultIfMinValue<int>(5).TransformAsync(int.MinValue));
        Assert.Equal(3, await TransformerRules.DefaultIfMinValue<int>(5).TransformAsync(3));
        Assert.Equal(5, await TransformerRules.DefaultIfMaxValue<int>(5).TransformAsync(int.MaxValue));
        Assert.Equal("fallback", await TransformerRules.DefaultIfMinValue("b", "fallback").TransformAsync("a"));
        Assert.Equal("c", await TransformerRules.DefaultIfMinValue("b", "fallback").TransformAsync("c"));
        Assert.Null(await TransformerRules.DefaultIfMinValue("b", "fallback").TransformAsync(null!));
        Assert.Equal("fallback", await TransformerRules.DefaultIfMaxValue("b", "fallback").TransformAsync("b"));
        Assert.Equal("a", await TransformerRules.DefaultIfMaxValue("b", "fallback").TransformAsync("a"));

        var replacement = new DateTime(2020, 1, 2);
        Assert.Equal(
            replacement,
            await TransformerRules.DefaultIfMinValue(replacement).TransformAsync(DateTime.MinValue));
        Assert.Equal(
            replacement,
            await TransformerRules.DefaultIfMaxValue(replacement).TransformAsync(DateTime.MaxValue));
        Assert.Equal(
            new DateTime(2020, 6, 1),
            await TransformerRules.DefaultIfMinValue(replacement).TransformAsync(new DateTime(2020, 6, 1)));

        var offset = new DateTimeOffset(2020, 1, 2, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(offset, await TransformerRules.DefaultIfMinValue(offset).TransformAsync(DateTimeOffset.MinValue));
        Assert.Equal(offset, await TransformerRules.DefaultIfMaxValue(offset).TransformAsync(DateTimeOffset.MaxValue));
        Assert.Equal(
            TimeSpan.FromHours(1),
            await TransformerRules.DefaultIfMinValue(TimeSpan.FromHours(1)).TransformAsync(TimeSpan.MinValue));
        Assert.Equal(
            TimeSpan.FromHours(1),
            await TransformerRules.DefaultIfMaxValue(TimeSpan.FromHours(1)).TransformAsync(TimeSpan.MaxValue));
        Assert.Equal(
            new DateOnly(2020, 1, 2),
            await TransformerRules.DefaultIfMinValue(new DateOnly(2020, 1, 2)).TransformAsync(DateOnly.MinValue));
        Assert.Equal(
            new DateOnly(2020, 1, 2),
            await TransformerRules.DefaultIfMaxValue(new DateOnly(2020, 1, 2)).TransformAsync(DateOnly.MaxValue));
        Assert.Equal(
            new TimeOnly(1, 2),
            await TransformerRules.DefaultIfMinValue(new TimeOnly(1, 2)).TransformAsync(TimeOnly.MinValue));
        Assert.Equal(
            new TimeOnly(1, 2),
            await TransformerRules.DefaultIfMaxValue(new TimeOnly(1, 2)).TransformAsync(TimeOnly.MaxValue));
    }

    private sealed class CountOnlyCollection(int count) : ICollection
    {
        public int Enumerations { get; private set; }

        public int Count { get; } = count;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public void CopyTo(Array array, int index) => throw new NotSupportedException();

        public IEnumerator GetEnumerator()
        {
            Enumerations++;
            throw new InvalidOperationException("The empty check enumerated a collection.");
        }
    }

    private struct DisposableSequence : IEnumerable
    {
        public static int Disposals;

        public IEnumerator GetEnumerator() => new Enumerator();

        private sealed class Enumerator : IEnumerator, IDisposable
        {
            public object? Current => null;

            public bool MoveNext() => false;

            public void Reset()
            {
            }

            public void Dispose() => Disposals++;
        }
    }
}
