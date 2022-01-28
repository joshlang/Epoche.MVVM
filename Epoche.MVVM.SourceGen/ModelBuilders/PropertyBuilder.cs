using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.ModelBuilders;
static class PropertyBuilder
{
    public static PropertyModel Create(ModelClassModel classModel, IPropertySymbol property)
    {
        var model = new PropertyModel
        {
            ClassModel = classModel,
            PropertyName = property.Name
        };
        foreach (var a in property.GetAttributes())
        {
            var attributeName = a.AttributeClass?.FullTypeName();
            if (attributeName == "Epoche.MVVM.Models.ModelBase.ChangedByAttribute")
            {
                model.ChangedBy.AddRange(a.ConstructorArguments.SelectMany(x => x.Values).Select(x => (string)x.Value!).Where(x => x is not null));
            }
        }
        return model;
    }
}
