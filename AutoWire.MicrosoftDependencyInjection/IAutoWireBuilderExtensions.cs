using AutoWire.AssemblyScanner;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection;
public static class IAutoWireBuilderExtensions
{
    public static IAutoWireBuilder AddMicrosoftDependencyInjectionWiring(this IAutoWireBuilder builder) =>
        AddMicrosoftDependencyInjectionWiring(builder, ScannerOptions.Default);

    public static IAutoWireBuilder AddMicrosoftDependencyInjectionWiring(this IAutoWireBuilder builder, ScannerOptions scannerOptions)
    {
        builder.Services
            .AddTransient(sp => new Scanner(scannerOptions, sp.GetServices<IWireContainer>()))
            .AddTransient<IWireContainer>(sp => ActivatorUtilities.CreateInstance<WireMicrosoftDependencyInjectionContainer>(sp, builder.Services));

        var serviceProvider = builder.Services.BuildServiceProvider();
        var scanner = serviceProvider.GetRequiredService<Scanner>();

        scanner.Scan();

        return builder;
    }
}
