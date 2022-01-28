namespace Epoche.MVVM.SourceGen.Models;
class GenerateModel
{
    public INamedTypeSymbol? ViewModelBaseSymbol = default!;
    public List<ModelClassModel> ViewModelClassModels = new();
    public GeneratorExecutionContext Context;
}
