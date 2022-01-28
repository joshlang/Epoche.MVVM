namespace Epoche.MVVM.SourceGen.Models;
class PropertyModel
{
    public ModelClassModel ClassModel = default!;
    public string PropertyName = default!;
    public List<string> ChangedBy = new();
}
