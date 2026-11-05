using BenchmarkDotNet.Attributes;
using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer.Benchmarks;

/// <summary>
/// Measures throughput and allocations of the transformer pipeline at three granularities: a single rule, a
/// freestanding multi-step pipeline, and a full object transformer over several property pipelines.
/// <see cref="MemoryDiagnoserAttribute"/> tracks allocations so the synchronous fast path can be verified to stay
/// allocation-light (and the object transform allocation-free) in steady state.
/// </summary>
[MemoryDiagnoser]
public class TransformerBenchmarks
{
    /// <summary>Sample target type whose properties exercise the string and numeric pipelines.</summary>
    // Public because it appears in a public [Benchmark] method's return type.
    public sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    private sealed class PersonTransformer : Transformer<Person>
    {
        public PersonTransformer()
        {
            RuleFor(x => x.Name).Trim().ToUpper();
            RuleFor(x => x.Description).Trim().Truncate(50).DefaultIfEmpty("N/A");
            RuleFor(x => x.Total).Round(2).Clamp(0m, 1000m);
        }
    }

    private readonly PersonTransformer _transformer = new();

    // A freestanding multi-step string pipeline, built once from the fluent extensions.
    private readonly Transformer<string> _stringPipeline =
        new Transformer<string>().Trim().ToUpper().Truncate(50);

    // A freestanding date/time pipeline over the new calendar rules.
    private readonly Transformer<DateTime> _dateTimePipeline =
        new Transformer<DateTime>().StartOfMonth().AddDays(9).WithHour(9);

    private readonly TransformerRule<string> _singleRule = TransformerRules.Trim();

    private Person _person = null!;

    /// <summary>Re-seeds the shared <see cref="Person"/> before each benchmark run.</summary>
    [GlobalSetup]
    public void Setup()
        // Rules are idempotent, so steady-state still exercises the full pipeline.
        => _person = new Person { Name = "  john doe  ", Description = "  some text  ", Total = 1234.5678m };

    /// <summary>Baseline: a single trim rule, the cheapest possible step.</summary>
    [Benchmark(Baseline = true)]
    public ValueTask<string> SingleRule() => _singleRule.TransformAsync("  hello world  ");

    /// <summary>A multi-step string pipeline composed from the fluent extensions.</summary>
    [Benchmark]
    public ValueTask<string> StringPipeline() => _stringPipeline.TransformAsync("  hello world  ");

    /// <summary>A multi-step date/time pipeline over the calendar rules.</summary>
    [Benchmark]
    public ValueTask<DateTime> DateTimePipeline() => _dateTimePipeline.TransformAsync(new DateTime(2024, 3, 25, 14, 0, 0));

    /// <summary>A full object transformer running every property pipeline over an instance.</summary>
    [Benchmark]
    public ValueTask<Person> ObjectTransformer() => _transformer.TransformAsync(_person);
}
