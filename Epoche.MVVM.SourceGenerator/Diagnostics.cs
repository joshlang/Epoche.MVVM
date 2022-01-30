namespace Epoche.MVVM.SourceGenerator;
static class Diagnostics
{
    public static class Errors
    {
        static DiagnosticDescriptor Create(string id, string text) => new DiagnosticDescriptor(id, text, text, "SourceGeneration", DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor NotPartial = Create("GEN001", "Classes decorated with [UseSourceGen] must be declared with the 'partial' modifier");
        public static DiagnosticDescriptor SubClass = Create("GEN002", "Nested types are not supported");
        public static DiagnosticDescriptor InjectMissingType = Create("GEN003", "[Inject] is missing a Type");
    }
    public static class Warnings
    {
    }
}
