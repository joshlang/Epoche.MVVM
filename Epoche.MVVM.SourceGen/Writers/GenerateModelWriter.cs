using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.Writers;
static class GenerateModelWriter
{
    public static void Write(GeneratorExecutionContext context, GenerateModel generateModel)
    {
        CachedPropertyChangeEventArgsWriter.Write(context, generateModel);
        foreach (var viewModel in generateModel.ViewModelClassModels)
        {
            ModelClassWriter.Write(context, viewModel);
        }
    }
}
