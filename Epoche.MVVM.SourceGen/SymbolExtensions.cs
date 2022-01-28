namespace Epoche.MVVM.SourceGen;
static class SymbolExtensions
{
    static readonly SymbolDisplayFormat DisplayFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,        
        memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static string FullTypeName(this ITypeSymbol typeSymbol) => typeSymbol.ToDisplayString(DisplayFormat);

    public static string FullMethodName(this IMethodSymbol methodSymbol) => methodSymbol.ToDisplayString(DisplayFormat);
}
