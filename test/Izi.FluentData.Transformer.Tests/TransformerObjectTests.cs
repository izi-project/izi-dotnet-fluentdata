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

    [Fact]
    public async Task All_sync_class_pipeline_returns_a_completed_ValueTask()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Trim())
            .Add(TransformerRules.ToUpper());
        transformer.RuleFor(customer => customer.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Then(TransformerRules.Add(1))
            .Into(customer => customer.Age);

        var pending = transformer.TransformAsync(new Customer
        {
            Name = "  ada  ",
            AgeText = " 6 ",
            Age = 0,
        });

        Assert.True(pending.IsCompletedSuccessfully);
        var customer = await pending.ConfigureAwait(true);
        Assert.Equal("ADA", customer.Name);
        Assert.Equal(7, customer.Age);
        Assert.Equal(" 6 ", customer.AgeText);
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
            .Add(TransformerRules.Map<string, string>(name =>
            {
                order.Add("trim");
                return name.Trim();
            }))
            .Add(async (name, _) =>
            {
                order.Add("yield");
                await Task.Yield();
                return name;
            })
            .Add(TransformerRules.Map<string, string>(name =>
            {
                order.Add("upper");
                return name.ToUpperInvariant();
            }));
        transformer.RuleFor(customer => customer.Age).Add(TransformerRules.Map<int, int>(age =>
        {
            order.Add("age");
            return age + 1;
        }));

        var pending = transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 4 });
        var customer = await pending.ConfigureAwait(true);
        Assert.Equal("ADA", customer.Name);
        Assert.Equal(5, customer.Age);
        Assert.Equal(new[] { "trim", "yield", "upper", "age" }, order);
    }

    [Fact]
    public async Task Yield_in_the_middle_of_a_Then_chain_runs_later_steps()
    {
        var order = new List<string>();
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.AgeText)
            .Add(TransformerRules.Map<string, string>(text =>
            {
                order.Add("pre");
                return text.Trim();
            }))
            .Convert(TransformerRules.Map<string, string>(text =>
            {
                order.Add("convert");
                return text;
            }))
            .Then(TransformerRules.Map<string, string>(text =>
            {
                order.Add("then-before");
                return text + "0";
            }))
            .Then(async (text, _) =>
            {
                order.Add("yield");
                await Task.Yield();
                return text;
            })
            .Then(TransformerRules.Map<string, int>(text =>
            {
                order.Add("then-after");
                return int.Parse(text, Invariant);
            }))
            .Into(customer => customer.Age);

        var pending = transformer.TransformAsync(new Customer { AgeText = " 4 " });
        var customer = await pending.ConfigureAwait(true);
        Assert.Equal(40, customer.Age);
        Assert.Equal(" 4 ", customer.AgeText);
        Assert.Equal(new[] { "pre", "convert", "then-before", "yield", "then-after" }, order);
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
        var addError = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await addPending.ConfigureAwait(true));
        Assert.Equal("sync-add:ada", addError.Message);
        Assert.Equal(0, later);

        var thenLater = 0;
        var conversion = new Transformer<Customer>();
        conversion.RuleFor(customer => customer.AgeText)
            .Convert(new TransformerRule<string, string>(static async (value, _) =>
            {
                await new ValueTask().ConfigureAwait(true);
                throw new InvalidOperationException("sync-then:" + value);
            }))
            .Then<int>((_, _) =>
            {
                thenLater++;
                return new ValueTask<int>(1);
            })
            .Into(customer => customer.Age);

        var unchanged = new Customer { AgeText = "7", Age = 3 };
        var thenPending = conversion.TransformAsync(unchanged);
        var thenError = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await thenPending.ConfigureAwait(true));
        Assert.Equal("sync-then:7", thenError.Message);
        Assert.Equal(0, thenLater);
        Assert.Equal(3, unchanged.Age);
        Assert.Equal("7", unchanged.AgeText);
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
        var addError = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await addPending.ConfigureAwait(true));
        Assert.Equal("async-add:ada", addError.Message);
        Assert.Equal(0, later);

        var thenLater = 0;
        var conversion = new Transformer<Customer>();
        conversion.RuleFor(customer => customer.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Map<string, string>(text => text))
            .Then<string>(async (value, _) =>
            {
                await Task.Yield();
                throw new InvalidOperationException("async-then:" + value);
            })
            .Then<int>((_, _) =>
            {
                thenLater++;
                return new ValueTask<int>(1);
            })
            .Into(customer => customer.Age);

        var thenPending = conversion.TransformAsync(new Customer { AgeText = " 8 ", Age = 2 });
        Assert.False(thenPending.IsCompletedSuccessfully);
        var thenError = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await thenPending.ConfigureAwait(true));
        Assert.Equal("async-then:8", thenError.Message);
        Assert.Equal(0, thenLater);
    }

    [Fact]
    public async Task Cancellation_is_honoured_mid_pipeline()
    {
        var addLater = 0;
        using var duringAdd = new CancellationTokenSource();
        var addChain = new Transformer<Customer>();
        addChain.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Map<string, string>(name =>
            {
                duringAdd.Cancel();
                return name.Trim();
            }))
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

        var thenLater = 0;
        using var duringThen = new CancellationTokenSource();
        var conversion = new Transformer<Customer>();
        conversion.RuleFor(customer => customer.AgeText)
            .Convert(TransformerRules.Map<string, string>(text => text.Trim()))
            .Then(TransformerRules.Map<string, string>(text =>
            {
                duringThen.Cancel();
                return text;
            }))
            .Then<int>((_, _) =>
            {
                thenLater++;
                return new ValueTask<int>(1);
            })
            .Into(customer => customer.Age);

        var unchanged = new Customer { AgeText = " 9 ", Age = 4 };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await conversion.TransformAsync(unchanged, duringThen.Token).ConfigureAwait(true));
        Assert.Equal(0, thenLater);
        Assert.Equal(4, unchanged.Age);
        Assert.Equal(" 9 ", unchanged.AgeText);
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
