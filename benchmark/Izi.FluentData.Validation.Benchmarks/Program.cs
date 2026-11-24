using BenchmarkDotNet.Running;

namespace Izi.FluentData.Validation.Benchmarks;

/// <summary>Entry point. Checks validator results, then runs BenchmarkDotNet.</summary>
public static class Program
{
    /// <summary>Verifies the scenarios, then runs the benchmarks selected by <paramref name="args"/>.</summary>
    /// <param name="args">BenchmarkDotNet arguments, for example <c>--job short</c>.</param>
    /// <returns>Zero when every benchmark succeeded.</returns>
    public static int Main(string[] args)
    {
        ValidationBenchmarks.Verify();
        // BenchmarkDotNet searches every project under the nearest solution.
        // .old has a project with this same name, so the search has to start in this folder.
        Directory.SetCurrentDirectory(ProjectDirectory());
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

    private static string ProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Izi.FluentData.Validation.Benchmarks.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find Izi.FluentData.Validation.Benchmarks.csproj.");
    }
}
