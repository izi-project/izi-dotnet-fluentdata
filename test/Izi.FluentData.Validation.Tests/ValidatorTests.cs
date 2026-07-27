namespace Izi.FluentData.Validation.Tests;

/// <summary>
/// Verifies <see cref="Validator{T}"/> end-to-end: error aggregation across properties, multiple rules per
/// property, custom <c>Must</c> predicates, whole-instance (cross-field) rules, and dependent rules that only
/// run once their parent passes.
/// </summary>
public class ValidatorTests
{
    public sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    // Multiple rules per property, across several properties.
    public sealed class PersonValidator : Validator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaxLength(5);
            RuleFor(x => x.Age).GreaterThan(0);
            RuleFor(x => x.Email).Email();
        }
    }

    [Fact]
    public async Task Valid_instance_yields_no_errors()
    {
        var person = new Person { Name = "Anna", Age = 30, Email = "anna@example.com" };
        Assert.Empty(await new PersonValidator().ValidateAsync(person));
    }

    [Fact]
    public async Task Each_broken_rule_yields_one_error()
    {
        // Name present but too long, age non-positive, email malformed.
        var person = new Person { Name = "Alexander", Age = 0, Email = "nope" };
        var errors = await new PersonValidator().ValidateAsync(person);
        Assert.Equal(3, errors.Count);
    }

    [Fact]
    public async Task Multiple_rules_on_one_property_are_all_evaluated()
    {
        var person = new Person { Name = "TooLong", Age = 20, Email = "a@b.com" };
        var errors = await new PersonValidator().ValidateAsync(person);
        Assert.Single(errors); // only MaxLength fails
        Assert.Contains(errors, e => e.Contains("maximum length"));
    }

    [Fact]
    public async Task Empty_name_fails_notempty_only()
    {
        var person = new Person { Name = "", Age = 20, Email = "a@b.com" };
        var errors = await new PersonValidator().ValidateAsync(person);
        Assert.Single(errors); // NotEmpty fails; MaxLength("") passes
        Assert.Contains(errors, e => e.Contains("empty"));
    }

    // ---- Must (custom predicate) ----
    public sealed class AdultValidator : Validator<Person>
    {
        public AdultValidator() => RuleFor(x => x.Age).Must(age => age >= 18, "Must be an adult.");
    }

    [Fact]
    public async Task Must_runs_custom_predicate()
    {
        Assert.Equal("Must be an adult.", Assert.Single(await new AdultValidator().ValidateAsync(new Person { Age = 10 })));
        Assert.Empty(await new AdultValidator().ValidateAsync(new Person { Age = 20 }));
    }

    // ---- Self / whole-instance rule (cross-field validation) ----
    public sealed class ConsistencyValidator : Validator<Person>
    {
        public ConsistencyValidator()
            => RuleFor(x => x).Must(p => p.Age < 18 || !string.IsNullOrEmpty(p.Email), "Adults must have an email.");
    }

    [Fact]
    public async Task Self_rule_validates_across_fields()
    {
        // Adult without an email fails the cross-field rule.
        Assert.Equal("Adults must have an email.",
            Assert.Single(await new ConsistencyValidator().ValidateAsync(new Person { Age = 30, Email = "" })));

        // Adult with an email, and a minor without one, both pass.
        Assert.Empty(await new ConsistencyValidator().ValidateAsync(new Person { Age = 30, Email = "a@b.com" }));
        Assert.Empty(await new ConsistencyValidator().ValidateAsync(new Person { Age = 10, Email = "" }));
    }

    // ---- Dependent rules (only run if the parent's own rules passed) ----
    // RuleFor(...) returns a nested Validator<string>; WithDependents attaches a second nested
    // validator that only runs once NotEmpty (declared directly on the first) has passed.
    public sealed class DependentNameValidator : Validator<Person>
    {
        public DependentNameValidator()
            => RuleFor(x => x.Name)
                .NotEmpty("Value cannot be empty.")
                .WithDependents(v => v.MinLength(3));
    }

    [Fact]
    public async Task Dependent_rule_runs_when_parent_passes()
    {
        var errors = await new DependentNameValidator().ValidateAsync(new Person { Name = "ab" });
        Assert.Contains(errors, e => e.Contains("minimum length"));
    }

    [Fact]
    public async Task Dependent_rule_skipped_when_parent_fails()
    {
        var errors = await new DependentNameValidator().ValidateAsync(new Person { Name = "" });
        Assert.Single(errors); // only the parent; MinLength not evaluated
        Assert.Contains(errors, e => e.Contains("empty"));
    }

    [Fact]
    public async Task Dependent_rule_passes_when_both_satisfied()
        => Assert.Empty(await new DependentNameValidator().ValidateAsync(new Person { Name = "abc" }));

    // ---- Dependent rules authored inline via the WithDependents(Action<Validator<T>>) lambda ----
    public sealed class InlineDependentValidator : Validator<Person>
    {
        public InlineDependentValidator()
            => RuleFor(x => x.Name)
                .NotEmpty()
                .WithDependents(x =>
                {
                    x.MinLength(3);
                    x.MaxLength(5);
                });
    }

    [Fact]
    public async Task Inline_dependent_rules_run_when_parent_passes()
    {
        // "ab": NotEmpty passes, MinLength(3) fails.
        Assert.Contains(await new InlineDependentValidator().ValidateAsync(new Person { Name = "ab" }), e => e.Contains("minimum length"));
        // "abcdef": MaxLength(5) fails.
        Assert.Contains(await new InlineDependentValidator().ValidateAsync(new Person { Name = "abcdef" }), e => e.Contains("maximum length"));
        // "abcd": both dependents pass.
        Assert.Empty(await new InlineDependentValidator().ValidateAsync(new Person { Name = "abcd" }));
    }

    [Fact]
    public async Task Inline_dependent_rules_skipped_when_parent_fails()
        => Assert.Single(await new InlineDependentValidator().ValidateAsync(new Person { Name = "" })); // only NotEmpty

    // ---- MustAsync (custom asynchronous predicate) ----
    // Simulates an I/O-bound check (e.g. a uniqueness lookup) by actually yielding via Task.Delay,
    // so the rule's async slow-path is exercised rather than a synchronously-completed ValueTask.
    public sealed class AsyncAdultValidator : Validator<Person>
    {
        public AsyncAdultValidator()
            => RuleFor(x => x.Age).MustAsync(async (age, ct) =>
            {
                await Task.Delay(1, ct);
                return age >= 18;
            }, "Must be an adult.");
    }

    [Fact]
    public async Task MustAsync_runs_asynchronous_predicate()
    {
        Assert.Equal("Must be an adult.", Assert.Single(await new AsyncAdultValidator().ValidateAsync(new Person { Age = 10 })));
        Assert.Empty(await new AsyncAdultValidator().ValidateAsync(new Person { Age = 20 }));
    }

    // The Func<T, Task<bool>> overload (no CancellationToken parameter).
    public sealed class AsyncNoTokenValidator : Validator<Person>
    {
        public AsyncNoTokenValidator()
            => RuleFor(x => x.Email).MustAsync(async email =>
            {
                await Task.Yield();
                return !string.IsNullOrEmpty(email);
            }, "Email is required.");
    }

    [Fact]
    public async Task MustAsync_supports_predicate_without_cancellation_token()
    {
        Assert.Equal("Email is required.", Assert.Single(await new AsyncNoTokenValidator().ValidateAsync(new Person { Email = "" })));
        Assert.Empty(await new AsyncNoTokenValidator().ValidateAsync(new Person { Email = "a@b.com" }));
    }

    // ---- Message factories through the fluent extensions ----
    // Every extension that takes a constant message also takes a Func<T, string> that receives the failing value.
    public sealed class InstanceAwareMessageValidator : Validator<Person>
    {
        public InstanceAwareMessageValidator()
        {
            RuleFor(x => x.Name).NotEmpty(name => $"'{name}' is not a name.").MaxLength(5, name => $"'{name}' is too long.");
            RuleFor(x => x.Age).GreaterThan(0, age => $"{age} is not a valid age.");
            RuleFor(x => x.Email).Email(email => $"'{email}' is not an email.");
        }
    }

    [Fact]
    public async Task Extension_message_factories_quote_the_failing_value()
    {
        var errors = await new InstanceAwareMessageValidator().ValidateAsync(new Person { Name = "Alexander", Age = 0, Email = "nope" });

        Assert.Equal(3, errors.Count);
        Assert.Contains("'Alexander' is too long.", errors);
        Assert.Contains("0 is not a valid age.", errors);
        Assert.Contains("'nope' is not an email.", errors);
    }

    [Fact]
    public async Task Extension_message_factories_are_not_invoked_when_the_rules_pass()
        => Assert.Empty(await new InstanceAwareMessageValidator().ValidateAsync(new Person { Name = "Anna", Age = 30, Email = "anna@example.com" }));

    // ---- Must / MustAsync with an instance-aware message ----
    public sealed class MustWithFactoryValidator : Validator<Person>
    {
        public MustWithFactoryValidator()
        {
            RuleFor(x => x.Age).Must(age => age >= 18, age => $"{age} is too young.");
            RuleFor(x => x.Email).MustAsync(async (email, ct) =>
            {
                await Task.Delay(1, ct);
                return email.EndsWith("@example.com");
            }, email => $"'{email}' is not a company address.");
            RuleFor(x => x.Name).MustAsync(async name =>
            {
                await Task.Yield();
                return name.Length > 1;
            }, name => $"'{name}' is too short.");
        }
    }

    [Fact]
    public async Task Must_and_MustAsync_accept_a_message_factory()
    {
        var errors = await new MustWithFactoryValidator().ValidateAsync(new Person { Name = "A", Age = 10, Email = "a@b.com" });

        Assert.Equal(3, errors.Count);
        Assert.Contains("10 is too young.", errors);
        Assert.Contains("'a@b.com' is not a company address.", errors);
        Assert.Contains("'A' is too short.", errors);

        Assert.Empty(await new MustWithFactoryValidator().ValidateAsync(new Person { Name = "Anna", Age = 30, Email = "anna@example.com" }));
    }

    // The token passed to ValidateAsync is threaded straight through to the predicate.
    public sealed class CancellableValidator : Validator<Person>
    {
        public CancellableValidator()
            => RuleFor(x => x.Name).MustAsync(async (_, ct) =>
            {
                await Task.Delay(1000, ct);
                return true;
            }, "unreachable");
    }

    [Fact]
    public async Task MustAsync_honours_cancellation_token()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await new CancellableValidator().ValidateAsync(new Person { Name = "x" }, cts.Token));
    }
}
