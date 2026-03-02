# AutoWire

Attribute-based automatic service registration for Microsoft's dependency injection container. Built on the [AssembleMe](https://github.com/nevsnirG/AssembleMe) framework.

AutoWire scans your assemblies at startup, finds methods marked with `[AutoWire]`, and invokes them to register services — no more manually calling dozens of `Add*` extension methods from your `Program.cs`.

## Packages

| Package | Description |
|---|---|
| [AutoWire.Contract](https://www.nuget.org/packages/AutoWire.Contract) | The `[AutoWire]` attribute |
| [AutoWire.MicrosoftDependencyInjection](https://www.nuget.org/packages/AutoWire.MicrosoftDependencyInjection) | Microsoft DI integration |
| [AutoWire.Analyzer](https://www.nuget.org/packages/AutoWire.Analyzer) | Roslyn analyzer for compile-time validation |

## Getting Started

Projects that define service registrations only need to reference **AutoWire.Contract** (and optionally the **Analyzer** for compile-time checks). Only the host project that bootstraps the DI container needs to reference **AutoWire.MicrosoftDependencyInjection**.

```
# In each library/domain project:
dotnet add package AutoWire.Contract
dotnet add package AutoWire.Analyzer

# In your host/startup project:
dotnet add package AutoWire.MicrosoftDependencyInjection
```

## Usage

### 1. Define service registrations anywhere in your solution

Create static methods that accept an `IServiceCollection` and mark them with the `[AutoWire]` attribute:

```csharp
using AutoWire;
using Microsoft.Extensions.DependencyInjection;

public static class OrderServices
{
    [AutoWire]
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
    }
}
```

Methods can be `public` or `private`, and can live in any class, in any assembly that gets discovered by AssembleMe. This way each project owns its own registrations, and the host doesn't need to know about them.

### 2. Wire everything up at startup

In your host project:

```csharp
serviceCollection.AddAssembler(b => b.AddMicrosoftDependencyInjectionWiring());
```

That's it. AutoWire will discover and invoke all `[AutoWire]`-attributed methods across your assemblies.

## Analyzer

The `AutoWire.Analyzer` package provides compile-time validation for `[AutoWire]` methods. It reports a warning (`AW0001`) if a decorated method does not have the correct signature (a single `IServiceCollection` parameter).

## Method Requirements

Methods decorated with `[AutoWire]` must:

- Be **static**
- Have exactly **one parameter** of type `IServiceCollection`

Violations are caught both at compile-time (via the analyzer) and at runtime.

## License

[MIT](https://opensource.org/licenses/MIT)
