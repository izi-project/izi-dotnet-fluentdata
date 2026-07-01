# Izi.FluentData.Transformer

Fluent, **dependency-free** object transformation and normalization for **.NET 10**.

Subclass `Transformer<T>`, declare a pipeline per property, and `TransformAsync` reads each property, runs its pipeline, and writes the result back — in place, on the same instance. Pipelines are **type-changing** and **compile-time-checked**, and the execution path is engineered to stay allocation-free in steady state.

```bash
dotnet add package Izi.FluentData.Transformer
```

- **Target framework:** `net10.0`
- **Dependencies:** none (zero transitive packages)
- **Thread-safety:** a built transformer is immutable and safe to share as a singleton

---

## Quick start

```csharp
using Izi.FluentData.Transformer;
using Izi.FluentData.Transformer.Rules;

public sealed class CustomerTransformer : Transformer<Customer>
{
    public CustomerTransformer()
    {
        RuleFor(x => x.Name,  b => b.Trim().ToUpper());
        RuleFor(x => x.Email, b => b.Trim().ToLower());
        RuleFor(x => x.Total, b => b.Round(2).Clamp(0m, 10_000m));
    }
}

var transformer = new CustomerTransformer();           // build once, reuse forever
Customer normalised = await transformer.TransformAsync(customer);
```

`TransformAsync` returns the **same** instance it was given, with every configured property rewritten.

---

## Design & technical specifications

The library is a small composition of well-known patterns, each chosen to push work to *construction time* and keep *execution time* cheap.

### Type-state fluent Builder

`RuleFor(selector, configure)` hands your callback a `TransformerRuleBuilder<TProperty>`. The builder is **immutable (persistent)**: every step returns a *new* builder whose generic parameter is re-typed to the step's output.

```csharp
TransformerRuleBuilder<TSource, TCurrent>      // TCurrent = the running type so far
    .AddRule<TNext>(rule) -> TransformerRuleBuilder<TSource, TNext>
```

That `TCurrent` parameter is a **phantom type** that tracks the running value's type *at compile time*. Because a property pipeline must `Build()` back to the property's own type, an illegal pipeline simply **does not compile**:

```csharp
// string property … ends on int → compiler error, caught before you run it
RuleFor(x => x.Code, b => b.Trim().StringToInt32());     // ❌ won't compile
RuleFor(x => x.Code, b => b.Trim().StringToInt32().Clamp(0, 100).Int32ToString()); // ✅
```

### Pipeline as Composite

Steps are composed by `CompositeTransformerRule<TSource, TDestination>.Then(...)`, which threads each step's output into the next. The composed pipeline is itself a `TransformerRule`, so a whole pipeline and a single step are interchangeable (the Composite pattern).

### Strategy via delegates

Every step (`TransformerRule<TSource, TDestination>`) wraps a single `Func<TSource, CancellationToken, ValueTask<TDestination>>`. The built-in catalogue on the static `Rules` class is just a library of these strategies.

### Compiled property accessors

In the constructor, `RuleFor` compiles the member selector into a **getter** (`selector.Compile()`) and a matching **setter** (`Expression.Assign` → `Expression.Lambda<Action<T, TProperty>>().Compile()`). The expensive expression-tree work happens once, per property, at construction; `TransformAsync` then only invokes cached delegates.

> **Constraints.** Write-back targets a mutable reference type, and the selector must be a **direct property/field access** (`x => x.Name`). Anything else throws `ArgumentException`. Use the whole-instance overload `RuleFor(b => …)` (identity selector) for read-only/cross-field shaping.

---

## Performance & .NET 10 optimizations

This is a hot-path library; the design is deliberate about what it allocates.

- **Synchronous fast path, no async state machine.** Every rule returns `ValueTask<T>`. `TransformAsync` checks `IsCompletedSuccessfully` and, when the step completed synchronously (which **all** built-ins do), returns the result without ever building an `async` state machine. The slow path is only reached when a custom `Convert(async …)` step genuinely yields.
- **`this`-free continuations.** The asynchronous fallbacks (`TransformSlowAsync`) are `static` and take their state as parameters, so the awaiter captures no closure over the rule instance.
- **Zero-cost numeric abstraction via generic math.** Numeric steps are written against .NET's [generic math](https://learn.microsoft.com/dotnet/standard/generics/math) interfaces (`INumberBase<T>`, `IFloatingPoint<T>`, …). The JIT **monomorphizes** each closed generic per value type, so `Add(2)` on an `int` compiles to a direct integer add — **no boxing, no virtual dispatch**.
- **Null-coalescing string steps.** Every string rule coalesces `null` → `string.Empty` up front, so downstream steps never branch on null.
- **Work happens once.** Getters, setters, and the composed pipeline delegate are all built in the constructor; steady-state transformation just runs them.

### Benchmarks

`BenchmarkDotNet v0.15.8` · `.NET 10.0.7` · 13th Gen Intel Core i7-13700K · `[MemoryDiagnoser]`

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `SingleRule` (one `Trim`) | 8.59 ns | 48 B | the lone allocation is the boxed result of the single `ValueTask<string>` |
| `FreestandingPipeline` (`Trim → ToUpper → Truncate`) | 39.53 ns | 96 B | composed string pipeline |
| `ObjectTransformer` (3 property pipelines) | 105.49 ns | **0 B** | **fully allocation-free** end-to-end transform |

The headline result is `ObjectTransformer`: a complete object normalisation across three properties — string, string, and `decimal` pipelines — runs in ~105 ns and allocates **nothing**.

