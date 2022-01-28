using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.ModelBuilders;
static class GenerateModelBuilder
{
    public static GenerateModel Create(GeneratorExecutionContext context, SyntaxReceiver syntax)
    {
        var model = new GenerateModel
        {
            ViewModelBaseSymbol = context.Compilation.GetTypeByMetadataName("Epoche.MVVM.ViewModels.ViewModelBase"),
            Context = context
        };

        var modelBase = context.Compilation.GetTypeByMetadataName("Epoche.MVVM.Models.ModelBase");
        bool IsModel(INamedTypeSymbol? symbol) =>
            modelBase is null || symbol?.TypeKind != TypeKind.Class ? false :
            symbol.Equals(modelBase, SymbolEqualityComparer.Default) ? true :
            IsModel(symbol.BaseType);

        foreach (var subclass in syntax.Subclasses)
        {
            var classModel = context.Compilation.GetSemanticModel(subclass.SyntaxTree);            
            if (classModel.GetDeclaredSymbol(subclass) is not INamedTypeSymbol classSymbol || !IsModel(classSymbol))
            {
                continue;
            }

            model.ViewModelClassModels.Add(ModelClassBuilder.Create(model, classSymbol));
        }

        return model;
    }
}
