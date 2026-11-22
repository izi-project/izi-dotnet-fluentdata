using BenchmarkDotNet.Running;

namespace Izi.FluentData.Transformer.Benchmarks;

/// <summary>Entry point. Checks pipeline results, then runs BenchmarkDotNet.</summary>
public static class Program
{
    /// <summary>Verifies the scenarios, then runs the benchmarks selected by <paramref name="args"/>.</summary>
    /// <param name="args">BenchmarkDotNet arguments, for example <c>--job short</c>.</param>
    /// <returns>Zero when every benchmark succeeded.</returns>
    public static async Task<int> Main(string[] args)
    {
        await TransformerBenchmarks.VerifyAsync().ConfigureAwait(false);
        ValidationBenchmarks.Verify();
        var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        foreach (var summary in summaries)
        {
            if (summary.HasCriticalValidationErrors)
            {
                return 1;
            }

            foreach (var report in summary.Reports)
            {
                if (!report.Success)
                {
                    return 1;
                }
            }
        }

        return 0;
    }
}
