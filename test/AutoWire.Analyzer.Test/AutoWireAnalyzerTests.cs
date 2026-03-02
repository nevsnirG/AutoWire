using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.Analyzer.Test;

public class AutoWireAnalyzerTests
{
    private static CSharpAnalyzerTest<AutoWireAnalyzer, DefaultVerifier> CreateTest(string testCode)
    {
        var test = new CSharpAnalyzerTest<AutoWireAnalyzer, DefaultVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };
        test.TestState.AdditionalReferences.Add(typeof(AutoWireAttribute).Assembly);
        test.TestState.AdditionalReferences.Add(typeof(IServiceCollection).Assembly);
        return test;
    }

    [Fact]
    public async Task EmptyCode_NoDiagnostics()
    {
        var test = new CSharpAnalyzerTest<AutoWireAnalyzer, DefaultVerifier>
        {
            TestCode = "",
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ValidMethod_SingleIServiceCollectionParameter_NoDiagnostics()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                [AutoWire]
                public static void Register(IServiceCollection services) { }
            }
            """);

        await test.RunAsync();
    }

    [Fact]
    public async Task ValidMethod_PrivateMethod_NoDiagnostics()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                [AutoWire]
                private static void Register(IServiceCollection services) { }
            }
            """);

        await test.RunAsync();
    }

    [Fact]
    public async Task ValidMethod_MultipleAutoWireMethods_NoDiagnostics()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                [AutoWire]
                public static void RegisterA(IServiceCollection services) { }

                [AutoWire]
                public static void RegisterB(IServiceCollection services) { }
            }
            """);

        await test.RunAsync();
    }

    [Fact]
    public async Task MethodWithoutAttribute_WrongSignature_NoDiagnostics()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                public static void NotAutoWired(string something) { }
            }
            """);

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidMethod_NoParameters_ReportsDiagnostic()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                {|#0:[AutoWire]
                public static void Register() { }|}
            }
            """);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(AutoWireAnalyzer.DiagnosticId, Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("Register"));

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidMethod_WrongParameterType_ReportsDiagnostic()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                {|#0:[AutoWire]
                public static void Register(string notAServiceCollection) { }|}
            }
            """);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(AutoWireAnalyzer.DiagnosticId, Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("Register"));

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidMethod_MultipleParameters_ReportsDiagnostic()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                {|#0:[AutoWire]
                public static void Register(IServiceCollection services, object extra) { }|}
            }
            """);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(AutoWireAnalyzer.DiagnosticId, Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("Register"));

        await test.RunAsync();
    }

    [Fact]
    public async Task MixedMethods_OnlyInvalidMethodReportsDiagnostic()
    {
        var test = CreateTest("""
            using AutoWire;
            using Microsoft.Extensions.DependencyInjection;

            public static class MyServices
            {
                [AutoWire]
                public static void ValidMethod(IServiceCollection services) { }

                {|#0:[AutoWire]
                public static void InvalidMethod() { }|}
            }
            """);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(AutoWireAnalyzer.DiagnosticId, Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("InvalidMethod"));

        await test.RunAsync();
    }
}
