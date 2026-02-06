using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.Analyzer.Test;

public class AutoWireAnalyzerTests
{
    //No diagnostics expected to show up
    [Fact]
    public async Task TestMethod1()
    {
        var test = new CSharpAnalyzerTest<AutoWireAnalyzer, DefaultVerifier>
        {
            TestCode = "",
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        await test.RunAsync();
    }

    //Diagnostic and CodeFix both triggered and checked for
    [Fact]
    public async Task TestMethod2()
    {
        var test = new CSharpAnalyzerTest<AutoWireAnalyzer, DefaultVerifier>
        {
            TestCode = @"
    using AutoWire;
    using Microsoft.Extensions.DependencyInjection;

    namespace ConsoleApplication1
    {
        public static class Test
        {   
            [|[AutoWire]
            public static void Inject(IServiceCollection services, object somethingElse)
{ }|]
        }
    }".Trim(),
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        test.TestState.AdditionalReferences.Add(typeof(AutoWireAttribute).Assembly);
        test.TestState.AdditionalReferences.Add(typeof(IServiceCollection).Assembly);

        await test.RunAsync();
    }
}
