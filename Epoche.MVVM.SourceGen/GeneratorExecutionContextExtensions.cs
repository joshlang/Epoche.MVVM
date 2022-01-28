namespace Epoche.MVVM.SourceGen;
static class GeneratorExecutionContextExtensions
{
    static Location? GetLocation(object? syntaxNodeOrSymbol) =>
        syntaxNodeOrSymbol is SyntaxNode node
        ? node.GetLocation()
        : syntaxNodeOrSymbol is ISymbol symbol
        ? symbol.Locations.FirstOrDefault()
        : null;

    public static void Report(this GeneratorExecutionContext context, DiagnosticDescriptor descriptor, object? syntaxNodeOrSymbol) => context.ReportDiagnostic(Diagnostic.Create(descriptor, GetLocation(syntaxNodeOrSymbol)));
}
