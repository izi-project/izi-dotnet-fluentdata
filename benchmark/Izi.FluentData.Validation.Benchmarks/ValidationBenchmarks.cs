using BenchmarkDotNet.Attributes;
using Izi.FluentData.Validation;

namespace Izi.FluentData.Validation.Benchmarks;

/// <summary>
/// Measures a compiled <see cref="Validator{T}"/>. The valid run is the allocation-free path.
/// </summary>
[MemoryDiagnoser]
public class ValidationBenchmarks
{
    private readonly Validator<Person> _validator = Create();
    private readonly Person _valid = new() { Name = "Ada", Age = 30, Email = "ada@example.com" };
    private readonly Person _invalid = new() { Name = "", Age = 0, Email = "nope" };

    /// <summary>Creates the benchmark and compiles the runner once.</summary>
    public ValidationBenchmarks()
    {
        var compiled = _validator.ValidateAsync(_valid).GetAwaiter().GetResult();
        if (!compiled.IsValid)
        {
            throw new InvalidOperationException(compiled.ToString());
        }
    }

    /// <summary>Checks the valid and invalid scenarios before BenchmarkDotNet starts.</summary>
    public static void Verify()
    {
        var benchmarks = new ValidationBenchmarks();
        var valid = benchmarks.Valid();
        if (!valid.IsValid)
        {
            throw new InvalidOperationException(valid.ToString());
        }

        var invalid = benchmarks.Invalid();
        if (invalid.Errors.Count != 2)
        {
            throw new InvalidOperationException(invalid.ToString());
        }
    }

    /// <summary>Validates an object that passes every rule.</summary>
    /// <returns>The shared success result.</returns>
    [Benchmark]
    public ValidationResult Valid() => _validator.ValidateAsync(_valid).GetAwaiter().GetResult();

    /// <summary>Validates an object that fails the name and age rules.</summary>
    /// <returns>The two failed properties.</returns>
    [Benchmark]
    public ValidationResult Invalid() => _validator.ValidateAsync(_invalid).GetAwaiter().GetResult();

    private static Validator<Person> Create()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().MaxLength(32);
        validator.RuleFor(person => person.Age).GreaterThan(0);
        return validator;
    }

    /// <summary>The object measured by the benchmarks.</summary>
    public sealed class Person
    {
        /// <summary>The name.</summary>
        public string? Name { get; set; }

        /// <summary>The age.</summary>
        public int Age { get; set; }

        /// <summary>The email address.</summary>
        public string? Email { get; set; }
    }
}
