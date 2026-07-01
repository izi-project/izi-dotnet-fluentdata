using BenchmarkDotNet.Attributes;
using Izi.FluentData.Transformer;
using Izi.FluentData.Transformer.Rules;
using static Izi.FluentData.Transformer.Rules.Rules;

namespace Izi.FluentData.Transformer.Benchmarks;

/// <summary>
/// Measures throughput and allocations of the transformer pipeline across three granularities: a single rule,
/// a freestanding composed pipeline, and a full object transformer. <see cref="MemoryDiagnoserAttribute"/>
/// tracks allocations so the synchronous fast paths can be verified to stay allocation-light.
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
            RuleFor(x => x.Name, b => b.Trim().ToUpper());
            RuleFor(x => x.Description, b => b.Trim().Truncate(50).DefaultIfEmpty("N/A"));
            RuleFor(x => x.Total, b => b.Round(2).Clamp(0m, 1000m));
        }
    }

    private readonly PersonTransformer _transformer = new();

    // Freestanding pipeline built from the public composite API (string -> string).
    private readonly TransformerRule<string, string> _pipeline =
        new CompositeTransformerRule<string, string>(Trim())
            .Then(ToUpper())
            .Then(Truncate(50));

    private readonly TransformerRule<string> _singleRule = Trim();

    private Person _person = null!;

    /// <summary>Re-seeds the shared <see cref="Person"/> before each benchmark run.</summary>
    [GlobalSetup]
    public void Setup()
        // Rules are idempotent, so steady-state still exercises the full pipeline.
        => _person = new Person { Name = "  john doe  ", Description = "  some text  ", Total = 1234.5678m };

    /// <summary>Baseline: a single trim rule, the cheapest possible step.</summary>
    [Benchmark(Baseline = true)]
    public ValueTask<string> SingleRule() => _singleRule.TransformAsync("  hello world  ");

    /// <summary>A multi-step string pipeline composed directly from the public composite API.</summary>
    [Benchmark]
    public ValueTask<string> FreestandingPipeline() => _pipeline.TransformAsync("  hello world  ");

    /// <summary>A full object transformer running every property pipeline over an instance.</summary>
    [Benchmark]
    public ValueTask<Person> ObjectTransformer() => _transformer.TransformAsync(_person);
}
