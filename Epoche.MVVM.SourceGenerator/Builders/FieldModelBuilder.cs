using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class FieldModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IFieldSymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new FieldModel
        {
            FieldName = symbol.Name
        };
        
        foreach (var attributeData in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.PropertyAttributeSymbol))
            {
                PropertyAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.FactoryInitializeAttributeSymbol))
            {
                FactoryInitializeAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }

        if (model.PropertyAttribute is null && 
            model.FactoryInitializeAttribute is null)
        {
            return;
        }



        classModel.FieldModels.Add(model);
    }
}
