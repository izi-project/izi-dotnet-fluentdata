using BenchmarkDotNet.Attributes;
using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation.Benchmarks;

/// <summary>
/// Measures what each of <see cref="ValidatorRule{T}"/>'s two message constructors costs at <em>evaluation</em>
/// time. The passing rows quantify the delegate indirection on the hot path (the message is never built); the
/// failing rows quantify actually producing the message.
/// </summary>
[MemoryDiagnoser]
public class RuleMessageBenchmarks
{
    private const int Threshold = 10;
    private const int Valid = 1;
    private const int Invalid = 42;

    // Shared across all three rules so the rows differ only in how the message is produced.
    private static readonly Func<int, CancellationToken, ValueTask<bool>> LessThanThreshold =
        (value, _) => ValueTask.FromResult(value < Threshold);

    private readonly IValidatorRule<int> _constant = new ValidatorRule<int>(LessThanThreshold, "Value must be less than 10.");
    private readonly IValidatorRule<int> _factory = new ValidatorRule<int>(LessThanThreshold, value => $"Value {value} must be less than {Threshold}.");

    /// <summary>Baseline: a passing rule with a constant message — the message delegate is never invoked.</summary>
    [Benchmark(Baseline = true)]
    public ValueTask<string?> PassConstant() => _constant.ValidateAsync(Valid);

    /// <summary>A passing rule built from an instance-aware factory; identical work to the baseline.</summary>
    [Benchmark]
    public ValueTask<string?> PassFactory() => _factory.ValidateAsync(Valid);

    /// <summary>A failing rule with a constant message — one delegate call returning a cached literal.</summary>
    [Benchmark]
    public ValueTask<string?> FailConstant() => _constant.ValidateAsync(Invalid);

    /// <summary>A failing rule that interpolates the failing value into a fresh message.</summary>
    [Benchmark]
    public ValueTask<string?> FailFactory() => _factory.ValidateAsync(Invalid);
}
