using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.ModelBuilders;
static class ViewModelBuilder
{
    public static ViewModelClassModel Create(GenerateModel generateModel, INamedTypeSymbol classSymbol)
    {
        var model = new ViewModelClassModel
        {
            GenerateModel = generateModel,
            ClassName = classSymbol.Name,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString()
        };

        foreach (var memberSymbol in classSymbol.GetMembers())
        {
            // IMethodSymbol, IFieldSymbol, IPropertySymbol
            if (memberSymbol is IFieldSymbol fieldSymbol)
            {
                model.Fields.Add(FieldBuilder.Create(model, fieldSymbol));
            }
            else if (memberSymbol is IPropertySymbol propertySymbol)
            {
                model.Properties.Add(PropertyBuilder.Create(model, propertySymbol));
            }
            else if (memberSymbol is IMethodSymbol methodSymbol)
            {
                model.Methods.Add(MethodBuilder.Create(model, methodSymbol));
            }
        }
        foreach (var a in classSymbol.GetAttributes())
        {
            switch (a.AttributeClass?.FullTypeName())
            {
                case "Epoche.MVVM.ViewModels.InjectAttribute":
                    var inject = new InjectModel();
                    var type = a.ConstructorArguments[0].Value as ITypeSymbol;
                    inject.FullTypeName = type!.FullTypeName();
                    inject.PropertyName = type!.Name.WithoutInterface();
                    foreach (var named in a.NamedArguments)
                    {
                        switch (named.Key)
                        {
                            case "Name":
                                inject.PropertyName = named.Value.Value as string ?? inject.PropertyName;
                                break;
                            case "AccessModifier":
                                inject.AccessModifier = named.Value.Value as string;
                                break;
                        }
                    }
                    model.Injections.Add(inject);
                    break;
                case "Epoche.MVVM.ViewModels.WithFactoryAttribute":
                    model.WithFactory = true;
                    foreach (var named in a.NamedArguments)
                    {
                        switch (named.Key)
                        {
                            case "InterfaceName":
                                model.FactoryInterfaceName = named.Value.Value as string ?? null!;
                                break;
                            case "FactoryName":
                                model.FactoryClassName = named.Value.Value as string ?? null!;
                                break;
                            case "InterfaceAccessModifier":
                                model.FactoryInterfaceAccessibility = named.Value.Value as string ?? model.FactoryInterfaceAccessibility;
                                break;
                            case "FactoryAccessModifier":
                                model.FactoryClassAccessibility = named.Value.Value as string ?? model.FactoryClassAccessibility;
                                break;
                        }
                    }
                    break;
            }
        }
        model.FactoryInterfaceName ??= $"I{model.ClassName}Factory";
        model.FactoryClassName ??= $"{model.ClassName}Factory";

        HookupChangedBy(model);
        RemoveMissingCanExecute(model);

        return model;
    }

    static void HookupChangedBy(ViewModelClassModel model)
    {
        var fieldsByName = model
            .Fields
            .Where(x => x.GenerateProperty)
            .ToDictionary(x => x.FieldName);
        var fieldsByProp = model
            .Fields
            .Where(x => x.GenerateProperty)
            .ToDictionary(x => x.PropertyName);

        foreach (var prop in model.Properties)
        {
            foreach (var changedBy in prop.ChangedBy)
            {
                if (fieldsByName.TryGetValue(changedBy, out var field) ||
                    fieldsByProp.TryGetValue(changedBy, out field))
                {
                    field.AffectedProperties.Add(prop.PropertyName);
                }
            }
        }

        foreach (var command in model.Methods)
        {
            if (!command.GenerateCommand ||
                string.IsNullOrEmpty(command.CanExecuteName))
            {
                continue;
            }
            foreach (var changedBy in command.ChangedBy)
            {
                if (fieldsByName.TryGetValue(changedBy, out var field) ||
                       fieldsByProp.TryGetValue(changedBy, out field))
                {
                    field.AffectedCommands.Add(command.CommandName);
                }
            }
        }
    }

    static void RemoveMissingCanExecute(ViewModelClassModel model)
    {
        var methodNames = new HashSet<string>(model.Methods.Select(x => x.MethodName));
        foreach (var command in model.Methods)
        {
            if (!command.GenerateCommand ||
                command.CanExecuteName is null)
            {
                continue;
            }
            if (!methodNames.Contains(command.CanExecuteName))
            {
                command.CanExecuteName = null;
            }
        }
    }
}