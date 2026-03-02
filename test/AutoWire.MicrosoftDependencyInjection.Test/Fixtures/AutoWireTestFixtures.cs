using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.MicrosoftDependencyInjection.Test.Fixtures;

public interface IPublicTestService;

internal interface IPrivateTestService;

public static class AutoWireTestFixtures
{
    public static int PublicMethodCallCount { get; private set; }
    public static int PrivateMethodCallCount { get; private set; }

    public static void Reset()
    {
        PublicMethodCallCount = 0;
        PrivateMethodCallCount = 0;
    }

    [AutoWire]
    public static void RegisterPublicServices(IServiceCollection services)
    {
        PublicMethodCallCount++;
        services.AddSingleton<IPublicTestService, PublicTestService>();
    }

#pragma warning disable IDE0051
    [AutoWire]
    private static void RegisterPrivateServices(IServiceCollection services)
    {
        PrivateMethodCallCount++;
        services.AddSingleton<IPrivateTestService, PrivateTestService>();
    }
#pragma warning restore
}

internal class PublicTestService : IPublicTestService;

internal class PrivateTestService : IPrivateTestService;
