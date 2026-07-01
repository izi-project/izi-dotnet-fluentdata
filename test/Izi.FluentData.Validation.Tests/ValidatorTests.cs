using Izi.FluentData.Validation.Rules;

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
            RuleFor(x => x.Email).EmailAddress();
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

    // ---- Dependent rules (only run if the parent rule passed) ----
    // Rules2 attaches dependents on the rule itself via ValidatorRule<T>.WithDependent.
    public sealed class DependentNameValidator : Validator<Person>
    {
        public DependentNameValidator()
            => RuleFor(x => x.Name).AddRule(
                new ValidatorRule<string>((v, _) => ValueTask.FromResult(!string.IsNullOrWhiteSpace(v)), "Value cannot be empty.")
                    .WithDependent(ValidatorRules.MinLength<string>(3)));
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
}
