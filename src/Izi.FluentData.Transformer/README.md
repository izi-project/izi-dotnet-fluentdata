# Izi.FluentData.Transformer

Fluent, **dependency-free** object transformation and normalization for **.NET 10**.

Subclass `Transformer<T>`, declare a pipeline per property, and `TransformAsync` reads each property, runs its pipeline, and writes the result back — in place, on the same instance. The design mirrors [`Izi.FluentData.Validation`](../Izi.FluentData.Validation/README.md): `RuleFor` wires in a nested transformer you chain steps onto, so there is no separate builder type to learn, and the execution path is engineered to stay allocation-free in steady state.

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

public sealed class CustomerTransformer : Transformer<Customer>
{
    public CustomerTransformer()
    {
        RuleFor(x => x.Name).Trim().ToUpper();
        RuleFor(x => x.Email).Trim().ToLower();
        RuleFor(x => x.Total).Round(2).Clamp(0m, 10_000m);
        RuleFor(x => x.SignedUp).ToUniversalTime().StartOfDay();
    }
}

var transformer = new CustomerTransformer();           // build once, reuse forever
Customer normalised = await transformer.TransformAsync(customer);
```

`RuleFor(x => x.Name)` returns a nested `Transformer<string>`; the fluent extensions (`Trim`, `ToUpper`, …) each append a step and return that same nested transformer, so the chain reads top-to-bottom. `TransformAsync` returns the **same** instance it was given, with every configured property rewritten.

> **One-way, same-type pipelines.** Every step maps `T → T`; a pipeline does not change the property's type. That keeps write-back trivial and the whole thing allocation-free. If you need parsing or reshaping (`string → int`), do it in your own step (see [Custom steps](#custom--asynchronous-steps)).

---

## Design & how it works

The library is a small composition of well-known patterns, each chosen to push work to *construction time* and keep *execution time* cheap.

### One interface, uniform composition

Everything is an [`ITransformer<T>`](ITransformer.cs) — a single `ValueTask<T> TransformAsync(T, CancellationToken)`:

- a built-in **rule** ([`TransformerRule<T>`](Rules/TransformerRule.cs)) wrapping one delegate,
- a **nested** `Transformer<TProperty>` produced by `RuleFor`,
- the top-level `Transformer<T>` itself.

Because a whole pipeline is itself an `ITransformer<T>`, steps and pipelines are interchangeable, and a `Transformer<T>` just holds an ordered `List<ITransformer<T>>` that it runs in sequence, threading each step's output into the next.

### `RuleFor` — nested transformer, no builder

`RuleFor(x => x.Property)` compiles the member selector into a **getter** and a matching **setter** once, creates a fresh `Transformer<TProperty>`, wires it in as a step that *reads → runs the nested pipeline → writes back*, and returns it so you can chain steps directly. Pass the identity selector `x => x` to shape the whole instance.

```csharp
RuleFor(x => x.Name)      // Transformer<string>
    .Trim()
    .ToUpper();           // steps chain onto the nested transformer
