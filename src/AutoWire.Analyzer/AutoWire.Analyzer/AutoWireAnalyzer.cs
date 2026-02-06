using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoWire.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoWireAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AW0001";

        private const string Category = "Usage";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, _title, _messageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void CompilationStartAction(CompilationStartAnalysisContext context)
        {
            var targetAttribute = context.Compilation.GetTypeByMetadataName("AutoWire.AutoWireAttribute");
            var targetParameterType = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection");
            if (targetAttribute is null || targetParameterType is null)
            {
                return;
            }

            context.RegisterSymbolAction(symbolContext =>
            {
                var methodSymbol = (IMethodSymbol)symbolContext.Symbol;
                if (!methodSymbol.GetAttributes().Any(attrData => targetAttribute.Equals(attrData.AttributeClass, SymbolEqualityComparer.Default)))
                {
                    return;
                }

                if (methodSymbol.Parameters.Length != 1 || !methodSymbol.Parameters.Single().Type.Equals(targetParameterType, SymbolEqualityComparer.Default))
                {
                    var location = methodSymbol.DeclaringSyntaxReferences
                        .FirstOrDefault()?
                        .GetSyntax(symbolContext.CancellationToken)?
                        .GetLocation();
                    var diagnostic = Diagnostic.Create(_rule, location, methodSymbol.Name);
                    symbolContext.ReportDiagnostic(diagnostic);
                }
            }, SymbolKind.Method);
        }
    }
}
