using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class FieldModel
{
    public string FieldName = default!;

    public PropertyAttributeModel? PropertyAttribute;
    public FactoryInitializeAttributeModel? FactoryInitializeAttribute;
}
