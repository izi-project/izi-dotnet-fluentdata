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

`ValidatorRule<T>` (the `IValidatorRule<T>` implementation returned by every `ValidatorRules` factory) wraps a single `Func<T, CancellationToken, ValueTask<bool>>` predicate and the single message reported on failure. The built-in catalogue on the static `ValidatorRules` class is a library of these; `Must(...)` / `MustAsync(...)` are the open escape hatch for anything the catalogue doesn't cover.

The message is held as a `Func<T, string>` and is **invoked only when the predicate fails**, so a rule that passes never builds its message. Two constructors feed that delegate:

```csharp
// 1. Constant — reported verbatim.
new ValidatorRule<string>(predicate, "Value is invalid.");

// 2. Factory — receives the value that actually failed.
new ValidatorRule<string>(predicate, value => $"'{value}' is not a valid code.");
```

There is deliberately no `string.Format`-style overload. Constant messages are reported **verbatim**, so braces are ordinary characters — a regex pattern or JSON snippet embedded in a message survives intact, and `Matches`' default message can quote a pattern like `^\d{3}$` without ceremony. Anything needing substitution, culture control, or localisation goes inside the factory, where you have the full language available and pay for it only when a value actually fails.

Both constructors are surfaced by the whole catalogue: every built-in rule — and `Must`/`MustAsync` — has a constant-message overload *and* a `Func<T, string>` overload, so the factory is reachable from the fluent API without dropping down to `AddRule`:

```csharp
RuleFor(x => x.Name).MaxLength(50, name => $"'{name}' is {name.Length} characters; the limit is 50.");
```

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
- **Lazily-built messages.** A rule's message is a `Func<T, string>` invoked only on failure, so a passing rule never touches it — the delegate indirection costs nothing on the hot path, and messages that interpolate the failing value are built only for values that actually fail.
- **Synchronous fast path, no async state machine.** Predicates return `ValueTask<bool>`; `ValidateAsync` checks `IsCompletedSuccessfully` and stays fully synchronous while rules complete synchronously (all built-ins do). The slow path is reached only when a custom async rule genuinely yields.
- **Source-generated regex.** `Email` and `CreditCard` are backed by `[GeneratedRegex]` partial methods — the matcher is generated at **compile time**, so there is no runtime `Regex` construction/compilation cost and the patterns are trimming/AOT-friendly.
- **Single source of truth for ISO data.** Country codes are stored once as aligned rows and projected into three lookup `HashSet`s at type-init, so the alpha-2/alpha-3/numeric sets can never drift out of sync.

### Benchmarks

`BenchmarkDotNet v0.15.8` · `.NET 10.0.7` · Intel Core i7-13700K 3.40GHz · `[MemoryDiagnoser]`

End-to-end, over an object with three validated properties (`ValidatorBenchmarks`):

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `ValidateValid` (all pass) | 57.3 ns | **0 B** | **allocation-free** success path |
| `ValidateInvalid` (all fail) | 98.3 ns | 352 B | pays only for the error list it must return |

#### Message strategies

Evaluating one rule, by how its message is declared (`RuleMessageBenchmarks`):

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `PassConstant` | 1.18 ns | **0 B** | baseline |
| `PassFactory` | 0.84 ns | **0 B** | indistinguishable from the baseline |
| `FailConstant` | 1.33 ns | **0 B** | returns the captured literal |
| `FailFactory` | 28.22 ns | 88 B | builds a message quoting the failing value |

**The passing path is free.** Both strategies are allocation-free and within a nanosecond of each other when the rule passes, because the message delegate is never invoked — the ordering between them is measurement noise at this scale, not a real difference. Message cost is paid only by values that actually fail.

Building one rule, i.e. what a validator's constructor pays once (`RuleConstructionBenchmarks`):

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `BuildConstant` | 18.86 ns | 200 B | eagerly interpolated message string |
| `BuildFactory` | 3.93 ns | **32 B** | non-capturing lambda is cached in a static field |

> Reproduce locally:
> ```bash
> dotnet run -c Release --project benchmark/Izi.FluentData.Validation.Benchmarks -- --filter *
> ```

---

## Built-in rules

Every rule has an overload taking a custom message, e.g. `.NotEmpty("Name is required.")`, and one taking a message factory that receives the failing value, e.g. `.NotEmpty(name => $"'{name}' is not a name.")`.

| Category | Rules |
| --- | --- |
| Null & emptiness | `NotNull`, `Null`, `NotEmpty`, `Empty` |
| Equality | `Equal`, `NotEqual` |
| Comparison *(`IComparable<T>`)* | `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual`, `Range`, `NotRange` |
| Length *(strings & collections)* | `Length`, `MinLength`, `MaxLength` |
| Numeric | `ScalePrecision` |
| Pattern | `Matches`, `NotMatches`, `Email`, `CreditCard` |
| ISO codes | `CountryIso2`, `CountryIso3`, `CountryIsoNumeric`, `CurrencyIso` |
| Custom | `Must`, `MustAsync` |

The ISO rules validate against curated, dependency-free code sets (ISO 3166-1 alpha-2/alpha-3/numeric and ISO 4217); alpha codes match case-insensitively.

---

## Recipes

### Custom predicates with `Must`

```csharp
RuleFor(x => x.Password).Must(p => p.Any(char.IsDigit), "Password must contain a digit.");
```

### Messages that name the failing value

Every rule takes a `Func<T, string>` in place of a constant message when the message needs to quote what was actually rejected. The factory runs only on failure, so a passing rule pays nothing for it:

```csharp
RuleFor(x => x.CountryCode).CountryIso2(code => $"'{code}' is not a supported country code.");
RuleFor(x => x.Password).Must(p => p.Any(char.IsDigit), p => $"'{p}' must contain a digit.");
```

The factory is also where any formatting or localisation belongs — there is no format-string overload, because a lambda already does the job with compile-time checking and without a `params` array:

```csharp
RuleFor(x => x.Age).GreaterThanOrEqual(
    MinimumAge,
    _ => string.Format(CultureInfo.InvariantCulture, "You must be at least {0} to register.", MinimumAge));
```

For a check the catalogue doesn't cover, `ValidatorRule<T>` takes the same pair directly:

```csharp
using Izi.FluentData.Validation.Rules;

RuleFor(x => x.CountryCode).AddRule(new ValidatorRule<string>(
    (code, _) => ValueTask.FromResult(SupportedCountries.Contains(code)),
    code => $"'{code}' is not a supported country code."));
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

`MustAsync` is the async counterpart to `Must` — the usual way to express an I/O-bound check such as a uniqueness lookup. Overloads exist with and without a `CancellationToken`; the token is the one passed to `ValidateAsync`:

```csharp
RuleFor(x => x.Email).MustAsync(
    (email, ct) => store.IsUniqueAsync(email, ct),
    "Email is already registered.");
```

Both `MustAsync` overloads also accept a message factory, for a message that quotes the value that failed:

```csharp
RuleFor(x => x.Email).MustAsync(
    (email, ct) => store.IsUniqueAsync(email, ct),
    email => $"'{email}' is already registered.");
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
