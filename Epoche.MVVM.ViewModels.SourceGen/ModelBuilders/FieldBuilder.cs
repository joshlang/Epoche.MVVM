using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.ModelBuilders;
static class FieldBuilder
{
    public static FieldModel Create(ViewModelClassModel viewModel, IFieldSymbol field)
    {
        var model = new FieldModel
        {
            ViewModel = viewModel,
            FieldName = field.Name,
            PropertyName = field.Name.ToPascalCase(),
            FullTypeName = field.Type.FullTypeName()
        };
        if (field.Type.IsReferenceType && field.Type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            model.FullTypeName += "?";
        }
        foreach (var a in field.GetAttributes())
        {
            var attributeName = a.AttributeClass?.FullTypeName();
            if (attributeName == "Epoche.MVVM.ViewModels.ViewModelBase.PropertyAttribute")
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
                    }
                }
            }
        }
        return model;
    }
}
