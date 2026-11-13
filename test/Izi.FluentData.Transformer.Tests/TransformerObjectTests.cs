using System.Globalization;

namespace Izi.FluentData.Transformer.Tests;

public class TransformerObjectTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    [Fact]
    public async Task RuleFor_trims_a_class_property()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Add(TransformerRules.Trim());

        var customer = new Customer { Name = "  ada  ", AgeText = "3", Age = 3 };
        customer = await transformer.TransformAsync(customer);

        Assert.Equal("ada", customer.Name);
        Assert.Equal(3, customer.Age);
    }

    [Fact]
    public async Task Convert_parse_writes_the_target_property()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Into(customer => customer.Age);

        var customer = new Customer { Name = "Ada", AgeText = " 7 ", Age = 0 };
        customer = await transformer.TransformAsync(customer);

        Assert.Equal(7, customer.Age);
        Assert.Equal(" 7 ", customer.AgeText);
    }

    [Fact]
    public async Task RuleFor_returns_the_updated_struct()
    {
        var transformer = new Transformer<Point>();
        transformer.RuleFor(point => point.X).Add(TransformerRules.Add(1));

        var point = await transformer.TransformAsync(new Point { X = 1, Y = 5 });

        Assert.Equal(2, point.X);
        Assert.Equal(5, point.Y);
    }

    [Fact]
    public async Task RuleFor_clones_a_record_instead_of_mutating_it()
    {
        var transformer = new Transformer<Person>();
        transformer.RuleFor(person => person.Name).Add(TransformerRules.Trim());

        var original = new Person("  ada  ", 4);
        var updated = await transformer.TransformAsync(original);

        Assert.Equal("  ada  ", original.Name);
        Assert.Equal("ada", updated.Name);
        Assert.Equal(4, updated.Age);
        Assert.NotSame(original, updated);
    }

    [Fact]
    public async Task Property_rules_run_in_registration_order()
    {
        var order = new List<string>();
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Add(
            TransformerRules.Map<string, string>(name =>
            {
                order.Add("name");
                return name.Trim();
            }));
        transformer.RuleFor(customer => customer.Age).Add(
            TransformerRules.Map<int, int>(age =>
            {
                order.Add("age");
                return age + 1;
            }));

        var customer = await transformer.TransformAsync(
            new Customer { Name = "  ada  ", Age = 1 });

        Assert.Equal("ada", customer.Name);
        Assert.Equal(2, customer.Age);
        Assert.Equal(new[] { "name", "age" }, order);
    }

    [Fact]
    public async Task String_transformer_returns_the_input_and_rejects_RuleFor()
    {
        var transformer = new Transformer<string>();

        Assert.Equal("hi", await transformer.TransformAsync("hi"));
        Assert.Throws<ArgumentException>(() => transformer.RuleFor(value => value.Length));
    }

    [Fact]
    public void Invalid_property_expressions_are_rejected()
    {
        var transformer = new Transformer<Customer>();

        Assert.Throws<ArgumentNullException>(() => transformer.RuleFor<string>(null!));
        var trimmed = Assert.Throws<ArgumentException>(() => transformer.RuleFor(customer => customer.Name.Trim()));
        Assert.Contains("simple property", trimmed.Message, StringComparison.Ordinal);
        var nested = Assert.Throws<ArgumentException>(() => transformer.RuleFor(customer => customer.Address.City));
        Assert.Contains("simple property", nested.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cancellation_is_checked_between_property_rules()
    {
        var ran = 0;
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Add(
            TransformerRules.Map<string, string>(name =>
            {
                ran++;
                return name;
            }));
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await transformer.TransformAsync(new Customer { Name = "ada" }, cancelled.Token).ConfigureAwait(true));
        Assert.Equal(0, ran);

        using var during = new CancellationTokenSource();
        var later = 0;
        var pipeline = new Transformer<Customer>();
        pipeline.RuleFor(customer => customer.Name).Add(
            TransformerRules.Map<string, string>(name =>
            {
                during.Cancel();
                ran++;
                return name;
            }));
        pipeline.RuleFor(customer => customer.Age).Add(
            TransformerRules.Map<int, int>(age =>
            {
                later++;
                return age;
            }));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await pipeline.TransformAsync(new Customer { Name = "ada", Age = 1 }, during.Token).ConfigureAwait(true));
        Assert.Equal(1, ran);
        Assert.Equal(0, later);
    }

    [Fact]
    public async Task Convert_without_Into_throws_and_does_not_change_the_instance()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.AgeText)
            .Convert(TransformerRules.Parse<int>(Invariant));
        var customer = new Customer { Name = "Ada", AgeText = "7", Age = 1 };

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await transformer.TransformAsync(customer).ConfigureAwait(true));

        Assert.Contains("Into", error.Message, StringComparison.Ordinal);
        Assert.Equal(1, customer.Age);
        Assert.Equal("Ada", customer.Name);
    }

    public sealed class Customer
    {
        public string Name { get; set; } = string.Empty;

        public string AgeText { get; set; } = string.Empty;

        public int Age { get; set; }

        public Address Address { get; set; } = new Address();
    }

    public sealed class Address
    {
        public string City { get; set; } = string.Empty;
    }

    public struct Point
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public sealed record Person(string Name, int Age);
}