```

The expensive expression-tree compilation happens per property, in the constructor; `TransformAsync` then only invokes cached delegates.

> **Selector constraints.** The setter is built from the selector, so it must be a **direct, assignable property/field access** (`x => x.Name`) or the identity selector (`x => x`). A read-only property or an arbitrary expression (`x => x.Name.Trim()`) throws `ArgumentException` at construction — before you can run it.

### Correct mutable-struct write-back

The compiled setter is `(instance, value) => { instance.Member = value; return instance; }` — it *returns* the instance rather than assigning through a `void` setter. For a value type that means the block assigns to and returns the local copy, so the new member value survives. Transforming a `struct`'s field works correctly.

### Synchronous fast path

Every step returns `ValueTask<T>`. `TransformAsync` checks `IsCompletedSuccessfully` and, when the step completed synchronously (which **all** built-ins do), reads `.Result` and moves on without ever building an `async` state machine. The asynchronous fallback (`TransformSlowAsync`) is `static` and takes its state as parameters, so no closure is captured; it's reached only when a custom step genuinely yields.

---

## Rules reference

Rules are surfaced two ways:

- **Fluent extensions** on `Transformer<T>` (in [`TransformerExtensions`](TransformerExtensions.cs)) — what you chain after `RuleFor`.
- **Factories** on the static `TransformerRules` class ([string/numeric/default](Rules/TransformerRules.cs), [date/time](Rules/TransformerRules.DateTime.cs)) — the underlying steps, usable standalone.

A step is only offered while the running value has a compatible type.

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

### Numeric (generic math)

Numeric rules are written against .NET's [generic math](https://learn.microsoft.com/dotnet/standard/generics/math) interfaces and constrained to the narrowest interface that exposes the operation, so floating-point-only steps simply aren't offered on integer types.

| Rule | Applies to | Description |
| --- | --- | --- |
| `Add` / `Subtract` / `Multiply` / `Divide(value)` | any number (`INumberBase`) | Arithmetic against a constant |
| `Abs` / `Invert` | any number (`INumberBase`) | Absolute value / negation |
| `Clamp(min, max)` / `Normalize(min, max)` | comparable numbers (`INumber`) | Clamp to a range / map `[min,max]` → `[0,1]` |
| `Round([digits][, mode])` / `Floor` / `Ceiling` / `Truncate` | floating-point (`IFloatingPoint`) | Rounding (banker's by default) |
| `Sqrt` / `Cbrt` / `RootN(n)` / `Pow(exp)` | roots / powers interfaces | Roots and powers |
| `Exp` / `Exp2` / `Exp10` / `Log([base])` / `Log2` / `Log10` | exp / log interfaces | Exponential and logarithmic |
| `Sin`/`Cos`/`Tan`/`Asin`/`Acos`/`Atan`, `DegreesToRadians`/`RadiansToDegrees` | `ITrigonometricFunctions` | Trigonometry (radians) |
| `Sinh`/`Cosh`/`Tanh`/`Asinh`/`Acosh`/`Atanh` | `IHyperbolicFunctions` | Hyperbolic |

### Date & time

Date/time rules map `T → T` over `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, and `TimeSpan` — each rule supporting the subset of those types the operation makes sense for. When used standalone the flowing type is fixed by the caller (e.g. `AddDays<DateTime>(1)`); after `RuleFor` it's already known. A rule applied to a type it doesn't cover throws `NotSupportedException`.

| Group | Rules |
| --- | --- |
| Arithmetic / offset | `AddYears`/`AddMonths`/`AddDays`/`AddHours`/`AddMinutes`/`AddSeconds`/`AddMilliseconds`/`AddTicks`, `Add`/`Subtract(TimeSpan)`, `Negate`, `Duration` |
| Clamp | `Clamp(min, max)` |
| Start / end of period | `StartOfDay`/`EndOfDay`, `StartOfWeek`/`EndOfWeek(firstDay)`, `StartOfMonth`/`EndOfMonth`, `StartOfQuarter`/`EndOfQuarter`, `StartOfYear`/`EndOfYear` |
| Interval rounding | `RoundTo`/`TruncateTo`/`CeilingTo(interval)` |
| Component replacement | `WithYear`/`WithMonth`/`WithDay`/`WithHour`/`WithMinute`/`WithSecond`/`WithMillisecond`, `SetTimeOfDay`, `SetDate` |
| Calendar navigation | `NextDayOfWeek`/`PreviousDayOfWeek`, `AddBusinessDays(count[, holidays])`, `NextBusinessDay`/`PreviousBusinessDay([holidays])` |
| Kind / time zone | `ToUniversalTime`/`ToLocalTime`, `SpecifyKind`, `ToOffset`, `ConvertToTimeZone` |
| Sentinel defaults | `DefaultIfMinValue`/`DefaultIfMaxValue(defaultValue)` for each date/time type |

> End-of-period rules return the **last representable instant** of the period for time-bearing types (start of the next period minus one tick) and the **last day** for `DateOnly`.

### Defaults

| Rule | Description |
| --- | --- |
| `DefaultIf(value, predicate)` | Substitute `value` when the predicate matches |
| `DefaultIfNull(value)` | Substitute when `null` |
| `DefaultIfEmpty(value)` | Substitute when null, empty string, or empty collection/sequence |
| `DefaultIfNullOrWhitespace(value)` | Substitute when a string is null/empty/whitespace |
| `DefaultIfMinValue([threshold,] value)` / `DefaultIfMaxValue([threshold,] value)` | Substitute at the type's `MinValue`/`MaxValue`, or at/below/above a comparable threshold |

