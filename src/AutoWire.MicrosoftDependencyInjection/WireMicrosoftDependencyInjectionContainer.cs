using AssembleMe.Abstractions;
using AutoWire.Contract;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoWire.MicrosoftDependencyInjection;
internal sealed class WireMicrosoftDependencyInjectionContainer(IServiceCollection services) : IProcessAssemblies
{
    public void ProcessAssembly(Assembly assembly)
    {
        var classes = assembly.GetTypes()
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

            method.Invoke(null, [services]);
        }
    }
}