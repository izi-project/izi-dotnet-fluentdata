using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Verifies <see cref="Transformer{T}"/> end-to-end: per-property pipelines run and are written back to the same
/// instance, whole-instance steps run in order, mutable structs round-trip correctly, an invalid selector is
/// rejected at construction, cancellation is honoured, and a genuinely asynchronous step drives the slow path.
/// </summary>
public class TransformerTests
{
    public sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Age { get; set; }
        public string Id { get; } = "read-only";
    }

    public sealed class PersonTransformer : Transformer<Person>
    {
        public PersonTransformer()
        {
            RuleFor(x => x.Name).Trim().ToUpper();
            RuleFor(x => x.Description).Trim().Truncate(5).DefaultIfEmpty("N/A");
            RuleFor(x => x.Total).Round(2).Clamp(0m, 1000m);
        }
    }

    public sealed class EmptyTransformer : Transformer<Person>;

    [Fact]
    public async Task Transforms_single_property_and_writes_back()
    {
        var result = await new PersonTransformer().TransformAsync(new Person { Name = "  bob  " });
        Assert.Equal("BOB", result.Name);
    }

    [Fact]
    public async Task Returns_the_same_mutated_instance()
    {
        var person = new Person { Name = " x " };
        var result = await new PersonTransformer().TransformAsync(person);
        Assert.Same(person, result);
    }

    [Fact]
    public async Task Transforms_multiple_properties_independently()
    {
        var person = new Person { Name = "  alice ", Description = "  hello world ", Total = 5000.126m };
        var result = await new PersonTransformer().TransformAsync(person);

        Assert.Equal("ALICE", result.Name);
        Assert.Equal("hello", result.Description);
        Assert.Equal(1000m, result.Total);
    }

    [Fact]
    public async Task Empty_result_falls_back_to_default()
    {
        var result = await new PersonTransformer().TransformAsync(new Person { Description = "    " });
        Assert.Equal("N/A", result.Description);
    }

    [Fact]
    public async Task No_steps_leaves_instance_unchanged()
    {
        var result = await new EmptyTransformer().TransformAsync(new Person { Name = "  untouched  " });
        Assert.Equal("  untouched  ", result.Name);
    }

    [Fact]
    public async Task Steps_run_in_registration_order()
    {
        // Append then Trim would leave the added space; Trim then Append proves ordering.
        var result = await new Transformer<string>().Trim().Append("!").TransformAsync("  hi  ");
        Assert.Equal("hi!", result);
    }

    // ---- Whole-instance steps ----

    public sealed class DeriveAgeTransformer : Transformer<Person>
    {
        public DeriveAgeTransformer()
        {
            // x => x is the identity selector: the nested pipeline sees the whole instance.
            RuleFor(x => x).AddTransformer(new TransformerRule<Person>((p, _) =>
            {
                p.Age = p.Name.Length;
                return ValueTask.FromResult(p);
            }));
        }
    }

    [Fact]
    public async Task Identity_selector_transforms_the_whole_instance()
    {
        var result = await new DeriveAgeTransformer().TransformAsync(new Person { Name = "abcd" });
        Assert.Equal(4, result.Age);
    }

    [Fact]
    public async Task AddTransformer_runs_a_whole_instance_step()
    {
        var transformer = new Transformer<Person>().AddTransformer(new TransformerRule<Person>((p, _) =>
        {
            p.Name = p.Name.Trim();
            return ValueTask.FromResult(p);
        }));

        var result = await transformer.TransformAsync(new Person { Name = "  spaced  " });
        Assert.Equal("spaced", result.Name);
    }

    // ---- Mutable struct write-back ----

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class PointTransformer : Transformer<Point>
    {
        public PointTransformer() => RuleFor(p => p.X).Add(10);
    }

    [Fact]
    public async Task Writes_back_into_a_mutable_struct()
    {
        var result = await new PointTransformer().TransformAsync(new Point { X = 1, Y = 2 });
        Assert.Equal(11, result.X);
        Assert.Equal(2, result.Y);
    }

    // ---- Selector validation ----

    [Fact]
    public void RuleFor_rejects_null_selector()
        => Assert.Throws<ArgumentNullException>(() => new Transformer<Person>().RuleFor<string>(null!));

    [Fact]
    public void RuleFor_rejects_read_only_property()
        => Assert.Throws<ArgumentException>(() => new Transformer<Person>().RuleFor(x => x.Id));

    [Fact]
    public void RuleFor_rejects_non_assignable_selector()
        => Assert.Throws<ArgumentException>(() => new Transformer<Person>().RuleFor(x => x.Name.Trim()));

    // ---- Cancellation ----

    [Fact]
    public async Task Honours_a_cancelled_token()
    {
        var transformer = new PersonTransformer();
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await transformer.TransformAsync(new Person(), new CancellationToken(canceled: true)));
    }

    // ---- Asynchronous slow path ----

    [Fact]
    public async Task Drives_the_async_slow_path_for_a_yielding_step()
    {
        var yielding = new TransformerRule<int>(async (value, _) =>
        {
            await Task.Yield();
            return value + 1;
        });

        var result = await new Transformer<int>().AddTransformer(yielding).AddTransformer(yielding).TransformAsync(1);
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task Drives_the_async_slow_path_through_a_property_step()
    {
        var transformer = new Transformer<Person>();
        transformer.RuleFor(x => x.Age).AddTransformer(new TransformerRule<int>(async (value, _) =>
        {
            await Task.Yield();
            return value * 2;
        }));

        var result = await transformer.TransformAsync(new Person { Age = 21 });
        Assert.Equal(42, result.Age);
    }
}
