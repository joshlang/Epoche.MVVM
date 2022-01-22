using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.ModelBuilders;
static class PropertyBuilder
{
    public static PropertyModel Create(ViewModelClassModel viewModel, IPropertySymbol property)
    {
        var model = new PropertyModel
        {
            ViewModel = viewModel,
            PropertyName = property.Name
        };
        foreach (var a in property.GetAttributes())
        {
            var attributeName = a.AttributeClass?.FullTypeName();
            if (attributeName == "Epoche.MVVM.ViewModels.ViewModelBase.ChangedByAttribute")
            {
                model.ChangedBy.AddRange(a.ConstructorArguments.SelectMany(x => x.Values).Select(x => (string)x.Value!).Where(x => x is not null));
            }
        }
        return model;
    }
}
