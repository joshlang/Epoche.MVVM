using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.ModelBuilders;
static class GenerateModelBuilder
{
    public static GenerateModel Create(GeneratorExecutionContext context, SyntaxReceiver syntax)
    {
        var model = new GenerateModel();

        var viewModelBase = context.Compilation.GetTypeByMetadataName("Epoche.MVVM.ViewModels.ViewModelBase");
        bool IsViewModel(INamedTypeSymbol? symbol) =>
            symbol?.TypeKind != TypeKind.Class ? false :
            symbol.Equals(viewModelBase, SymbolEqualityComparer.Default) ? true :
            IsViewModel(symbol.BaseType);

        foreach (var subclass in syntax.Subclasses)
        {
            var classModel = context.Compilation.GetSemanticModel(subclass.SyntaxTree);
            if (classModel.GetDeclaredSymbol(subclass) is not INamedTypeSymbol classSymbol || !IsViewModel(classSymbol))
            {
                continue;
            }

            model.ViewModelClassModels.Add(ViewModelBuilder.Create(model, classSymbol));
        }

        return model;
    }
}
