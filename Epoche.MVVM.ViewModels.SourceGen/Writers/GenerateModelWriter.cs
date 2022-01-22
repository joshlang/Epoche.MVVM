using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.Writers;
static class GenerateModelWriter
{
    public static void Write(GeneratorExecutionContext context, GenerateModel generateModel)
    {
        CachedPropertyChangeEventArgsWriter.Write(context, generateModel);
        foreach (var viewModel in generateModel.ViewModelClassModels)
        {
            ViewModelWriter.Write(context, viewModel);
        }
    }
}
