namespace Epoche.MVVM.SourceGen.Models;
class PropertyModel
{
    public ModelClassModel ViewModel = default!;
    public string PropertyName = default!;
    public List<string> ChangedBy = new();
}
