namespace Izi.FluentData.Validation.Tests;

public class ValidatorTests
{
    private sealed class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public string? Email { get; set; }

        public Address? Address { get; set; }

        public List<string>? Items { get; set; }

        public List<Address>? Places { get; set; }

        public string this[int index] => Name ?? string.Empty;
    }

    private sealed class Address
    {
        public string? Street { get; set; }

        public City? City { get; set; }
    }

    private sealed class City
    {
        public string? Name { get; set; }
    }

    [Fact]
    public async Task A_valid_object_returns_the_shared_success_result()
    {
        var validator = PersonValidator();
        var result = await validator.ValidateAsync(new Person { Name = "Ada", Age = 30, Email = "ada@example.com" }).ConfigureAwait(true);
        Assert.True(result.IsValid);
        Assert.Same(ValidationResult.Success, result);
        Assert.Empty(result.Errors);
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public async Task Each_failed_property_keeps_its_name_and_default_message()
    {
        var result = await PersonValidator().ValidateAsync(new Person { Name = "Alexander", Age = 0, Email = "nope" }).ConfigureAwait(true);
        Assert.Equal(3, result.Errors.Count);
        Assert.Equal("Name", result.Errors[0].PropertyName);
        Assert.Equal("Must be at most 5 characters.", result.Errors[0].Errors[0]);
        Assert.Equal("Age", result.Errors[1].PropertyName);
        Assert.Equal("Must be greater than 0.", result.Errors[1].Errors[0]);
        Assert.Equal("Email", result.Errors[2].PropertyName);
        Assert.Equal("Email: Must be a valid email address.", result.Errors[2].ToString());
    }

    [Fact]
    public async Task Several_rules_on_one_property_are_all_reported()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().MaxLength(3);
        var result = await validator.ValidateAsync(new Person { Name = "    " }).ConfigureAwait(true);
        var error = Assert.Single(result.Errors);
        Assert.Equal(new[] { "Must not be empty.", "Must be at most 3 characters." }, error.Errors);
        Assert.Equal(
            "Name: Must not be empty." + Environment.NewLine + "Name: Must be at most 3 characters.",
            error.ToString());
        Assert.Equal("Name: Must not be empty. | Name: Must be at most 3 characters.", error.ToString(" | "));
    }

    [Fact]
    public async Task WithMessage_replaces_the_last_rule_verbatim_and_WithName_renames_the_leaf()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().WithMessage("Required.").WithName("FullName");
        var result = await validator.ValidateAsync(new Person { Name = "" }).ConfigureAwait(true);
        var error = Assert.Single(result.Errors);
        Assert.Equal("FullName", error.PropertyName);
        Assert.Equal("Required.", Assert.Single(error.Errors));
        Assert.Equal("FullName: Required.", result.ToString());
    }

    [Fact]
    public async Task A_null_parent_is_one_message_and_the_leaf_rules_do_not_run()
    {
        var ran = 0;
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Address!.Street)
            .Must(_ =>
            {
                ran++;
                return false;
            }, "Must not be blank.");
        var result = await validator.ValidateAsync(new Person()).ConfigureAwait(true);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Address", error.PropertyName);
        Assert.Equal("Must not be null.", Assert.Single(error.Errors));
        Assert.Equal(0, ran);

        var named = new Validator<Person>();
        named.RuleFor(person => person.Address!.Street).NotEmpty().WithName("StreetLine");
        var renamed = await named.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.Equal("Address", Assert.Single(renamed.Errors).PropertyName);

        var present = await named.ValidateAsync(new Person { Address = new Address { Street = "" } }).ConfigureAwait(true);
        Assert.Equal("StreetLine", Assert.Single(present.Errors).PropertyName);
        Assert.Equal("Address.City.Name", (await CityName().ConfigureAwait(true)).Errors[0].PropertyName);
    }

    [Fact]
    public async Task When_skips_the_property_and_repeated_calls_combine_with_and()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().When(person => person.Age >= 18).When(person => person.Age < 100);
        Assert.True((await validator.ValidateAsync(new Person { Age = 10, Name = "" }).ConfigureAwait(true)).IsValid);
        Assert.True((await validator.ValidateAsync(new Person { Age = 120, Name = "" }).ConfigureAwait(true)).IsValid);
        Assert.False((await validator.ValidateAsync(new Person { Age = 20, Name = "" }).ConfigureAwait(true)).IsValid);

        var skippedParent = new Validator<Person>();
        skippedParent.RuleFor(person => person.Address!.Street).NotEmpty().When(_ => false);
        Assert.True((await skippedParent.ValidateAsync(new Person()).ConfigureAwait(true)).IsValid);
    }

    [Fact]
    public async Task SetValidator_prefixes_child_property_names()
    {
        var address = new Validator<Address>();
        address.RuleFor(item => item.Street).NotEmpty();
        address.RuleFor(item => item.City!.Name).NotEmpty();
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Address!).SetValidator(address);

        var missing = await validator.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.True(missing.IsValid);

        var nested = await validator.ValidateAsync(new Person { Address = new Address { City = new City() } }).ConfigureAwait(true);
        Assert.Equal(2, nested.Errors.Count);
        Assert.Equal("Address.Street", nested.Errors[0].PropertyName);
        Assert.Equal("Address.City.Name", nested.Errors[1].PropertyName);
    }

    [Fact]
    public async Task Include_merges_the_other_validator()
    {
        var name = new Validator<Person>();
        name.RuleFor(person => person.Name).NotEmpty();
        var age = new Validator<Person>();
        age.RuleFor(person => person.Age).GreaterThan(0);
        var both = new Validator<Person>();
        both.Include(name).Include(age);
        var result = await both.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.Equal(new[] { "Name", "Age" }, result.Errors.Select(error => error.PropertyName));
        Assert.Throws<InvalidOperationException>(() => both.Include(both));
    }

    [Fact]
    public async Task RuleForEach_names_the_failing_indexes()
    {
        var validator = new Validator<Person>();
        validator.RuleForEach(person => person.Items!).NotEmpty();
        var result = await validator.ValidateAsync(new Person { Items = ["ok", "", "x"] }).ConfigureAwait(true);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Items[1]", error.PropertyName);
        Assert.Equal("Must not be empty.", Assert.Single(error.Errors));

        var missing = await validator.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.True(missing.IsValid);

        var renamed = new Validator<Person>();
        renamed.RuleForEach(person => person.Items!).NotEmpty().WithName("Lines");
        var lines = await renamed.ValidateAsync(new Person { Items = ["", "ok"] }).ConfigureAwait(true);
        Assert.Equal("Lines[0]", Assert.Single(lines.Errors).PropertyName);
    }

    [Fact]
    public async Task An_async_rule_is_awaited_and_cancellation_propagates()
    {
        var validator = new Validator<Person>();
        var gate = new TaskCompletionSource<bool>();
        validator.RuleFor(person => person.Name).MustAsync(async (string? name, CancellationToken _) =>
        {
            await gate.Task.ConfigureAwait(true);
            return name == "Ada";
        }, "Must be Ada.");
        var pending = validator.ValidateAsync(new Person { Name = "Bea" });
        Assert.False(pending.IsCompleted);
        gate.SetResult(true);
        var result = await pending.ConfigureAwait(true);
        Assert.Equal("Must be Ada.", Assert.Single(Assert.Single(result.Errors).Errors));

        var cancelled = new Validator<Person>();
        var started = new TaskCompletionSource<bool>();
        using var source = new CancellationTokenSource();
        cancelled.RuleFor(person => person.Name).MustAsync(async (string? _, CancellationToken token) =>
        {
            started.TrySetResult(true);
            await Task.Delay(Timeout.Infinite, token).ConfigureAwait(true);
            return true;
        }, "unused");
        var running = cancelled.ValidateAsync(new Person { Name = "Ada" }, source.Token);
        await started.Task.ConfigureAwait(true);
        await source.CancelAsync().ConfigureAwait(true);
        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await running.ConfigureAwait(true));
        Assert.IsNotType<InvalidOperationException>(error);
    }

    [Fact]
    public async Task A_later_rule_is_visible_after_the_compiled_runner_was_cached()
    {
        var validator = new Validator<Person>();
        var name = validator.RuleFor(person => person.Name).NotEmpty();
        Assert.False((await validator.ValidateAsync(new Person { Name = "" }).ConfigureAwait(true)).IsValid);
        Assert.True((await validator.ValidateAsync(new Person { Name = "abcd" }).ConfigureAwait(true)).IsValid);
        name.MaxLength(3);
        var result = await validator.ValidateAsync(new Person { Name = "abcd" }).ConfigureAwait(true);
        Assert.Equal("Must be at most 3 characters.", Assert.Single(Assert.Single(result.Errors).Errors));
    }

    [Fact]
    public void ValidateAsync_rejects_a_null_instance_and_a_bad_selector()
    {
        var validator = new Validator<Person>();
        var missing = Assert.Throws<ArgumentNullException>(() => validator.ValidateAsync(null!));
        Assert.Equal("instance", missing.ParamName);
        Assert.Throws<ArgumentException>(() => validator.RuleFor(person => person.Name!.Trim()));
        Assert.Throws<ArgumentException>(() => validator.RuleFor(person => person[0]));
        Assert.Throws<ArgumentNullException>(() => validator.RuleFor<string>(null!));
        Assert.Throws<InvalidOperationException>(() => new Validator<Person>().RuleFor(person => person.Name).WithMessage("x"));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.Throws<OperationCanceledException>(() =>
            validator.ValidateAsync(new Person { Name = "Ada" }, cancellation.Token));
    }

    [Fact]
    public void A_valid_run_allocates_nothing_after_the_runner_is_compiled()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().MaxLength(32);
        validator.RuleFor(person => person.Age).GreaterThan(0);
        var person = new Person { Name = "Ada", Age = 30, Email = "ada@example.com" };
        var items = new Validator<Person>();
        items.RuleForEach(person => person.Items!).NotEmpty().MaxLength(8);
        var listed = new Person { Items = ["alpha", "beta"] };

        Assert.Equal(0, Measure(validator, person));
        Assert.Equal(0, Measure(items, listed));

        var child = new Validator<Address>();
        child.RuleFor(item => item.Street).NotEmpty();
        var nested = new Validator<Person>();
        nested.RuleForEach(item => item.Places!).SetValidator(child);
        var many = new Person
        {
            Places = Enumerable.Range(0, 100).Select(_ => new Address { Street = "ok" }).ToList(),
        };
        Assert.Equal(0, Measure(nested, many));
    }

    private static long Measure(Validator<Person> validator, Person person)
    {
        for (var i = 0; i < 50; i++)
        {
            var warmed = validator.ValidateAsync(person).GetAwaiter().GetResult();
            if (!warmed.IsValid)
            {
                throw new InvalidOperationException(warmed.ToString());
            }
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            var result = validator.ValidateAsync(person).GetAwaiter().GetResult();
            if (!result.IsValid)
            {
                throw new InvalidOperationException(result.ToString());
            }
        }

        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    [Fact]
    public async Task ToString_joins_failed_properties_with_the_separator()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty();
        validator.RuleFor(person => person.Age).GreaterThan(0);
        var result = await validator.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.Equal(
            "Name: Must not be empty." + Environment.NewLine + "Age: Must be greater than 0.",
            result.ToString());
        Assert.Equal("Name: Must not be empty. / Age: Must be greater than 0.", result.ToString(" / "));
    }

    private static Validator<Person> PersonValidator()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().MaxLength(5);
        validator.RuleFor(person => person.Age).GreaterThan(0);
        validator.RuleFor(person => person.Email).Email();
        return validator;
    }

    private static async Task<ValidationResult> CityName()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Address!.City!.Name).NotEmpty();
        return await validator.ValidateAsync(new Person
        {
            Address = new Address { City = new City { Name = "" } },
        }).ConfigureAwait(true);
    }
}
