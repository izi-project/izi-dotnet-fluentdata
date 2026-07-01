# Izi.FluentData.Validation.DependencyInjectionExtensions

`Microsoft.Extensions.DependencyInjection` registration helpers for **[Izi.FluentData.Validation](https://www.nuget.org/packages/Izi.FluentData.Validation)**. Kept in a separate package so the core library stays **dependency-free** — reference this only when you want container wiring.

```bash
dotnet add package Izi.FluentData.Validation.DependencyInjectionExtensions
```

- **Target framework:** `net10.0`
- **Dependencies:** `Microsoft.Extensions.DependencyInjection.Abstractions` (+ the core `Izi.FluentData.Validation`)
- **Namespace:** helpers live in `Microsoft.Extensions.DependencyInjection`, so they surface wherever `IServiceCollection` is in scope.

---

## Usage

```csharp
using Microsoft.Extensions.DependencyInjection;

// register a validator type (singleton by default)
services.AddValidator<CustomerValidator>();
services.AddValidator<CustomerValidator>(ServiceLifetime.Scoped);

// register a pre-built instance or a factory
services.AddValidator<Customer>(new CustomerValidator());
services.AddValidator<Customer>(sp => new CustomerValidator(/* deps */));

// scan one or more assemblies for every IValidator<T> implementation
services.AddValidators(ServiceLifetime.Singleton, typeof(CustomerValidator).Assembly);
```

Then inject the interface wherever you need it:

```csharp
public sealed class CustomerService(IValidator<Customer> validator)
{
    public ValueTask<IReadOnlyList<string>> CheckAsync(Customer customer)
        => validator.ValidateAsync(customer);
}
```

---

## How registration works

`AddValidator<T>()` registers the **concrete type once**, then forwards **every** `IValidator<T>` interface the type implements to that single registration:

```
services.AddValidator<CustomerValidator>();

   CustomerValidator           ──▶  (one registration, the chosen lifetime)
   IValidator<Customer>        ──▶  sp => sp.GetRequiredService<CustomerValidator>()
```

Two consequences worth knowing:

- **Resolving the interface and the concrete type returns the same instance.** A singleton is therefore never built twice, even though it is registered under two service types.
- **One type can serve several `IValidator<T>`.** If a class implements `IValidator<A>` and `IValidator<B>`, both interfaces resolve to the same shared instance.

`AddValidators(...)` applies the same logic to every non-abstract, non-generic `IValidator<T>` implementation it discovers in the supplied assemblies. Passing a type that does not implement `IValidator<T>` throws `ArgumentException`.

---

## Which lifetime?

**Singleton (the default) is recommended.** A validator builds its rule set once in the constructor and is stateless and thread-safe afterward, so a single shared instance avoids repeating that setup and is safe under concurrency.

Drop to **Scoped** or **Transient** only when the validator itself depends on a shorter-lived service (e.g. a `DbContext`). Match its lifetime to its **shortest-lived dependency** to avoid a captive-dependency bug.

---

## Links

- Repository & full documentation: <https://github.com/izi-project/izi-dotnet-fluentdata>
- License: MIT
