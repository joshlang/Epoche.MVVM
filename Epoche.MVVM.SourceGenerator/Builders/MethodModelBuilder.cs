using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class MethodModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IMethodSymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new MethodModel
        {
            MethodName = symbol.Name
        };

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.CommandAttributeSymbol))
            {
                CommandAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }

        if (model.CommandAttribute is null)
        {
            return;
        }

        classModel.MethodModels.Add(model);
    }
}
