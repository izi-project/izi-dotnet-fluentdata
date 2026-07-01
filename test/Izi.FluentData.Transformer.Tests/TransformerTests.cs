using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Verifies <see cref="Transformer{T}"/> end-to-end: property pipelines run, results are written back to the
/// same instance, multiple properties are handled independently, defaults apply, and a rule-less transformer
/// is a no-op.
/// </summary>
public class TransformerTests
{
    public sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Age { get; set; }
    }

    public class PersonTransformer : Transformer<Person>
    {
        public PersonTransformer()
        {
            RuleFor(x => x.Name, b => b.Trim().ToUpper());
            RuleFor(x => x.Description, b => b.Trim().Truncate(5).DefaultIfEmpty("N/A"));
            RuleFor(x => x.Total, b => b.Round(2).Clamp(0m, 1000m));
        }
    }

    public class PersonTransformerNoRules : Transformer<Person>
    {
        public PersonTransformerNoRules()
        {
        }
    }

    [Fact]
    public async Task Transforms_single_property_and_writes_back()
    {
        var transformer = new PersonTransformer();

        var person = new Person { Name = "  bob  " };
        var result = await transformer.TransformAsync(person);

        Assert.Equal("BOB", result.Name);
    }

    [Fact]
    public async Task Returns_the_same_mutated_instance()
    {
        var transformer = new PersonTransformer();

        var person = new Person { Name = " x " };
        var result = await transformer.TransformAsync(person);

        Assert.Same(person, result);
    }

    [Fact]
    public async Task Transforms_multiple_properties_independently()
    {
        var transformer = new PersonTransformer();

        var person = new Person { Name = "  alice ", Description = "  hello world ", Total = 5000.126m };
        var result = await transformer.TransformAsync(person);

        Assert.Equal("ALICE", result.Name);
        Assert.Equal("hello", result.Description);
        Assert.Equal(1000m, result.Total);
    }

    [Fact]
    public async Task Empty_description_falls_back_to_default()
    {
        var transformer = new PersonTransformer();

        var person = new Person { Description = "    " };
        var result = await transformer.TransformAsync(person);

        Assert.Equal("N/A", result.Description);
    }

    [Fact]
    public async Task No_rules_leaves_instance_unchanged()
    {
        var transformer = new PersonTransformerNoRules();

        var person = new Person { Name = "  untouched  " };
        var result = await transformer.TransformAsync(person);

        Assert.Equal("  untouched  ", result.Name);
    }
}
