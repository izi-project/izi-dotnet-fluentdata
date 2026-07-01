# Izi.FluentData.Validation

Fluent, **dependency-free** object validation for **.NET 10**.

Subclass `Validator<T>`, declare rules per property (or for the whole instance), and `ValidateAsync` runs every rule and returns the collected error messages — **an empty list means valid**. The success path is allocation-free, the API is `ValueTask`-based, and the format rules are backed by **source-generated** regular expressions.

```bash
dotnet add package Izi.FluentData.Validation
```

- **Target framework:** `net10.0`
- **Dependencies:** none (zero transitive packages)
- **Thread-safety:** a built validator is immutable and safe to share as a singleton

---

## Quick start

```csharp
using Izi.FluentData.Validation;

public sealed class CustomerValidator : Validator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaxLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Age).InRange(18, 120);
    }
}

var validator = new CustomerValidator();               // build once, reuse forever
IReadOnlyList<string> errors = await validator.ValidateAsync(customer);
if (errors.Count > 0)
{
    // surface errors
}
```

`RuleFor` returns a mutable `ValidatorRuleBuilder<TProperty>` — rules chain directly off it.

---

## Design & technical specifications

### Mutable accumulator Builder

Because every rule for a given selector shares the same type `T`, the builder can be a simple **mutable accumulator**. `ValidatorRuleBuilder<T>.AddRule(...)` appends to an internal list and records the rule as `_current`, so refinements (`WithMessage`, `WithDependent`) always target the **most recently added** rule:

```csharp
RuleFor(x => x.Name)
    .NotNull().WithMessage("Name is required.")   // refines NotNull
    .MaxLength(50);                               // a new rule
```

### Rule = Strategy over a predicate

A `ValidatorRule<T>` wraps one `Func<T, CancellationToken, ValueTask<bool>>` predicate plus the message(s) reported on failure. The built-in catalogue on the static `ValidatorRules` class is a library of these strategies; `Must(...)` is the open escape hatch.

### Composite aggregation

`CompositeValidatorRule<T>` holds the ordered rule set and runs **all** of them in one pass, unioning every failure message — so a caller sees the complete list of problems, not just the first. Its primary constructor uses `params ReadOnlySpan<ValidatorRule<T>>` (C# 13), so composing rules from a span allocates no backing array.

### Dependent rules (conditional short-circuit)

A rule can carry **dependents** that run **only if the parent passed**. This avoids cascading noise — e.g. don't bother format-checking an email that is already known to be empty. Dependents form a small tree, evaluated depth-first.

---

## Performance & .NET 10 optimizations

- **Allocation-free success path.** A passing rule returns a shared, static empty result; `Validator<T>` returns a shared `NoErrors` singleton when nothing failed. A valid instance therefore allocates **nothing**.
- **Lazily-allocated error list.** The aggregation list stays `null` until the first failure, so partially-valid instances pay only for the errors they actually produce.
- **Synchronous fast path, no async state machine.** Predicates return `ValueTask<bool>`; `ValidateAsync` checks `IsCompletedSuccessfully` and stays fully synchronous while rules complete synchronously (all built-ins do). The `static`, `this`-free slow path is reached only when a custom async rule genuinely yields.
- **Source-generated regex.** `EmailAddress` and `CreditCard` are backed by `[GeneratedRegex]` partial methods — the matcher is generated at **compile time**, so there is no runtime `Regex` construction/compilation cost and the patterns are trimming/AOT-friendly.
- **Single source of truth for ISO data.** Country codes are stored once as aligned rows and projected into three lookup `HashSet`s at type-init, so the alpha-2/alpha-3/numeric sets can never drift out of sync.
- **Pre-sized buffers.** A rule's message list is pre-sized to 1 — the overwhelmingly common case.

### Benchmarks

`BenchmarkDotNet v0.15.8` · `.NET 10.0.7` · 13th Gen Intel Core i7-13700K · `[MemoryDiagnoser]`

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `ValidateValid` (3 properties, all pass) | 71.71 ns | **0 B** | **allocation-free** success path |
| `ValidateInvalid` (3 properties, all fail) | 125.35 ns | 352 B | pays only for the error list it must return |

> Reproduce locally:
> ```bash
> dotnet run -c Release --project benchmark/Izi.FluentData.Validation.Benchmarks -- --filter *
> ```

---

## Built-in rules

Every rule has an overload taking a custom message, e.g. `.NotEmpty("Name is required.")`.

| Category | Rules |
| --- | --- |
| Null & emptiness | `NotNull`, `Null`, `NotEmpty`, `Empty` |
| Equality | `Equal`, `NotEqual` |
| Comparison *(`IComparable<T>`)* | `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual`, `InRange`, `NotInRange` |
| Length *(strings & collections)* | `Length`, `MinLength`, `MaxLength` |
| Numeric | `ScalePrecision` |
| Pattern | `Matches`, `NotMatches`, `EmailAddress`, `CreditCard` |
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

### Refining messages

`WithMessage` / `WithMessages` replace the failure message(s) of the most recently added rule, so chain them directly after it:

```csharp
RuleFor(x => x.Name).NotNull().WithMessage("Name is required.");
```

### Dependent rules

Dependent rules run **only if their parent rule passed**. Attach a prebuilt rule from the `ValidatorRules` factory with `WithDependent`:

```csharp
using Izi.FluentData.Validation.Rules;

// Format is only checked once the value is known to be non-empty.
RuleFor(x => x.Email)
    .NotEmpty()
    .WithDependent(ValidatorRules.Email<string>());

// Compose a parent + dependent as a single prebuilt rule, then add it.
RuleFor(x => x.Name).AddRule(
    ValidatorRules.NotEmpty<string>("Name is required.")
        .WithDependent(ValidatorRules.MinLength<string>(3, "Name must be at least 3 characters.")));
```

`WithDependent` also accepts a predicate directly — `WithDependent((value, ct) => …)` — when you don't need a named rule.

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
