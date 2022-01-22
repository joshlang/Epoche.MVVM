namespace Epoche.MVVM.ViewModels.SourceGen.Models;
class PropertyModel
{
    public ViewModelClassModel ViewModel = default!;
    public string PropertyName = default!;
    public List<string> ChangedBy = new();
}
