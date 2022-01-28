using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.ModelBuilders;
static class FieldBuilder
{
    public static FieldModel Create(ModelClassModel classModel, IFieldSymbol field)
    {
        var model = new FieldModel
        {
            ClassModel = classModel,
            FieldName = field.Name,
            FieldSymbol = field,
            PropertyName = field.Name.ToPascalCase(),
            FullTypeName = field.Type.FullTypeName(),
            IsReadOnly = field.IsReadOnly,
            TrackChanges = !classModel.IsViewModel
        };        
        if (field.Type.IsReferenceType && field.Type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            model.FullTypeName += "?";
        }
        foreach (var a in field.GetAttributes())
        {
            var attributeName = a.AttributeClass?.FullTypeName();
            if (attributeName == "Epoche.MVVM.Models.ModelBase.PropertyAttribute")
            {
                model.GenerateProperty = true;
                var propertyName = a.ConstructorArguments.FirstOrDefault().Value as string;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    model.PropertyName = propertyName!;
                }
                foreach (var named in a.NamedArguments)
                {
                    switch (named.Key)
                    {
                        case "OnChange":
                            model.OnChange = named.Value.Value as string;
                            break;
                        case "EqualityComparer":
                            model.EqualityComparer = named.Value.Value as string;
                            break;
                        case "TrackChanges":
                            model.TrackChanges = (bool?)named.Value.Value ?? false;
                            break;
                        case "PrivateSetter":
                            model.PrivateSetter = (bool?)named.Value.Value ?? false;
                            break;
                    }
                }
            }
            else if (attributeName == "Epoche.MVVM.Models.ModelBase.FactoryInitializeAttribute")
            {
                model.FactoryInitializer = true;
                var typeSymbol = a.ConstructorArguments[0].Value as ITypeSymbol;
                model.FactoryInitializerFullTypeName = typeSymbol?.FullTypeName() ?? $@"{field.Type.ContainingNamespace.ToDisplayString()}.I{field.Type.Name}Factory";
            }
        }
        return model;
    }
}
