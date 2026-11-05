using BenchmarkDotNet.Running;

namespace Izi.FluentData.Transformer.Benchmarks;

/// <summary>Entry point for the transformer benchmark suite; dispatches to BenchmarkDotNet.</summary>
public static class Program
{
    /// <summary>Runs the benchmarks selected by <paramref name="args"/> (or prompts when none are given).</summary>
    /// <param name="args">BenchmarkDotNet command-line arguments (filters, exporters, etc.).</param>
    public static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
