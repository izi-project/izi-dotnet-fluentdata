using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer.Tests;

public class TransformerObjectTests
{
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
        transformer.RuleFor(customer => customer.Name).Add((name, _) =>
        {
            order.Add("name");
            return new ValueTask<string>(name.Trim());
        });
        transformer.RuleFor(customer => customer.Age).Add((age, _) =>
        {
            order.Add("age");
            return new ValueTask<int>(age + 1);
        });

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
        transformer.RuleFor(customer => customer.Name).Add((name, _) =>
        {
            ran++;
            return new ValueTask<string>(name);
        });
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await transformer.TransformAsync(new Customer { Name = "ada" }, cancelled.Token).ConfigureAwait(true));
        Assert.Equal(0, ran);

        using var during = new CancellationTokenSource();
        var later = 0;
        var pipeline = new Transformer<Customer>();
        pipeline.RuleFor(customer => customer.Name).Add((name, _) =>
        {
            during.Cancel();
            ran++;
            return new ValueTask<string>(name);
        });
        pipeline.RuleFor(customer => customer.Age).Add((age, _) =>
        {
            later++;
            return new ValueTask<int>(age);
        });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await pipeline.TransformAsync(new Customer { Name = "ada", Age = 1 }, during.Token).ConfigureAwait(true));
        Assert.Equal(1, ran);
        Assert.Equal(0, later);
    }

    [Fact]
    public async Task All_sync_class_pipeline_returns_a_completed_ValueTask()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Trim())
            .Add(TransformerRules.ToUpper());
        transformer.RuleFor(customer => customer.Age).Add(TransformerRules.Add(1));

        var pending = transformer.TransformAsync(new Customer
        {
            Name = "  ada  ",
            Age = 6,
        });

        Assert.True(pending.IsCompletedSuccessfully);
        var customer = await pending.ConfigureAwait(true);
        Assert.Equal("ADA", customer.Name);
        Assert.Equal(7, customer.Age);
    }

    [Fact]
    public async Task All_sync_struct_pipeline_returns_a_completed_ValueTask()
    {
        var transformer = new Transformer<Point>();
        transformer.RuleFor(point => point.X).Add(TransformerRules.Add(1)).Add(TransformerRules.Add(1));
        transformer.RuleFor(point => point.Y).Add(TransformerRules.Multiply(3));

        var pending = transformer.TransformAsync(new Point { X = 1, Y = 5 });

        Assert.True(pending.IsCompletedSuccessfully);
        var point = await pending.ConfigureAwait(true);
        Assert.Equal(3, point.X);
        Assert.Equal(15, point.Y);
    }

    [Fact]
    public async Task Yield_in_the_middle_of_an_Add_chain_runs_later_steps()
    {
        var order = new List<string>();
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name)
            .Add((name, _) =>
            {
                order.Add("trim");
                return new ValueTask<string>(name.Trim());
            })
            .Add(async (name, _) =>
            {
                order.Add("yield");
                await Task.Yield();
                return name;
            })
            .Add((name, _) =>
            {
                order.Add("upper");
                return new ValueTask<string>(name.ToUpperInvariant());
            });
        transformer.RuleFor(customer => customer.Age).Add((age, _) =>
        {
            order.Add("age");
            return new ValueTask<int>(age + 1);
        });

        var pending = transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 4 });
        var customer = await pending.ConfigureAwait(true);
        Assert.Equal("ADA", customer.Name);
        Assert.Equal(5, customer.Age);
        Assert.Equal(new[] { "trim", "yield", "upper", "age" }, order);
    }

    [Fact]
    public async Task Sync_faulted_ValueTask_propagates()
    {
        var later = 0;
        var addChain = new Transformer<Customer>();
        addChain.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Trim())
            .Add(static async (value, _) =>
            {
                await new ValueTask().ConfigureAwait(true);
                throw new InvalidOperationException("sync-add:" + value);
            })
            .Add((_, _) =>
            {
                later++;
                return new ValueTask<string>("skipped");
            });

        var addPending = addChain.TransformAsync(new Customer { Name = "  ada  " });
        var addError = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await addPending.ConfigureAwait(true));
        Assert.Equal("Name", addError.PropertyName);
        var syncRule = Assert.IsType<TransformationRuleException>(addError.InnerException);
        Assert.Equal("sync-add:ada", syncRule.InnerException!.Message);
        Assert.Equal(0, later);
    }

    [Fact]
    public async Task Async_fault_propagates()
    {
        var later = 0;
        var addChain = new Transformer<Customer>();
        addChain.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Trim())
            .Add(async (value, _) =>
            {
                await Task.Yield();
                throw new InvalidOperationException("async-add:" + value);
            })
            .Add((_, _) =>
            {
                later++;
                return new ValueTask<string>("skipped");
            });

        var addPending = addChain.TransformAsync(new Customer { Name = "  ada  " });
        Assert.False(addPending.IsCompletedSuccessfully);
        var addError = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await addPending.ConfigureAwait(true));
        Assert.Equal("Name", addError.PropertyName);
        var asyncRule = Assert.IsType<TransformationRuleException>(addError.InnerException);
        Assert.Equal("async-add:ada", asyncRule.InnerException!.Message);
        Assert.Equal(0, later);
    }

    [Fact]
    public async Task Cancellation_is_honoured_mid_pipeline()
    {
        var addLater = 0;
        using var duringAdd = new CancellationTokenSource();
        var addChain = new Transformer<Customer>();
        addChain.RuleFor(customer => customer.Name)
            .Add((name, _) =>
            {
                duringAdd.Cancel();
                return new ValueTask<string>(name.Trim());
            })
            .Add((_, _) =>
            {
                addLater++;
                return new ValueTask<string>("skipped");
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await addChain.TransformAsync(new Customer { Name = "  ada  " }, duringAdd.Token).ConfigureAwait(true));
        Assert.Equal(0, addLater);

        var yieldLater = 0;
        using var duringYield = new CancellationTokenSource();
        var yieldChain = new Transformer<Customer>();
        yieldChain.RuleFor(customer => customer.Name)
            .Add(async (name, _) =>
            {
                await Task.Yield();
                duringYield.Cancel();
                return name;
            })
            .Add((_, _) =>
            {
                yieldLater++;
                return new ValueTask<string>("skipped");
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await yieldChain.TransformAsync(new Customer { Name = "ada" }, duringYield.Token).ConfigureAwait(true));
        Assert.Equal(0, yieldLater);
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
