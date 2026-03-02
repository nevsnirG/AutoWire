using AssembleMe.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AutoWire.MicrosoftDependencyInjection.Test;

public class IAssemblerBuilderExtensionsTests
{
    [Fact]
    public void AddMicrosoftDependencyInjectionWiring_RegistersProcessAssembliesService()
    {
        var services = new ServiceCollection();
        var builder = Substitute.For<IAssemblerBuilder>();
        builder.Services.Returns(services);

        builder.AddMicrosoftDependencyInjectionWiring();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IProcessAssemblies) &&
            sd.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddMicrosoftDependencyInjectionWiring_ReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = Substitute.For<IAssemblerBuilder>();
        builder.Services.Returns(services);

        var result = builder.AddMicrosoftDependencyInjectionWiring();

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AddMicrosoftDependencyInjectionWiring_RegisteredFactory_ResolvesToWireMicrosoftDependencyInjectionContainer()
    {
        var services = new ServiceCollection();
        var builder = Substitute.For<IAssemblerBuilder>();
        builder.Services.Returns(services);

        builder.AddMicrosoftDependencyInjectionWiring();

        var provider = services.BuildServiceProvider();
        var processor = provider.GetRequiredService<IProcessAssemblies>();
        processor.Should().BeOfType<WireMicrosoftDependencyInjectionContainer>();
    }
}
