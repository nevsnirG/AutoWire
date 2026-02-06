using AssembleMe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection;

public static class IAssemblerBuilderExtensions
{
    public static IAssemblerBuilder AddMicrosoftDependencyInjectionWiring(this IAssemblerBuilder builder)
    {
        builder.Services.AddTransient<IProcessAssemblies>(sp => new WireMicrosoftDependencyInjectionContainer(builder.Services));
        return builder;
    }
}
