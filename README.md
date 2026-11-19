# Izi.FluentData

Libraries for normalising objects in .NET 8 and .NET 10. Version 2.0 of the transformer is a single `Transformer<T>`: it changes properties of `T` and returns the updated value.

Validation is not in this branch yet. `src/Izi.FluentData.Validation` is reserved for it.

## Install

```bash
dotnet add package Izi.FluentData.Transformer --version 2.0.0
```

The package targets `net8.0` and `net10.0`. It has no dependency-injection dependency. Register `ITransformer<T>` yourself if you use a container.

## Quick start

`Transformer<T>` is sealed. Configure an instance, then call `TransformAsync`.

```csharp
using Izi.FluentData.Transformer;

var customer = new Customer { Name = "  ada  ", Email = "  Ada@Example.COM ", Total = 1.239m };
var transformer = new Transformer<Customer>();
transformer.RuleFor(customer => customer.Name).Trim().ToUpper();
transformer.RuleFor(customer => customer.Email).Trim().ToLower();
transformer.RuleFor(customer => customer.Total).Round(2);

customer = await transformer.TransformAsync(customer);

sealed class Customer
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public decimal Total { get; set; }
}
```

`Trim`, `ToUpper`, `ToLower`, and `Round` are fluent wrappers over the catalogue in `TransformerRules`. Every step reads and writes the same property type, so the chain stays on that property.

Conversions are not part of Transformer 2.0. Parse a string, map one property onto another, or change a numeric type in your own code (or in model binding) before you call `TransformAsync`.

## Custom rules

`Add` is still the public hook for a rule the catalogue does not have. The delegate receives the property value and the cancellation token.

```csharp
using Izi.FluentData.Transformer;

var transformer = new Transformer<Customer>();
transformer.RuleFor(customer => customer.Name).Add(static (name, token) =>
{
    token.ThrowIfCancellationRequested();
    return new ValueTask<string>(name.Trim());
});

sealed class Customer
{
    public string Name { get; set; } = "";
}
```

`Add` also accepts an `ITransformerRule<TProperty>`, including any `TransformerRules` factory.

## Structs and records

`TransformAsync` returns the updated value. A class with ordinary setters is updated in place and that same instance is returned. A struct is not written back to the caller's variable. A record, or any other init-only property, is cloned and the original is left unchanged. Reassign the result:

```csharp
using Izi.FluentData.Transformer;

var points = new Transformer<Point>();
points.RuleFor(point => point.X).Add(1);
var point = await points.TransformAsync(new Point { X = 1, Y = 5 });

var people = new Transformer<Person>();
people.RuleFor(person => person.Name).Trim();
var person = await people.TransformAsync(new Person("  ada  ", 4));

struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

sealed record Person(string Name, int Age);
```

## ITransformer&lt;T&gt;

`ITransformer<T>` exists so the pipeline can be injected or mocked. This library does not ship a container integration.

```csharp
using Izi.FluentData.Transformer;

var configured = new Transformer<Customer>();
configured.RuleFor(customer => customer.Name).Trim().ToUpper();

ITransformer<Customer> transformer = configured;
var customer = await transformer.TransformAsync(new Customer { Name = "  ada  " });

sealed class Customer
{
    public string Name { get; set; } = "";
}
```

A mock implements the same method:

```csharp
using Izi.FluentData.Transformer;

ITransformer<Customer> transformer = new Echo();
var customer = await transformer.TransformAsync(new Customer());

sealed class Echo : ITransformer<Customer>
{
    public ValueTask<Customer> TransformAsync(Customer value, CancellationToken cancellationToken = default)
        => new(value);
}

sealed class Customer
{
    public string Name { get; set; } = "";
}
```

## Cancellation

The token is checked before the run and before each property step. Pass it through:

```csharp
using Izi.FluentData.Transformer;

var transformer = new Transformer<Customer>();
using var cancellation = new CancellationTokenSource();
var customer = await transformer.TransformAsync(new Customer(), cancellation.Token);

sealed class Customer
{
    public string Name { get; set; } = "";
}
```

A rule that waits should watch the same token. Synchronous rules finish on the calling thread and do not allocate an async state machine.

## Benchmarks

The benchmark project is `benchmark/Izi.FluentData.Transformer.PipelineBenchmarks`. Release mode is required. It checks the scenario results, then runs BenchmarkDotNet.

```bash
dotnet run -c Release -f net10.0 --project benchmark/Izi.FluentData.Transformer.PipelineBenchmarks -- --filter '*'
```

A short run, useful while changing the library:

```bash
dotnet run -c Release -f net10.0 --project benchmark/Izi.FluentData.Transformer.PipelineBenchmarks -- --job short --filter '*'
```

Use `-f net8.0` for the other target. The scenarios cover an empty transformer, a synchronous class, the same rules on a struct and on an init-only record, one yielding step, the fluent wrappers against the same rules added with `Add`, and direct `Trim` and `Round` calls.

## Build and test

```bash
dotnet build izi-dotnet-fluentdata.slnx
dotnet test test/Izi.FluentData.Transformer.Tests/Izi.FluentData.Transformer.Tests.csproj -f net8.0
dotnet test test/Izi.FluentData.Transformer.Tests/Izi.FluentData.Transformer.Tests.csproj -f net10.0
```

## Validation

The 2.0 validator is not implemented in this branch. Do not use the 1.x `Validator<T>` samples against this tree. They will return when `src/Izi.FluentData.Validation` has an API.

## License

MIT.
