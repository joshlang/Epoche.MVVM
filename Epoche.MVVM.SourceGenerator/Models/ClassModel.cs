using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class ClassModel
{
    public string Namespace = default!;
    public string ClassName = default!;

    public UseSourceGenAttributeModel UseSourceGenAttribute = default!;
    public List<InjectAttributeModel> InjectAttributes = new();
    public WithFactoryAttributeModel? WithFactoryAttribute;

    public List<FieldModel> FieldModels = new();
    public List<PropertyModel> PropertyModels = new();
    public List<MethodModel> MethodModels = new();
}
