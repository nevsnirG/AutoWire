using AutoWire.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.Loader;

namespace AutoWire.MicrosoftDependencyInjection;
public static class IAutoWireBuilderExtensions
{
    public static IAutoWireBuilder AddMicrosoftDependencyInjectionWiring(this IAutoWireBuilder builder) =>
        AddMicrosoftDependencyInjectionWiring(builder, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
    public static IAutoWireBuilder AddMicrosoftDependencyInjectionWiring(this IAutoWireBuilder builder, string pathToScan)
    {
        LoadAssembliesFromFileSystem(pathToScan);

        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        var classes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass);

        var methods = classes
            .SelectMany(c => c.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => m.CustomAttributes.Any(ca => ca.AttributeType == typeof(AutoWireAttribute)))
            .ToList();

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0 || parameters[0].ParameterType != typeof(IServiceCollection))
                throw new InvalidOperationException($"A method attributed with the {nameof(AutoWireAttribute)} must specify a single IServiceCollection typed parameter.");
            if (parameters.Length > 1)
                throw new InvalidOperationException("Cannot auto-wire methods with more than 1 parameter.");
            if (!method.IsStatic)
                throw new InvalidOperationException($"A method attributed with the {nameof(AutoWireAttribute)} must be static.");

            method.Invoke(null, new[] { builder.Services });
        }

        return builder;
    }

    private static void LoadAssembliesFromFileSystem(string pathToLoadFrom)
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());

        var directory = new DirectoryInfo(pathToLoadFrom);
        foreach (var file in directory.EnumerateFiles("*.dll"))
        {
            if (!context!.Assemblies.Any(a => a.Location == file.FullName))
                context.LoadFromAssemblyPath(file.FullName);
            //TODO - Scan assembly and unload if no attributed methods found
        }
    }
}
