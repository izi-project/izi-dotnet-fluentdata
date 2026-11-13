namespace Izi.FluentData.Transformer.Tests;

public class FluentAndInterfaceTests
{
    [Fact]
    public async Task ITransformer_runs_the_concrete_pipeline_and_can_be_replaced()
    {
        var concrete = new Transformer<Customer>();
        concrete.RuleFor(customer => customer.Name).Trim().ToUpper();
        ITransformer<Customer> transformer = concrete;

        var updated = await transformer.TransformAsync(new Customer { Name = "  ada  " });

        Assert.Equal("ADA", updated.Name);

        ITransformer<Customer> mock = new PrefixName();
        var prefixed = await mock.TransformAsync(new Customer { Name = "ada" });
        Assert.Equal("xada", prefixed.Name);
    }

    [Fact]
    public async Task String_numeric_default_and_datetime_fluent_rules_run()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Trim().ToUpper();
        transformer.RuleFor(customer => customer.Email).DefaultIfNullOrWhitespace("none").Trim().ToLower();
        transformer.RuleFor(customer => customer.Price).Round(2);
        transformer.RuleFor(customer => customer.When).StartOfMonth();

        var updated = await transformer.TransformAsync(new Customer
        {
            Name = "  ada  ",
            Email = "   ",
            Price = 1.239,
            When = new DateTime(2024, 3, 25),
        });

        Assert.Equal("ADA", updated.Name);
        Assert.Equal("none", updated.Email);
        Assert.Equal(double.Round(1.239, 2), updated.Price);
        Assert.NotEqual(1.239, updated.Price);
        Assert.Equal(new DateTime(2024, 3, 1), updated.When);
    }

    [Fact]
    public async Task ParseInt_and_a_later_numeric_rule_write_the_target()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.AgeText)
            .Trim()
            .ParseInt()
            .Add(1)
            .Into(customer => customer.Age);

        var updated = await transformer.TransformAsync(new Customer { AgeText = " 41 " });

        Assert.Equal(42, updated.Age);
        Assert.Equal(" 41 ", updated.AgeText);
    }

    [Fact]
    public async Task ConvertChecked_and_a_custom_Add_remain_available()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Quantity)
            .Add(static (value, _) => new ValueTask<int>(value + 1))
            .ConvertChecked<Customer, int, short>()
            .Into(customer => customer.QuantitySmall);

        var updated = await transformer.TransformAsync(new Customer { Quantity = 7 });

        Assert.Equal(7, updated.Quantity);
        Assert.Equal((short)8, updated.QuantitySmall);
    }

    [Fact]
    public async Task A_rule_added_after_the_first_run_is_used_on_the_next_run()
    {
        var transformer = new Transformer<Customer>();
        var rule = transformer.RuleFor(customer => customer.Name);
        var first = await transformer.TransformAsync(new Customer { Name = "  ada  " });
        Assert.Equal("  ada  ", first.Name);

        rule.Trim();
        var second = await transformer.TransformAsync(new Customer { Name = "  ada  " });
        Assert.Equal("ada", second.Name);
    }

    private sealed class PrefixName : ITransformer<Customer>
    {
        public ValueTask<Customer> TransformAsync(Customer value, CancellationToken cancellationToken = default)
        {
            value.Name = "x" + value.Name;
            return new ValueTask<Customer>(value);
        }
    }

    public sealed class Customer
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AgeText { get; set; } = string.Empty;

        public int Age { get; set; }

        public double Price { get; set; }

        public DateTime When { get; set; }

        public int Quantity { get; set; }

        public short QuantitySmall { get; set; }
    }
}
