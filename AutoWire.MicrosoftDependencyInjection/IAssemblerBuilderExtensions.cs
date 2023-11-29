using AssembleMe;
using AssembleMe.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection;
public static class IAssemblerBuilderExtensions
{
    public static IAssemblerBuilder AddMicrosoftDependencyInjectionWiring(this IAssemblerBuilder builder)
    {
        builder.Services.AddTransient<IProcessAssemblies, WireMicrosoftDependencyInjectionContainer>();
        return builder;
    }
}