---

## Recipes

### Whole-instance / cross-field shaping

Pass the identity selector to receive the whole instance in a step. Combine with a custom step (below) for cross-field work:

```csharp
RuleFor(x => x).AddTransformer(new TransformerRule<Customer>((c, _) =>
{
    c.DisplayName = $"{c.FirstName} {c.LastName}".Trim();
    return ValueTask.FromResult(c);
}));
```

Or append a whole-instance step directly with `AddTransformer(...)` on the transformer itself.

### Custom & asynchronous steps

A step is just a `TransformerRule<T>` wrapping a `(value, token) => ValueTask<T>` delegate — the escape hatch for anything not in the catalogue, including I/O-bound work:

```csharp
RuleFor(x => x.Slug)
    .Trim()
    .ToLower()
    .AddTransformer(new TransformerRule<string>(
        async (value, ct) => await slugifier.NormalizeAsync(value, ct)));
```

This is the only path that legitimately reaches the asynchronous slow path; synchronous steps before and after it still run synchronously.

### Reusing a standalone rule or pipeline

`TransformerRules` factories return plain `ITransformer<T>` steps you can run without a `Transformer<T>`, and a `Transformer<T>` composed of extensions is itself reusable:

```csharp
using static Izi.FluentData.Transformer.Rules.TransformerRules;

string trimmed = await Trim().TransformAsync("  hello  ");          // single rule

var normalise = new Transformer<string>().Trim().ToUpper().Truncate(50);
string result = await normalise.TransformAsync("  hello world  ");  // reusable pipeline
```

---

## Dependency injection

Use the companion package **[Izi.FluentData.Transformer.DependencyInjectionExtensions](https://www.nuget.org/packages/Izi.FluentData.Transformer.DependencyInjectionExtensions)**:

```csharp
services.AddTransformer<CustomerTransformer>();   // singleton (recommended)
```

A transformer compiles its accessors and builds its pipelines once in the constructor and is stateless afterward, so a shared singleton is ideal.

---

## Performance & .NET 10 optimizations

This is a hot-path library; the design is deliberate about what it allocates.

- **Synchronous fast path, no async state machine.** `TransformAsync` checks `IsCompletedSuccessfully` and returns synchronously for every built-in step; the `static` async fallback is reached only when a custom step yields.
- **`this`-free continuations.** The asynchronous fallbacks are `static` and take their state as parameters, so the awaiter captures no closure over the rule instance.
- **Zero-cost numeric abstraction via generic math.** The JIT monomorphizes each closed generic per value type, so `Add(2)` on an `int` compiles to a direct integer add — no boxing, no virtual dispatch.
- **Value-type steps never touch the heap.** Date/time and numeric steps flow structs through `ValueTask<T>` without boxing.
- **Null-coalescing string steps.** Every string rule coalesces `null` → `string.Empty` up front, so downstream steps never branch on null.
- **Work happens once.** Getters, setters, and nested pipelines are all built in the constructor; steady-state transformation just runs them.

### Benchmarks

`BenchmarkDotNet v0.15.8` · `.NET 10.0.8` · Intel Core i7-9700F · `[MemoryDiagnoser]`

| Method | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| `SingleRule` (one `Trim`) | 15.5 ns | 48 B | the lone allocation is the trimmed `string` result |
| `StringPipeline` (`Trim → ToUpper → Truncate`) | 50.3 ns | 96 B | two intermediate strings; a string op that changes the value allocates the new string |
| `DateTimePipeline` (`StartOfMonth → AddDays → WithHour`) | 36.5 ns | **0 B** | struct values flow through `ValueTask` without ever touching the heap |
| `ObjectTransformer` (3 property pipelines) | 88.3 ns | **0 B** | **allocation-free** — the built-in string ops return the same instance once values are already normalised (idempotent steady state) |

The headline results are the two **0 B** rows: a complete object normalisation across three properties runs in ~88 ns and, in steady state, allocates **nothing**; a three-step date/time pipeline is allocation-free by construction because it flows value types.

> Reproduce locally:
> ```bash
> dotnet run -c Release --project benchmark/Izi.FluentData.Transformer.Benchmarks -- --filter *
> ```

---

## Links

- Repository & full documentation: <https://github.com/izi-project/izi-dotnet-fluentdata>
- License: MIT
