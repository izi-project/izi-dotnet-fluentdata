using BenchmarkDotNet.Attributes;
using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation.Benchmarks;

/// <summary>
/// Measures what each of <see cref="ValidatorRule{T}"/>'s two message constructors costs at <em>construction</em>
/// time — the cost a validator pays once in its constructor. This is where the message strategies actually differ:
/// an eagerly-interpolated constant allocates its string up front, while a non-capturing factory reuses a delegate
/// cached in a static field and builds nothing until a value fails.
/// </summary>
[MemoryDiagnoser]
public class RuleConstructionBenchmarks
{
    private const int Threshold = 10;

    private static readonly Func<int, CancellationToken, ValueTask<bool>> LessThanThreshold =
        (value, _) => ValueTask.FromResult(value < Threshold);

    /// <summary>Baseline: a constant message, eagerly interpolated by the caller the way <c>ValidatorRules</c> does today.</summary>
    [Benchmark(Baseline = true)]
    public IValidatorRule<int> BuildConstant()
        => new ValidatorRule<int>(LessThanThreshold, $"Value must be less than {Threshold}.");

    /// <summary>A non-capturing factory: the lambda is cached in a static field, so nothing is allocated for the message.</summary>
    [Benchmark]
    public IValidatorRule<int> BuildFactory()
        => new ValidatorRule<int>(LessThanThreshold, value => $"Value {value} must be less than {Threshold}.");
}
