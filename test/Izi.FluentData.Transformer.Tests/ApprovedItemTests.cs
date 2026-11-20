using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer.Tests;

public class ApprovedItemTests
{
    [Fact]
    public async Task Property_access_failures_are_wrapped_unless_fatal()
    {
        var reading = new Transformer<Exploding>();
        reading.RuleFor(item => item.Name).Add(static (value, _) => new ValueTask<string>(value));
        var readError = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await reading.TransformAsync(new Exploding(explodeOnGet: true)).ConfigureAwait(true));
        Assert.Equal("Name", readError.PropertyName);
        Assert.IsType<InvalidOperationException>(readError.InnerException);
        Assert.Equal("get", readError.InnerException!.Message);
        Assert.IsAssignableFrom<TransformationException>(readError);

        var writing = new Transformer<Exploding>();
        writing.RuleFor(item => item.Name).Add(static (value, _) => new ValueTask<string>(value + "!"));
        var writeError = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await writing.TransformAsync(new Exploding(explodeOnSet: true)).ConfigureAwait(true));
        Assert.Equal("Name", writeError.PropertyName);
        Assert.IsType<InvalidOperationException>(writeError.InnerException);
        Assert.Equal("set", writeError.InnerException!.Message);

        var cancelled = new Transformer<Exploding>();
        cancelled.RuleFor(item => item.Name).Add(static (value, _) => new ValueTask<string>(value));
        var cancel = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await cancelled.TransformAsync(new Exploding(cancelOnGet: true)).ConfigureAwait(true));
        Assert.IsNotType<PropertyTransformationException>(cancel);

        var fatal = new Transformer<Exploding>();
        fatal.RuleFor(item => item.Name).Add(static (_, _) => throw new OutOfMemoryException());
        await Assert.ThrowsAsync<OutOfMemoryException>(
            async () => await fatal.TransformAsync(new Exploding()).ConfigureAwait(true));
    }

    [Fact]
    public async Task Null_input_throws_ArgumentNullException()
    {
        var bare = Assert.Throws<ArgumentNullException>(() => new Transformer<string>().TransformAsync(null!));
        Assert.Equal("instance", bare.ParamName);

        var ruled = new Transformer<Customer>();
        ruled.RuleFor(customer => customer.Name).Trim();
        var ruledError = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await ruled.TransformAsync(null!).ConfigureAwait(true));
        Assert.Equal("instance", ruledError.ParamName);

        var nullable = Assert.Throws<ArgumentNullException>(() => new Transformer<int?>().TransformAsync(null));
        Assert.Equal("instance", nullable.ParamName);
        Assert.Equal(0, await new Transformer<int>().TransformAsync(0));
    }

    [Fact]
    public async Task Later_failure_keeps_earlier_class_edits()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Trim();
        transformer.RuleFor(customer => customer.Age).Add(static (_, _) => throw new InvalidOperationException("age"));
        var customer = new Customer { Name = "  ada  ", Age = 1 };

        var error = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await transformer.TransformAsync(customer).ConfigureAwait(true));

        Assert.Equal("Age", error.PropertyName);
        var rule = Assert.IsType<TransformationRuleException>(error.InnerException);
        Assert.IsType<InvalidOperationException>(rule.InnerException);
        Assert.Equal("ada", customer.Name);
        Assert.Equal(1, customer.Age);
    }

    [Fact]
    public async Task Fluent_date_rules_match_the_supported_types()
    {
        var transformer = new Transformer<When>();
        transformer.RuleFor(item => item.Stamp).StartOfDay();
        transformer.RuleFor(item => item.Day).StartOfMonth();
        transformer.RuleFor(item => item.Clock).AddHours(1);
        transformer.RuleFor(item => item.Span).AddDays(1.9);

        var updated = await transformer.TransformAsync(new When
        {
            Stamp = new DateTime(2024, 3, 25, 10, 0, 0),
            Day = new DateOnly(2024, 3, 25),
            Clock = new TimeOnly(10, 0),
            Span = TimeSpan.Zero,
        });

        Assert.Equal(new DateTime(2024, 3, 25), updated.Stamp);
        Assert.Equal(new DateOnly(2024, 3, 1), updated.Day);
        Assert.Equal(new TimeOnly(11, 0), updated.Clock);
        Assert.Equal(TimeSpan.FromDays(1.9), updated.Span);

        var startOfDay = typeof(TransformerRuleExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == "StartOfDay")
            .Select(method => method.GetParameters()[0].ParameterType.GetGenericArguments()[1])
            .ToArray();
        Assert.Contains(typeof(DateTime), startOfDay);
        Assert.Contains(typeof(DateTimeOffset), startOfDay);
        Assert.DoesNotContain(typeof(string), startOfDay);
        Assert.DoesNotContain(typeof(DateOnly), startOfDay);
    }

    [Fact]
    public async Task Abs_saturates_and_normalize_covers_the_full_range()
    {
        Assert.Equal(short.MaxValue, await TransformerRules.Abs<short>().TransformAsync(short.MinValue));
        Assert.Equal(Int128.MaxValue, await TransformerRules.Abs<Int128>().TransformAsync(Int128.MinValue));
        Assert.Equal(0.5f, await TransformerRules.Normalize(-float.MaxValue, float.MaxValue).TransformAsync(0f));
        Assert.Equal(0.5m, await TransformerRules.Normalize(-decimal.MaxValue, decimal.MaxValue).TransformAsync(0m));

        var nearStart = DateTime.MinValue.AddDays(10);
        var error = Assert.Throws<TransformationRuleException>(
            () => TransformerRules.AddBusinessDays<DateTime>(int.MinValue).TransformAsync(nearStart));
        Assert.IsType<ArgumentOutOfRangeException>(error.InnerException);
    }

    [Fact]
    public async Task Creation_rejects_bad_digits_roots_and_non_finite_amounts()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Round<double>(16));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Round<decimal>(29));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Round<float>(7));
        Assert.Equal(1.2d, await TransformerRules.Round<double>(1).TransformAsync(1.23d));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.RootN<double>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.AddDays<DateOnly>(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.AddHours<TimeSpan>(double.PositiveInfinity));
        Assert.Equal(
            new DateOnly(2024, 1, 2),
            await TransformerRules.AddDays<DateOnly>(1.9).TransformAsync(new DateOnly(2024, 1, 1)));
    }

    [Fact]
    public async Task Regex_timeout_and_empty_collections_follow_the_documented_rules()
    {
        var infinite = new Regex("a+");
        var error = Assert.Throws<ArgumentException>(() => TransformerRules.ReplaceRegex(infinite, "x"));
        Assert.Contains("timeout", error.Message, StringComparison.OrdinalIgnoreCase);

        var timed = new Regex("a+", RegexOptions.None, TimeSpan.FromSeconds(1));
        Assert.Equal("x", await TransformerRules.ReplaceRegex(timed, "x").TransformAsync("aaa"));

        var onlyReadOnly = new ReadOnlyOnly(0);
        var fallback = new ReadOnlyOnly(3);
        Assert.Same(fallback, await TransformerRules.DefaultIfEmpty(fallback).TransformAsync(onlyReadOnly));

        var sequence = new YieldOnce();
        Assert.Same(sequence, await TransformerRules.DefaultIfEmpty(new YieldOnce()).TransformAsync(sequence));
        Assert.Equal(0, sequence.Moves);

        var emptySet = new HashSet<int>();
        var setFallback = new HashSet<int> { 9 };
        Assert.Same(setFallback, await TransformerRules.DefaultIfEmpty(setFallback).TransformAsync(emptySet));

        var filledSet = new HashSet<int> { 1 };
        var setRule = TransformerRules.DefaultIfEmpty(setFallback);
        _ = await setRule.TransformAsync(filledSet);
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 200; i++)
        {
            Assert.Same(filledSet, await setRule.TransformAsync(filledSet));
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    [Fact]
    public async Task Mixed_set_and_init_properties_are_cloned_once()
    {
        var transformer = new Transformer<Mixed>();
        transformer.RuleFor(item => item.Label).Add(1);
        transformer.RuleFor(item => item.Score).Add(1);
        _ = await transformer.TransformAsync(new Mixed { Label = 1, Score = 1 });

        var original = new Mixed { Label = 4, Score = 4 };
        long oneClone = 0;
        for (var i = 0; i < 8; i++)
        {
            var sample = new Mixed { Label = 1, Score = 1 };
            var start = GC.GetAllocatedBytesForCurrentThread();
            _ = sample with { Label = sample.Label };
            var used = GC.GetAllocatedBytesForCurrentThread() - start;
            if (used > 0)
            {
                oneClone = used;
                break;
            }
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        var updated = await transformer.TransformAsync(original);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(4, original.Label);
        Assert.Equal(4, original.Score);
        Assert.Equal(5, updated.Label);
        Assert.Equal(5, updated.Score);
        Assert.NotSame(original, updated);
        Assert.True(oneClone > 0);
        Assert.Equal(oneClone, allocated);
    }

    private sealed class Exploding
    {
        private readonly bool _explodeOnGet;
        private readonly bool _explodeOnSet;
        private readonly bool _cancelOnGet;
        private string _name = "ada";

        public Exploding(bool explodeOnGet = false, bool explodeOnSet = false, bool cancelOnGet = false)
        {
            _explodeOnGet = explodeOnGet;
            _explodeOnSet = explodeOnSet;
            _cancelOnGet = cancelOnGet;
        }

        public string Name
        {
            get
            {
                if (_cancelOnGet)
                {
                    throw new OperationCanceledException("stop");
                }

                if (_explodeOnGet)
                {
                    throw new InvalidOperationException("get");
                }

                return _name;
            }

            set
            {
                if (_explodeOnSet)
                {
                    throw new InvalidOperationException("set");
                }

                _name = value;
            }
        }
    }

    private sealed class Customer
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }

    private sealed class When
    {
        public DateTime Stamp { get; set; }

        public DateOnly Day { get; set; }

        public TimeOnly Clock { get; set; }

        public TimeSpan Span { get; set; }
    }

    private sealed record Mixed
    {
        public int Label { get; set; }

        public int Score { get; init; }
    }

    private sealed class ReadOnlyOnly : IReadOnlyCollection<int>
    {
        public ReadOnlyOnly(int count) => Count = count;

        public int Count { get; }

        public IEnumerator<int> GetEnumerator() => throw new InvalidOperationException("enumerated");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class YieldOnce : IEnumerable<int>
    {
        public int Moves { get; private set; }

        public IEnumerator<int> GetEnumerator()
        {
            Moves++;
            yield return 1;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
