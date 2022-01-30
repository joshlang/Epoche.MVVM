﻿using Epoche.MVVM.SourceGenerator.Builders.Attributes;
using Epoche.MVVM.SourceGenerator.Models;

namespace Epoche.MVVM.SourceGenerator.Builders;
static class ClassModelBuilder
{
    public static void Build(OutputModel outputModel, ClassDeclarationSyntax syntax)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        if (syntax.Parent is TypeDeclarationSyntax)
        {
            outputModel.Context.Report(Diagnostics.Errors.SubClass, syntax);
            return;
        }

        var model = new ClassModel();
        var semanticModel = outputModel.Compilation.GetSemanticModel(syntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol classSymbol) { return; }

        model.Namespace = classSymbol.ContainingNamespace.ToDisplayString();
        model.ClassName = classSymbol.Name.ToString();

        foreach (var attributeData in classSymbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.UseSourceGenAttributeSymbol))
            {
                UseSourceGenAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.InjectAttributeSymbol))
            {
                InjectAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.WithFactoryAttributeSymbol))
            {
                WithFactoryAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }
        if (model.UseSourceGenAttribute is null) { return; }
        if (!syntax.Modifiers.Any(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
        {
            outputModel.Context.Report(Diagnostics.Errors.NotPartial, syntax);
            return;
        }

        foreach (var member in classSymbol.GetMembers())
        {
            switch (member)
            {
                case IFieldSymbol symbol:
                    FieldModelBuilder.Build(outputModel, model, symbol);
                    break;
                case IPropertySymbol symbol:
                    PropertyModelBuilder.Build(outputModel, model, symbol);
                    break;
                case IMethodSymbol symbol:
                    MethodModelBuilder.Build(outputModel, model, symbol);
                    break;
            }
        }

        outputModel.Classes.Add(model);
    }
}
