# Izi.FluentData.Transformer.DependencyInjectionExtensions

`Microsoft.Extensions.DependencyInjection` registration helpers for **[Izi.FluentData.Transformer](https://www.nuget.org/packages/Izi.FluentData.Transformer)**. Kept in a separate package so the core library stays **dependency-free** — reference this only when you want container wiring.

```bash
dotnet add package Izi.FluentData.Transformer.DependencyInjectionExtensions
```

- **Target framework:** `net10.0`
- **Dependencies:** `Microsoft.Extensions.DependencyInjection.Abstractions` (+ the core `Izi.FluentData.Transformer`)
- **Namespace:** helpers live in `Microsoft.Extensions.DependencyInjection`, so they surface wherever `IServiceCollection` is in scope.

---

## Usage

```csharp
using Microsoft.Extensions.DependencyInjection;

// register a transformer type (singleton by default)
services.AddTransformer<CustomerTransformer>();
services.AddTransformer<CustomerTransformer>(ServiceLifetime.Scoped);

// register a pre-built instance or a factory
services.AddTransformer<Customer>(new CustomerTransformer());
services.AddTransformer<Customer>(sp => new CustomerTransformer(/* deps */));

// scan one or more assemblies for every ITransformer<T> implementation
services.AddTransformers(ServiceLifetime.Singleton, typeof(CustomerTransformer).Assembly);
```

Then inject the interface wherever you need it:

```csharp
public sealed class CustomerService(ITransformer<Customer> transformer)
{
    public ValueTask<Customer> NormaliseAsync(Customer customer)
        => transformer.TransformAsync(customer);
}
```

---

## How registration works

`AddTransformer<T>()` registers the **concrete type once**, then forwards **every** `ITransformer<T>` interface the type implements to that single registration:

```
services.AddTransformer<CustomerTransformer>();

   CustomerTransformer            ──▶  (one registration, the chosen lifetime)
   ITransformer<Customer>         ──▶  sp => sp.GetRequiredService<CustomerTransformer>()
```

Two consequences worth knowing:

- **Resolving the interface and the concrete type returns the same instance.** A singleton is therefore never built twice, even though it is registered under two service types.
- **One type can serve several `ITransformer<T>`.** If a class implements `ITransformer<A>` and `ITransformer<B>`, both interfaces resolve to the same shared instance.

`AddTransformers(...)` applies the same logic to every non-abstract, non-generic `ITransformer<T>` implementation it discovers in the supplied assemblies. Passing a type that does not implement `ITransformer<T>` throws `ArgumentException`.

---

## Which lifetime?

**Singleton (the default) is recommended.** A transformer compiles its property getters/setters and builds its pipelines once in the constructor and is stateless and thread-safe afterward, so a single shared instance avoids repeating that setup and is safe under concurrency.

Drop to **Scoped** or **Transient** only when the transformer itself depends on a shorter-lived service (e.g. a `DbContext`). Match its lifetime to its **shortest-lived dependency** to avoid a captive-dependency bug.

---

## Links

- Repository & full documentation: <https://github.com/izi-project/izi-dotnet-fluentdata>
- License: MIT
