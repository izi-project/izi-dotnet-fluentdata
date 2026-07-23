# Izi.FluentData.Validation

Fluent, **dependency-free** object validation for **.NET 10**.

Subclass `Validator<T>`, declare rules per property (or for the whole instance), and `ValidateAsync` runs every rule and returns the collected error messages — **an empty list means valid**. The success path is allocation-free, the API is `ValueTask`-based, and the format rules are backed by **source-generated** regular expressions.

```bash
dotnet add package Izi.FluentData.Validation
```

- **Target framework:** `net10.0`
- **Dependencies:** none (zero transitive packages)
- **Thread-safety:** safe to share as a singleton *after configuration* (i.e., don’t call `AddRule`/`WithDependents` after construction)

---

## Quick start

```csharp
using Izi.FluentData.Validation;

public sealed class CustomerValidator : Validator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaxLength(50);
        RuleFor(x => x.Email).NotEmpty().Email();
        RuleFor(x => x.Age).Range(18, 120);
    }
}

var validator = new CustomerValidator();               // build once, reuse forever
IReadOnlyList<string> errors = await validator.ValidateAsync(customer);
if (errors.Count > 0)
{
    // surface errors
}
```

`RuleFor` returns a nested `Validator<TProperty>` — rules chain directly off it.

---

## Design & technical specifications

### `Validator<T>` is the whole engine — no separate builder

There is no builder or "compose" step: `Validator<T>` accumulates its own rules (`AddRule`) and its own dependent validators (`WithDependents`) directly, and `RuleFor<TProperty>` is sugar that creates a nested `Validator<TProperty>`, wires it in as a dependent, and returns it so rules chain straight onto it:

```csharp
RuleFor(x => x.Name)     // returns a new Validator<string>, wired as a dependent of `this`
    .NotEmpty()           // Validator<string>.AddRule(...)
    .MaxLength(50);       // Validator<string>.AddRule(...)
```

Because rules are added eagerly instead of being "built" from an accumulated list, there's no lazy-build step and nothing to race.

### Rule = one predicate, one message

`ValidatorRule<T>` (the `IValidatorRule<T>` implementation returned by every `ValidatorRules` factory) wraps a single `Func<T, CancellationToken, ValueTask<bool>>` predicate and the single message reported on failure. The built-in catalogue on the static `ValidatorRules` class is a library of these; `Must(...)` is the open escape hatch for anything the catalogue doesn't cover.

### Two-tier aggregation: rules, then dependents

Every `Validator<T>.ValidateAsync` call runs in two tiers:

1. **Its own rules** (added via `AddRule`/the fluent extensions) — **all** of them run and **all** their failure messages are collected, so a property with several rules reports every one that fails, not just the first.
2. **Its dependents** (added via `WithDependents`, including every `RuleFor` declaration) — but **only if every rule in tier 1 passed**.

That second tier is what makes `RuleFor(x => x.Name)` and `RuleFor(x => x.Age)` independent of each other: each lives on its own nested `Validator<TProperty>`, so a failing `Name` rule never suppresses the `Age` rule. The short-circuit only applies *within* one nested validator's own tier.

> **Sharp edge:** if you call `AddRule` directly on a validator that *also* uses `RuleFor`, a failing whole-instance `AddRule` rule will skip **every** `RuleFor` dependent, not just a related one. Prefer `RuleFor(x => x).Must(...)` for cross-field rules — it lives in its own dependent tier and won't interfere with sibling properties.

### Dependent rules (conditional cascades)

`WithDependents` attaches validators that only run once the parent's own rules passed — e.g. don't bother checking a minimum length until you know the value isn't empty:

```csharp
RuleFor(x => x.Name)
    .NotEmpty()
    .WithDependents(v => v.MinLength(3));
```

---

## Performance & .NET 10 optimizations

