using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer.Tests;

public class PropertyTransformerTests
{
    [Fact]
    public void TransformationException_keeps_only_the_message()
    {
        var empty = new TransformationException();
        Assert.Null(empty.InnerException);

        var message = new TransformationException("plain");
        Assert.Equal("plain", message.Message);
        Assert.Null(message.InnerException);

        var inner = new InvalidOperationException("inner");
        var wrapped = new TransformationException("outer", inner);
        Assert.Equal("outer", wrapped.Message);
        Assert.Same(inner, wrapped.InnerException);
    }

    [Fact]
    public async Task Sync_rule_failure_names_the_property_and_keeps_the_inner_exception()
    {
        var later = 0;
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name)
            .Add(TransformerRules.Trim())
            .Add((_, _) => throw new InvalidOperationException("boom"))
            .Add((_, _) =>
            {
                later++;
                return new ValueTask<string>("skipped");
            });
        transformer.RuleFor(customer => customer.Age).Add((_, _) =>
        {
            later++;
            return new ValueTask<int>(1);
        });
        var customer = new Customer { Name = "  ada  ", Age = 4 };

        var error = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await transformer.TransformAsync(customer).ConfigureAwait(true));

        Assert.IsAssignableFrom<TransformationException>(error);
        Assert.Equal("Name", error.PropertyName);
        Assert.Equal("Transformation failed for property 'Name'.", error.Message);
        var inner = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Equal("boom", inner.Message);
        Assert.Equal(0, later);
        Assert.Equal("  ada  ", customer.Name);
        Assert.Equal(4, customer.Age);
    }

    [Fact]
    public async Task Async_rule_failure_names_the_property_and_keeps_the_inner_exception()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Trim();
        transformer.RuleFor(customer => customer.Age).Add(async (age, _) =>
        {
            await Task.Yield();
            throw new InvalidOperationException("age:" + age);
        });
        var customer = new Customer { Name = "  ada  ", Age = 4 };

        var pending = transformer.TransformAsync(customer);
        Assert.False(pending.IsCompletedSuccessfully);
        var error = await Assert.ThrowsAsync<PropertyTransformationException>(
            async () => await pending.ConfigureAwait(true));

        Assert.IsAssignableFrom<TransformationException>(error);
        Assert.Equal("Age", error.PropertyName);
        Assert.Equal("Transformation failed for property 'Age'.", error.Message);
        var inner = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Equal("age:4", inner.Message);
        Assert.Equal("ada", customer.Name);
        Assert.Equal(4, customer.Age);
    }

    [Fact]
    public async Task Sync_cancellation_is_not_wrapped()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Add((_, _) => throw new OperationCanceledException("stop"));

        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await transformer.TransformAsync(new Customer { Name = "ada" }).ConfigureAwait(true));

        Assert.IsNotType<PropertyTransformationException>(error);
        Assert.Equal("stop", error.Message);
    }

    [Fact]
    public async Task Async_cancellation_is_not_wrapped()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Add(async (name, _) =>
        {
            await Task.Yield();
            throw new OperationCanceledException("stop-async:" + name);
        });

        var pending = transformer.TransformAsync(new Customer { Name = "ada" });
        Assert.False(pending.IsCompletedSuccessfully);
        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await pending.ConfigureAwait(true));

        Assert.IsNotType<PropertyTransformationException>(error);
        Assert.Equal("stop-async:ada", error.Message);
    }

    [Fact]
    public async Task Struct_record_and_init_only_properties_still_return_the_updated_value()
    {
        var points = new Transformer<Point>();
        points.RuleFor(point => point.X).Add(1);
        var point = await points.TransformAsync(new Point { X = 1, Y = 5 });
        Assert.Equal(2, point.X);
        Assert.Equal(5, point.Y);

        var initPoints = new Transformer<InitPoint>();
        initPoints.RuleFor(item => item.X).Add(1);
        var initOriginal = new InitPoint { X = 1, Y = 5 };
        var initUpdated = await initPoints.TransformAsync(initOriginal);
        Assert.Equal(1, initOriginal.X);
        Assert.Equal(2, initUpdated.X);
        Assert.Equal(5, initUpdated.Y);

        var people = new Transformer<Person>();
        people.RuleFor(person => person.Name).Trim();
        var original = new Person("  ada  ", 4);
        var updated = await people.TransformAsync(original);
        Assert.Equal("  ada  ", original.Name);
        Assert.Equal("ada", updated.Name);
        Assert.NotSame(original, updated);

        var badges = new Transformer<Badge>();
        badges.RuleFor(badge => badge.Label).Trim();
        var badge = new Badge { Label = "  ada  " };
        var cloned = await badges.TransformAsync(badge);
        Assert.Equal("  ada  ", badge.Label);
        Assert.Equal("ada", cloned.Label);
        Assert.NotSame(badge, cloned);
    }

    [Fact]
    public async Task Null_instance_is_not_wrapped_as_a_property_failure()
    {
        var transformer = new Transformer<Customer>();
        transformer.RuleFor(customer => customer.Name).Trim();

        await Assert.ThrowsAsync<NullReferenceException>(
            async () => await transformer.TransformAsync(null!).ConfigureAwait(true));
    }

    [Fact]
    public void RuleFor_rejects_indexers_get_only_properties_and_static_properties()
    {
        var indexed = new Transformer<Indexed>();
        var indexer = Assert.Throws<ArgumentException>(() => indexed.RuleFor(item => item[0]));
        Assert.Contains("Indexers are not supported.", indexer.Message, StringComparison.Ordinal);

        var readOnly = new Transformer<ReadOnlyName>();
        var getter = Assert.Throws<ArgumentException>(() => readOnly.RuleFor(item => item.Name));
        Assert.Contains("no public settable or init accessor", getter.Message, StringComparison.Ordinal);

        var shared = new Transformer<Customer>();
        var staticProperty = Assert.Throws<ArgumentException>(() => shared.RuleFor(_ => Customer.Shared));
        Assert.Contains("simple property", staticProperty.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Add_rejects_a_null_rule()
    {
        var transformer = new Transformer<Customer>();
        var property = transformer.RuleFor(customer => customer.Name);

        Assert.Throws<ArgumentNullException>(() => property.Add((ITransformerRule<string>)null!));
        Assert.Throws<ArgumentNullException>(() => property.Add((Func<string, CancellationToken, ValueTask<string>>)null!));
    }

    [Fact]
    public void PropertyTransformer_exposes_the_property_name()
    {
        var transformer = new Transformer<Customer>();
        var property = transformer.RuleFor(customer => customer.Name);

        Assert.Equal("Name", property.PropertyName);
    }

    public sealed class Customer
    {
        public static string Shared { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }

    public struct Point
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public struct InitPoint
    {
        public int X { get; init; }

        public int Y { get; init; }
    }

    public sealed record Person(string Name, int Age);

    public sealed class Badge
    {
        public string Label { get; init; } = string.Empty;
    }

    public sealed class Indexed
    {
        public string this[int index]
        {
            get => string.Empty;
            set { }
        }
    }

    public sealed class ReadOnlyName
    {
        public string Name { get; } = string.Empty;
    }
}
