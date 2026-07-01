using BenchmarkDotNet.Attributes;
using Izi.FluentData.Validation.Rules;

namespace Izi.FluentData.Validation.Benchmarks;

/// <summary>
/// Contrasts the validator's success and failure paths. <see cref="MemoryDiagnoserAttribute"/> tracks
/// allocations to confirm the valid path is allocation-free (it returns a shared empty result) while the
/// invalid path pays only for the error list it must produce.
/// </summary>
[MemoryDiagnoser]
public class ValidatorBenchmarks
{
    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    private sealed class PersonValidator : Validator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaxLength(50);
            RuleFor(x => x.Age).GreaterThan(0).LessThan(150);
            RuleFor(x => x.Email).EmailAddress();
        }
    }

    private readonly PersonValidator _validator = new();
    private readonly Person _valid = new() { Name = "Anna", Age = 30, Email = "anna@example.com" };
    private readonly Person _invalid = new() { Name = "", Age = -1, Email = "not-an-email" };

    /// <summary>Baseline: the all-valid instance, exercising the allocation-free success path (shared NoErrors result).</summary>
    [Benchmark(Baseline = true)]
    public ValueTask<IReadOnlyList<string>> ValidateValid() => _validator.ValidateAsync(_valid);

    /// <summary>The all-invalid instance, exercising the failure path that allocates the error list.</summary>
    [Benchmark]
    public ValueTask<IReadOnlyList<string>> ValidateInvalid() => _validator.ValidateAsync(_invalid);
}
