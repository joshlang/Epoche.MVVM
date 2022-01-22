namespace Epoche.MVVM.ViewModels.SourceGen.Models;
class ViewModelClassModel
{
    public GenerateModel GenerateModel = default!;
    public string ClassName = default!;
    public string Namespace = default!;
    public List<FieldModel> Fields = new();
    public List<PropertyModel> Properties = new();
    public List<MethodModel> Methods = new();
    public List<InjectModel> Injections = new();
    public bool WithFactory;
    public string FactoryClassAccessibility = "public";
    public string FactoryClassName = default!;
    public string FactoryInterfaceAccessibility = "public";
    public string FactoryInterfaceName = default!;
}
