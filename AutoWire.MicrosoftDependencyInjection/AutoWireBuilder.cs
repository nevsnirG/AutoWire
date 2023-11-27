using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection;

public interface IAutoWireBuilder
{
     IServiceCollection Services { get; }
}

internal sealed class AutoWireBuilder : IAutoWireBuilder
{
    public IServiceCollection Services { get; }

    public AutoWireBuilder(IServiceCollection services)
    {
        Services = services;
    }
}