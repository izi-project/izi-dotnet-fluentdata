using Izi.FluentData.Transformer.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the <c>AddTransformer</c>/<c>AddTransformers</c> registration helpers: interface and concrete
/// resolution, singleton sharing versus transient lifetimes, instance and factory registration, assembly
/// scanning, the non-transformer-type guard, and that a resolved transformer actually works.
/// </summary>
public class DependencyInjectionTests
{
    public sealed class Sample
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class SampleTransformer : Transformer<Sample>
    {
        public SampleTransformer() => RuleFor(x => x.Name, b => b.Trim().ToUpper());
    }

    [Fact]
    public void AddTransformer_registers_interface_and_concrete()
    {
        var provider = new ServiceCollection()
            .AddTransformer<SampleTransformer>()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<ITransformer<Sample>>());
        Assert.NotNull(provider.GetService<SampleTransformer>());
    }

    [Fact]
    public void Default_lifetime_is_singleton_shared_across_interface_and_concrete()
    {
        var provider = new ServiceCollection()
            .AddTransformer<SampleTransformer>()
            .BuildServiceProvider();

        var viaInterface = provider.GetRequiredService<ITransformer<Sample>>();
        var viaConcrete = provider.GetRequiredService<SampleTransformer>();

        // Forwarding means both resolve to the very same singleton instance.
        Assert.Same(viaInterface, viaConcrete);
        Assert.Same(viaInterface, provider.GetRequiredService<ITransformer<Sample>>());
    }

    [Fact]
    public void Transient_lifetime_yields_new_instances()
    {
        var provider = new ServiceCollection()
            .AddTransformer<SampleTransformer>(ServiceLifetime.Transient)
            .BuildServiceProvider();

        Assert.NotSame(
            provider.GetRequiredService<ITransformer<Sample>>(),
            provider.GetRequiredService<ITransformer<Sample>>());
    }

    [Fact]
    public void AddTransformer_instance_registers_singleton()
    {
        var instance = new SampleTransformer();
        var provider = new ServiceCollection()
            .AddTransformer<Sample>(instance)
            .BuildServiceProvider();

        Assert.Same(instance, provider.GetRequiredService<ITransformer<Sample>>());
    }

    [Fact]
    public void AddTransformer_factory_is_used()
    {
        var instance = new SampleTransformer();
        var provider = new ServiceCollection()
            .AddTransformer<Sample>(_ => instance)
            .BuildServiceProvider();

        Assert.Same(instance, provider.GetRequiredService<ITransformer<Sample>>());
    }

    [Fact]
    public void AddTransformers_scans_assembly()
    {
        var provider = new ServiceCollection()
            .AddTransformers(ServiceLifetime.Singleton, typeof(SampleTransformer).Assembly)
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<ITransformer<Sample>>());
    }

    [Fact]
    public void AddTransformer_rejects_non_transformer_type()
        => Assert.Throws<ArgumentException>(() =>
            new ServiceCollection().AddTransformer(typeof(Sample)));

    [Fact]
    public async Task Resolved_transformer_works()
    {
        var provider = new ServiceCollection()
            .AddTransformer<SampleTransformer>()
            .BuildServiceProvider();

        var transformer = provider.GetRequiredService<ITransformer<Sample>>();
        var result = await transformer.TransformAsync(new Sample { Name = "  bob  " });

        Assert.Equal("BOB", result.Name);
    }
}
