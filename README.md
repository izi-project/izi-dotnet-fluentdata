# Izi.FluentData

A small, allocation-conscious family of **.NET 10** libraries for **validating** and **transforming** objects through a fluent, FluentValidation-style API.

The ecosystem is built around two independent core libraries — use one, the other, or both. Each core library is **dependency-free**; the optional dependency-injection helpers live in separate packages so nothing third-party leaks into your domain unless you opt in.

> **Validate** → declare rules per property, get back a list of error messages.
> **Transform** → declare a pipeline per property, get back the normalised object.

---

## Why it exists

Most apps need the same two things at their boundaries: **normalise** incoming data (trim, case, parse, clamp) and **validate** it (required, length, range, format). Izi.FluentData gives both a single, declarative, strongly-typed style — without dragging in a validation framework, a mapper, or a regex-compilation tax — and keeps the hot paths allocation-free.

- **Fluent & declarative** — describe *what* is valid / *how* to normalise, not the control flow.
- **Strongly typed** — type-changing transformation pipelines are checked by the compiler.
- **Dependency-free core** — reference a core package and you pull in **zero** transitive dependencies.
- **Allocation-conscious** — synchronous fast paths, shared empty results, and source-generated regex keep steady-state allocations at or near zero.
- **`async`-ready** — every entry point is `ValueTask`-based, with a synchronous fast path that avoids the async state machine when no step actually yields.

---

## The ecosystem at a glance

| Package | Responsibility | Third-party deps |
| --- | --- | --- |
| [`Izi.FluentData.Validation`](src/Izi.FluentData.Validation/README.md) | Core validation engine | **none** |
| [`Izi.FluentData.Transformer`](src/Izi.FluentData.Transformer/README.md) | Core transformation engine | **none** |
| [`Izi.FluentData.Validation.DependencyInjectionExtensions`](src/Izi.FluentData.Validation.DependencyInjectionExtensions/README.md) | `AddValidator(...)` helpers for `IServiceCollection` | `Microsoft.Extensions.DependencyInjection.Abstractions` |
| [`Izi.FluentData.Transformer.DependencyInjectionExtensions`](src/Izi.FluentData.Transformer.DependencyInjectionExtensions/README.md) | `AddTransformer(...)` helpers for `IServiceCollection` | `Microsoft.Extensions.DependencyInjection.Abstractions` |

**Requirements:** .NET 10 (`net10.0`).

---

## Architecture

The two domains are symmetric. Each exposes a base class you subclass, an interface you depend on, a fluent builder, and an internal rule engine that composes small, single-purpose steps.

```
                         ┌──────────────────────────────────────────────┐
                         │                Your application              │
                         └───────────────┬──────────────┬───────────────┘
                       depends on (DI)   │              │   depends on (DI)
                                ┌─────────▼───┐      ┌───▼─────────┐
                                │ IValidator<T>│      │ITransformer<T>│
                                └─────────┬───┘      └───┬─────────┘
                                          │ implements   │ implements
                                ┌─────────▼───┐      ┌───▼─────────┐
                                │ Validator<T> │      │ Transformer<T>│
                                └─────────┬───┘      └───┬─────────┘
                                          │ builds       │ builds
                       CompositeValidatorRule<T>   CompositeTransformerRule<TSource,TDest>
                                          │              │
                                  ValidatorRule<T>   TransformerRule<TSource,TDest>
                                   (predicate)         (transform step)
```

### Repository structure

```
izi-dotnet-fluentdata/
├─ src/
│  ├─ Izi.FluentData.Validation/                              core validation engine
│  ├─ Izi.FluentData.Validation.DependencyInjectionExtensions/   AddValidator(...)
│  ├─ Izi.FluentData.Transformer/                             core transformation engine
│  └─ Izi.FluentData.Transformer.DependencyInjectionExtensions/  AddTransformer(...)
├─ test/
│  ├─ Izi.FluentData.Validation.Tests/                       xUnit test suite
│  └─ Izi.FluentData.Transformer.Tests/
├─ benchmark/
│  ├─ Izi.FluentData.Validation.Benchmarks/                  BenchmarkDotNet
│  └─ Izi.FluentData.Transformer.Benchmarks/
└─ README.md                                                 ← you are here
```

### Pipeline flow

**Validation** runs every rule for a value and aggregates the failures:

```
instance ──selector──▶ value ──▶ rule₁ ─┐
                                 rule₂ ─┼──▶ aggregate ──▶ IReadOnlyList<string>
                                 ruleₙ ─┘                  (empty ⇒ valid)
```

**Transformation** reads a property, threads it through a composed pipeline, and writes it back:

```
instance ──getter──▶ value ──▶ step₁ ──▶ step₂ ──▶ … ──▶ stepₙ ──setter──▶ instance
                              └────────── CompositeTransformerRule ─────────┘
```

### How the pieces interconnect

- **Core libraries are standalone.** `Validation` and `Transformer` share no code and can ship independently.
- **DI packages are thin adapters.** Each `*.DependencyInjectionExtensions` package references its core library plus `Microsoft.Extensions.DependencyInjection.Abstractions`, and adds `IServiceCollection` registration helpers in the `Microsoft.Extensions.DependencyInjection` namespace — nothing more.
- **A common composition pattern.** In a typical request you transform first, then validate the normalised result:

  ```csharp
  customer = await transformer.TransformAsync(customer);
  IReadOnlyList<string> errors = await validator.ValidateAsync(customer);
  ```

---

## Quick start

Install whichever cores you need (each is independent):

```bash
dotnet add package Izi.FluentData.Validation
dotnet add package Izi.FluentData.Transformer
```

Declare a validator and a transformer, then run them:

```csharp
using Izi.FluentData.Validation;
using Izi.FluentData.Transformer;

public sealed class CustomerValidator : Validator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaxLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Age).InRange(18, 120);
    }
}

public sealed class CustomerTransformer : Transformer<Customer>
{
    public CustomerTransformer()
    {
        RuleFor(x => x.Name,  b => b.Trim().ToUpper());
        RuleFor(x => x.Email, b => b.Trim().ToLower());
        RuleFor(x => x.Total, b => b.Round(2).Clamp(0m, 10_000m));
    }
}

var transformer = new CustomerTransformer();
var validator   = new CustomerValidator();

customer = await transformer.TransformAsync(customer);
IReadOnlyList<string> errors = await validator.ValidateAsync(customer);
```

For the full rule/transform catalogues, design notes, and DI wiring, see each library's README linked in the [table above](#the-ecosystem-at-a-glance).

---

## Build & test the solution

```bash
# restore + build every project
dotnet build

# run both test suites
dotnet test

# run the benchmarks (Release is required for meaningful numbers)
dotnet run -c Release --project benchmark/Izi.FluentData.Validation.Benchmarks  -- --filter *
dotnet run -c Release --project benchmark/Izi.FluentData.Transformer.Benchmarks -- --filter *
```

There is no solution file checked in; `dotnet build` from the repository root discovers and builds every project. To work on a single area, point the CLI at its project file, e.g. `dotnet test test/Izi.FluentData.Validation.Tests`.

---

## License

MIT.
