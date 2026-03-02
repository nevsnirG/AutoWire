using AutoWire.MicrosoftDependencyInjection.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection.Test;

public class WireMicrosoftDependencyInjectionContainerTests : IDisposable
{
    public WireMicrosoftDependencyInjectionContainerTests()
    {
        AutoWireTestFixtures.Reset();
    }

    public void Dispose()
    {
        AutoWireTestFixtures.Reset();
    }

    [Fact]
    public void ProcessAssembly_DiscoversAndInvokesPublicAutoWireMethods()
    {
        var services = new ServiceCollection();
        var container = new WireMicrosoftDependencyInjectionContainer(services);

        container.ProcessAssembly(typeof(AutoWireTestFixtures).Assembly);

        AutoWireTestFixtures.PublicMethodCallCount.Should().Be(1);
    }

    [Fact]
    public void ProcessAssembly_DiscoversAndInvokesPrivateAutoWireMethods()
    {
        var services = new ServiceCollection();
        var container = new WireMicrosoftDependencyInjectionContainer(services);

        container.ProcessAssembly(typeof(AutoWireTestFixtures).Assembly);

        AutoWireTestFixtures.PrivateMethodCallCount.Should().Be(1);
    }

    [Fact]
    public void ProcessAssembly_RegistersServicesInCollection()
    {
        var services = new ServiceCollection();
        var container = new WireMicrosoftDependencyInjectionContainer(services);

        container.ProcessAssembly(typeof(AutoWireTestFixtures).Assembly);

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IPublicTestService) &&
            sd.ImplementationType == typeof(PublicTestService));
    }

    [Fact]
    public void ProcessAssembly_RegisteredServicesAreResolvable()
    {
        var services = new ServiceCollection();
        var container = new WireMicrosoftDependencyInjectionContainer(services);

        container.ProcessAssembly(typeof(AutoWireTestFixtures).Assembly);

        var provider = services.BuildServiceProvider();
        provider.GetService<IPublicTestService>().Should().NotBeNull();
    }

    [Fact]
    public void ProcessAssembly_AssemblyWithNoAutoWireMethods_DoesNothing()
    {
        var services = new ServiceCollection();
        var container = new WireMicrosoftDependencyInjectionContainer(services);

        container.ProcessAssembly(typeof(object).Assembly);

        services.Should().BeEmpty();
    }
}
