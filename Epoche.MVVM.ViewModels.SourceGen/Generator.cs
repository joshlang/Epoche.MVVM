namespace Epoche.MVVM.ViewModels.SourceGen;

[Generator]
class Generator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver syntaxReceiver)
        {
            return;
        }

        var executor = new Executor(context, syntaxReceiver);
        executor.Generate();
    }
}
