using Epoche.MVVM.ViewModels.SourceGen.ModelBuilders;
using Epoche.MVVM.ViewModels.SourceGen.Writers;

namespace Epoche.MVVM.ViewModels.SourceGen;

class Executor
{
    readonly GeneratorExecutionContext Context;
    readonly SyntaxReceiver Syntax;

    public Executor(GeneratorExecutionContext context, SyntaxReceiver syntax)
    {
        Context = context;
        Syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
    }

    public void Generate()
    {
        var model = GenerateModelBuilder.Create(Context, Syntax);
        GenerateModelWriter.Write(Context, model);
    }
}