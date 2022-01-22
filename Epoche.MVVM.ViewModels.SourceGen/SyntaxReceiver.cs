namespace Epoche.MVVM.ViewModels.SourceGen;

class SyntaxReceiver : ISyntaxContextReceiver
{
    public readonly List<ClassDeclarationSyntax> Subclasses = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        switch (context.Node)
        {
            case ClassDeclarationSyntax syntax:
                HandleClass(context, syntax);
                break;
        }
    }

    void HandleClass(GeneratorSyntaxContext context, ClassDeclarationSyntax syntax)
    {
        if (syntax.BaseList?.Types.Count > 0)
        {
            Subclasses.Add(syntax);
        }
    }
}
