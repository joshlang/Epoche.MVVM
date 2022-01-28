using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.ModelBuilders;
static class ModelClassBuilder
{
    public static ModelClassModel Create(GenerateModel generateModel, INamedTypeSymbol classSymbol)
    {
        bool IsViewModel(INamedTypeSymbol? symbol) =>
            generateModel.ViewModelBaseSymbol is null || symbol?.TypeKind != TypeKind.Class ? false :
            symbol.Equals(generateModel.ViewModelBaseSymbol, SymbolEqualityComparer.Default) ? true :
            IsViewModel(symbol.BaseType);

        var model = new ModelClassModel
        {
            GenerateModel = generateModel,
            ClassName = classSymbol.Name,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
            IsViewModel = IsViewModel(classSymbol)
        };

        foreach (var memberSymbol in classSymbol.GetMembers())
        {
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
                case "Epoche.MVVM.Models.InjectAttribute":
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
                case "Epoche.MVVM.Models.WithFactoryAttribute":
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
        foreach (var field in model.Fields.Where(x => x.FactoryInitializer))
        {
            if (!model.Injections.Any(x => x.FullTypeName == field.FactoryInitializerFullTypeName))
            {
                model.Injections.Add(new InjectModel
                {
                    FullTypeName = field.FactoryInitializerFullTypeName!,
                    PropertyName = field.FactoryInitializerFullTypeName!.Split('.').Last().WithoutInterface()
                });
            }
        }
        
        var constructor = classSymbol
            .BaseType?
            .Constructors
            .AsEnumerable()
            .Where(x => !x.IsStatic)
            .OrderByDescending(x => x.Parameters.Length)
            .FirstOrDefault();
        if (constructor is not null)
        {
            foreach (var p in constructor.Parameters)
            {
                model.BaseConstructorArgs.Add((p.Type.FullTypeName(), p.Name));
            }
        }
        model.FactoryInterfaceName ??= $"I{model.ClassName}Factory";
        model.FactoryClassName ??= $"{model.ClassName}Factory";

        HookupChangedBy(model);
        RemoveMissingCanExecute(model);

        return model;
    }

    static void HookupChangedBy(ModelClassModel model)
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

    static void RemoveMissingCanExecute(ModelClassModel model)
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