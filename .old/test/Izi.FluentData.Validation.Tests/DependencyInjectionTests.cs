using Izi.FluentData.Validation.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Izi.FluentData.Validation.Tests;

/// <summary>
/// Covers the <c>AddValidator</c>/<c>AddValidators</c> registration helpers: interface and concrete
/// resolution, singleton sharing versus transient lifetimes, instance and factory registration, assembly
/// scanning, the non-validator-type guard, and that a resolved validator actually works.
/// </summary>
public class DependencyInjectionTests
{
    public sealed class Sample
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class SampleValidator : Validator<Sample>
    {
        public SampleValidator() => RuleFor(x => x.Name).NotEmpty();
    }

    [Fact]
    public void AddValidator_registers_interface_and_concrete()
    {
        var provider = new ServiceCollection()
            .AddValidator<SampleValidator>()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<IValidator<Sample>>());
        Assert.NotNull(provider.GetService<SampleValidator>());
    }

    [Fact]
    public void Default_lifetime_is_singleton_shared_across_interface_and_concrete()
    {
        var provider = new ServiceCollection()
            .AddValidator<SampleValidator>()
            .BuildServiceProvider();

        var viaInterface = provider.GetRequiredService<IValidator<Sample>>();
        var viaConcrete = provider.GetRequiredService<SampleValidator>();

        // Forwarding means both resolve to the very same singleton instance.
        Assert.Same(viaInterface, viaConcrete);
        Assert.Same(viaInterface, provider.GetRequiredService<IValidator<Sample>>());
    }

    [Fact]
    public void Transient_lifetime_yields_new_instances()
    {
        var provider = new ServiceCollection()
            .AddValidator<SampleValidator>(ServiceLifetime.Transient)
            .BuildServiceProvider();

        Assert.NotSame(
            provider.GetRequiredService<IValidator<Sample>>(),
            provider.GetRequiredService<IValidator<Sample>>());
    }

    [Fact]
    public void AddValidator_instance_registers_singleton()
    {
        var instance = new SampleValidator();
        var provider = new ServiceCollection()
            .AddValidator<Sample>(instance)
            .BuildServiceProvider();

        Assert.Same(instance, provider.GetRequiredService<IValidator<Sample>>());
    }

    [Fact]
    public void AddValidator_factory_is_used()
    {
        var instance = new SampleValidator();
        var provider = new ServiceCollection()
            .AddValidator<Sample>(_ => instance)
            .BuildServiceProvider();

        Assert.Same(instance, provider.GetRequiredService<IValidator<Sample>>());
    }

    [Fact]
    public void AddValidators_scans_assembly()
    {
        var provider = new ServiceCollection()
            .AddValidators(ServiceLifetime.Singleton, typeof(SampleValidator).Assembly)
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<IValidator<Sample>>());
    }

    [Fact]
    public void AddValidator_rejects_non_validator_type()
        => Assert.Throws<ArgumentException>(() =>
            new ServiceCollection().AddValidator(typeof(Sample)));

    [Fact]
    public async Task Resolved_validator_works()
    {
        var provider = new ServiceCollection()
            .AddValidator<SampleValidator>()
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<Sample>>();

        Assert.Empty(await validator.ValidateAsync(new Sample { Name = "ok" }));
        Assert.Single(await validator.ValidateAsync(new Sample { Name = "" }));
    }
}
