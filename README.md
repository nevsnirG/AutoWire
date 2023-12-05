# AutoWire
Framework for automatically wiring dependencies to dependency containers via assembly discovery. Built on the AssembleMe framework.

## Usage
```csharp
serviceCollection.AddAssembler(b => b.AddMicrosoftDependencyInjectionWiring());
```