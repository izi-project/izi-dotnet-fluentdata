using Izi.FluentData.Transformer.Rules;
using static Izi.FluentData.Transformer.Rules.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers <see cref="CompositeTransformerRule{TSource, TDestination}"/> composition: running a single step,
/// chaining with <c>Then</c> (including type changes), order preservation, and the null-guard behaviour of
/// both the rule constructor and the deferred initial-rule check.
/// </summary>
public class CompositeRuleTests
{
    [Fact]
    public async Task Single_rule_runs()
    {
        var rule = new CompositeTransformerRule<string, string>(Trim());
        Assert.Equal("x", await rule.TransformAsync("  x  "));
    }

    [Fact]
    public async Task Then_chains_and_changes_type()
    {
        // string -> trim -> string -> parse -> int -> clamp -> int
        var rule = new CompositeTransformerRule<string, string>(Trim())
            .Then(StringToInt32())
            .Then(Clamp(0, 100));

        Assert.Equal(100, await rule.TransformAsync("  150 "));
    }

    [Fact]
    public async Task Then_preserves_order()
    {
        var rule = new CompositeTransformerRule<string, string>(Append("a"))
            .Then(Append("b"));

        Assert.Equal("xab", await rule.TransformAsync("x"));
    }

    [Fact]
    public void Rule_rejects_null_action()
        => Assert.Throws<ArgumentNullException>(() => new TransformerRule<string, string>(null!));

    [Fact]
    public void Then_rejects_null_next_rule()
    {
        var rule = new CompositeTransformerRule<string, string>(Trim());
        Assert.Throws<ArgumentNullException>(() => rule.Then<string>(null!));
    }

    [Fact]
    public async Task Composite_with_null_initial_rule_throws_when_invoked()
    {
        // The null guard lives inside the pipeline delegate, so it surfaces on use.
        var rule = new CompositeTransformerRule<string, string>(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await rule.TransformAsync("x"));
    }
}