- **Allocation-free success path.** A passing rule returns a cached `null`; `Validator<T>` returns a shared empty result when nothing failed. A valid instance therefore allocates **nothing**.
- **Lazily-allocated error list.** The aggregation list stays `null` until the first failure, so partially-valid instances pay only for the errors they actually produce.
- **Synchronous fast path, no async state machine.** Predicates return `ValueTask<bool>`; `ValidateAsync` checks `IsCompletedSuccessfully` and stays fully synchronous while rules complete synchronously (all built-ins do). The slow path is reached only when a custom async rule genuinely yields.
- **Source-generated regex.** `Email` and `CreditCard` are backed by `[GeneratedRegex]` partial methods — the matcher is generated at **compile time**, so there is no runtime `Regex` construction/compilation cost and the patterns are trimming/AOT-friendly.
- **Single source of truth for ISO data.** Country codes are stored once as aligned rows and projected into three lookup `HashSet`s at type-init, so the alpha-2/alpha-3/numeric sets can never drift out of sync.

### Benchmarks

`BenchmarkDotNet v0.15.8` · `.NET 10.0.8` · Intel Core i7-9700F CPU 3.00GHz · `[MemoryDiagnoser]`

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `ValidateValid` (3 properties, all pass) | 103.5 ns | **0 B** | **allocation-free** success path |
| `ValidateInvalid` (3 properties, all fail) | 191.0 ns | 352 B | pays only for the error list it must return |

> Reproduce locally:
> ```bash
> dotnet run -c Release --project benchmark/Izi.FluentData.Validation.Benchmarks -- --filter '*'
> ```

---

## Built-in rules

Every rule has an overload taking a custom message, e.g. `.NotEmpty("Name is required.")`.

| Category | Rules |
| --- | --- |
| Null & emptiness | `NotNull`, `Null`, `NotEmpty`, `Empty` |
| Equality | `Equal`, `NotEqual` |
| Comparison *(`IComparable<T>`)* | `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual`, `Range`, `NotRange` |
| Length *(strings & collections)* | `Length`, `MinLength`, `MaxLength` |
| Numeric | `ScalePrecision` |
| Pattern | `Matches`, `NotMatches`, `Email`, `CreditCard` |
| ISO codes | `CountryIso2`, `CountryIso3`, `CountryIsoNumeric`, `CurrencyIso` |
| Custom | `Must` |

The ISO rules validate against curated, dependency-free code sets (ISO 3166-1 alpha-2/alpha-3/numeric and ISO 4217); alpha codes match case-insensitively.

---

## Recipes

### Custom predicates with `Must`

```csharp
RuleFor(x => x.Password).Must(p => p.Any(char.IsDigit), "Password must contain a digit.");
```

### Whole-instance (cross-field) rules

Pass the identity selector `x => x` to validate the entire instance:

```csharp
RuleFor(x => x).Must(
    c => c.Age < 18 || !string.IsNullOrEmpty(c.Email),
    "Adults must have an email.");
```

### Dependent rules

Dependent rules run **only if their parent's own rules passed**. Attach one or more via `WithDependents`, configuring a nested validator inline:

```csharp
// Format is only checked once the value is known to be non-empty.
RuleFor(x => x.Email)
    .NotEmpty()
    .WithDependents(v => v.Email());

// Or attach a pre-built nested validator directly.
RuleFor(x => x.Name).NotEmpty().WithDependents(new Validator<string>().MinLength(3));

// The configure callback can declare several dependent rules at once;
// all of them run against the same value once the parent passes.
RuleFor(x => x.Name)
    .NotEmpty()
    .WithDependents(v =>
    {
        v.MinLength(3, "Name must be at least 3 characters.");
        v.MaxLength(50);
    });
```

### Asynchronous rules

Any rule is `ValueTask<bool>`-based, so an `async` check (e.g. a uniqueness lookup) drops straight in via a prebuilt rule:

```csharp
using Izi.FluentData.Validation.Rules;

RuleFor(x => x.Email).AddRule(new ValidatorRule<string>(
    async (email, ct) => await store.IsUniqueAsync(email, ct),
    "Email is already registered."));
```

---

## Dependency injection

Use the companion package **[Izi.FluentData.Validation.DependencyInjectionExtensions](https://www.nuget.org/packages/Izi.FluentData.Validation.DependencyInjectionExtensions)**:

```csharp
services.AddValidator<CustomerValidator>();   // singleton (recommended)
```

A validator builds its rule set once in the constructor and is stateless afterward, so a shared singleton is ideal.

---

## Links

- Repository & full documentation: <https://github.com/izi-project/izi-dotnet-fluentdata>
- License: MIT
