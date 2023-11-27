using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddAutoWire(this IServiceCollection services, Action<IAutoWireBuilder> configure)
    {
        configure.Invoke(new AutoWireBuilder(services));
        return services;
    }
}
