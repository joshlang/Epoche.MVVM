using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Models;
class MethodModel
{
    public string MethodName = default!;

    public CommandAttributeModel? CommandAttribute;
}
