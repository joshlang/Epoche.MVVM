namespace Epoche.MVVM.SourceGenerator;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(OutputStaticFiles);

        var classDeclarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(SyntaxProvider.Filter, SyntaxProvider.Transform)
            .Where(x => x is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, SourceOutput.Write);
    }

    static void OutputStaticFiles(IncrementalGeneratorPostInitializationContext context) => context.AddSource("SourceGeneratorAttributes.g.cs", Attributes.Text);
}