> Reproduce locally:
> ```bash
> dotnet run -c Release --project benchmark/Izi.FluentData.Transformer.Benchmarks -- --filter *
> ```

---

## Rules reference

Every step is produced by a factory on the static `Rules` class and surfaced as a fluent builder method. A step is only offered while the running value has a compatible type.

### String (`string → string`)

`null` is coalesced to an empty string before every string step, so later steps never have to null-check.

| Rule | Description |
| --- | --- |
| `Trim` / `TrimStart` / `TrimEnd` | Remove surrounding / leading / trailing whitespace |
| `ToUpper([culture])` / `ToLower([culture])` | Change case (invariant culture by default) |
| `ToTitleCase([culture])` | Title-case the value |
| `Replace(oldValue, newValue)` | Replace every occurrence |
| `Substring(start[, length])` | Extract a substring |
| `Truncate(maxLength)` | Cap at `maxLength` characters |
| `PadLeft` / `PadRight(totalWidth, paddingChar)` | Pad to a fixed width |
| `Prepend(prefix)` / `Append(suffix)` | Concatenate |

### Numeric

Numeric rules use generic math; each is constrained to the narrowest interface that exposes the operation, so floating-point-only steps simply aren't offered on integer types.

| Rule | Applies to | Description |
| --- | --- | --- |
| `Add` / `Subtract` / `Multiply` / `Divide(value)` | any number (`INumberBase`) | Arithmetic against a constant |
| `Abs` / `Invert` | any number (`INumberBase`) | Absolute value / negation |
| `Clamp(min, max)` / `Normalize(min, max)` | comparable numbers (`INumber`) | Clamp to a range / map `[min,max]` → `[0,1]` |
| `Sign` | comparable numbers (`INumber`) | Yields `-1`/`0`/`+1` — **changes the running type to `int`** |
| `Round([digits][, mode])` / `Floor` / `Ceiling` / `Truncate` | floating-point (`IFloatingPoint`) | Rounding |
| `Sqrt` / `Cbrt` / `RootN(n)` / `Pow(exp)` | roots/powers interfaces | Roots and powers |
| `Exp` / `Exp2` / `Exp10` / `Log([base])` / `Log2` / `Log10` | exp/log interfaces | Exponential and logarithmic |
| `Sin`/`Cos`/`Tan`/`Asin`/`Acos`/`Atan`, `DegreesToRadians`/`RadiansToDegrees` | `ITrigonometricFunctions` | Trigonometry (radians) |
| `Sinh`/`Cosh`/`Tanh`/`Asinh`/`Acosh`/`Atanh` | `IHyperbolicFunctions` | Hyperbolic |

### Defaults

| Rule | Description |
| --- | --- |
| `DefaultIf(value, predicate)` | Substitute `value` when the predicate matches |
| `DefaultIfNull(value)` | Substitute when `null` |
| `DefaultIfEmpty(value)` | Substitute when null, empty string, or empty collection/sequence |
| `DefaultIfNullOrWhitespace(value)` | Substitute when a string is null/empty/whitespace |

### Conversions

The full primitive conversion matrix between `bool`, the integral/floating-point types, `char`, `DateTime`, and `string` is exposed as `{Source}To{Target}` methods (e.g. `StringToInt32`, `Int32ToString`, `DoubleToDecimal`). Each delegates to `System.Convert`; string-facing conversions use the invariant culture. Pairs `System.Convert` can't perform (e.g. `char`↔`bool`, numeric↔`DateTime`) compile but throw `InvalidCastException` at runtime.

---

## Recipes

### Type-changing pipeline (parse, clamp, format back)

```csharp
// string → trim → int → clamp → string
RuleFor(x => x.Code, b => b.Trim().StringToInt32().Clamp(0, 100).Int32ToString());
```

### Custom & asynchronous conversions

For anything outside the matrix — including I/O-bound work — use the generic `Convert(...)`. The running type becomes whatever the converter returns, so end back at the property's type:

```csharp
RuleFor(x => x.Slug, b => b
    .Trim()
    .ToLower()
    .Convert(async (value, ct) => await slugifier.NormalizeAsync(value, ct)));
```

This is the only path that can legitimately hit the asynchronous slow path; synchronous steps before and after it still run synchronously.

### Whole-instance shaping

```csharp
// No property selector: receive the instance, return a (re)shaped instance.
RuleFor(b => b.Convert((customer, _) =>
    new ValueTask<Customer>(customer with { Tier = Derive(customer) })));
```

### Reusing a standalone pipeline

A pipeline is just a `TransformerRule`; you can build and run one without a `Transformer<T>`:

```csharp
using static Izi.FluentData.Transformer.Rules.Rules;

TransformerRule<string, string> normalise =
    new CompositeTransformerRule<string, string>(Trim())
        .Then(ToUpper())
        .Then(Truncate(50));

string result = await normalise.TransformAsync("  hello world  ");
```

---

## Dependency injection

Use the companion package **[Izi.FluentData.Transformer.DependencyInjectionExtensions](https://www.nuget.org/packages/Izi.FluentData.Transformer.DependencyInjectionExtensions)**:

```csharp
services.AddTransformer<CustomerTransformer>();   // singleton (recommended)
```

A transformer compiles its accessors and builds its pipelines once in the constructor and is stateless afterward, so a shared singleton is ideal.

---

## Links

- Repository & full documentation: <https://github.com/izi-project/izi-dotnet-fluentdata>
- License: MIT
