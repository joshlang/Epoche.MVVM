using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.Writers;
static class ModelClassWriter
{
    public static void Write(GeneratorExecutionContext context, ModelClassModel model)
    {
        context.AddSource($"{model.ClassName}.g.cs", ViewModel(model));
        if (model.WithFactory)
        {
            if (!string.IsNullOrEmpty(model.FactoryInterfaceName))
            {
                context.AddSource($"{model.FactoryInterfaceName}.g.cs", FactoryInterface(model));
            }
            context.AddSource($"{model.FactoryClassName}.g.cs", Factory(model));
        }
    }

    static string ViewModel(ModelClassModel model) => @$"
#nullable enable
namespace {model.Namespace};
partial class {model.ClassName}
{{
    {Constructor(model)}

    partial void Init();
    
    {string.Concat(model.Injections.Select(InjectionProperty))}

    {string.Concat(model.Fields.Select(FieldModelWriter.Property))}

    {string.Concat(model.Methods.Select(MethodModelWriter.CommandProperty))}

    {string.Concat(model.Methods.Select(MethodModelWriter.CommandTaskProperty))}
}}
";

    static string FieldFactoryInitializer(ModelClassModel model, FieldModel fieldModel) =>
        string.IsNullOrEmpty(fieldModel.FactoryInitializerFullTypeName) ? "" : $@"
        this.{fieldModel.FieldName} = {model.Injections.First(x => x.FullTypeName == fieldModel.FactoryInitializerFullTypeName).PropertyName.ToCamelCase()}.Create();";

    static string Constructor(ModelClassModel model) => $@"
    public {model.ClassName}({AllConstructorArgs(model)}){BaseArgs(model)}
    {{
        {string.Concat(model.Injections.Select(x => AssignInjection(model, x)))}
        {string.Concat(model.Methods.Select(MethodModelWriter.CreateCommand))}
        {string.Concat(model.Fields.Select(x => FieldFactoryInitializer(model, x)))}
        this.Init();
    }}
";

    static string BaseArgs(ModelClassModel model) =>
        model.BaseConstructorArgs.Count == 0 ? "" : $@": base({string.Join(", ", model.BaseConstructorArgs.Select(x => x.Name))})";

    static string AllConstructorArgs(ModelClassModel model)
    {
        var baseargs = model.BaseConstructorArgs.Select(x => $"{x.FullTypeName} {x.Name}");
        var injects = model.Injections.Where(x => !model.BaseConstructorArgs.Any(c => c.FullTypeName == x.FullTypeName)).Select(x => $@"{x.FullTypeName} {x.PropertyName.ToCamelCase()}");
        string s = string.Join("," + Environment.NewLine, baseargs.Concat(injects).Select(x => $"        {x}"));
        return s == "" ? s : Environment.NewLine + s;
    }

    static string FactoryCreateArgs(ModelClassModel model)
    {
        var baseargs = model.BaseConstructorArgs.Select(x => x.Name);
        var injects = model.Injections.Where(x => !model.BaseConstructorArgs.Any(c => c.FullTypeName == x.FullTypeName)).Select(x => x.PropertyName);
        return string.Join(", ", baseargs.Concat(injects).Select(x => $"this.{x}"));
    }

    static string AssignFactoryArgs(ModelClassModel model)
    {
        var baseargs = model.BaseConstructorArgs.Select(x => $"this.{x.Name} = {x.Name};");
        var injects = model.Injections.Where(x => !model.BaseConstructorArgs.Any(c => c.FullTypeName == x.FullTypeName)).Select(x => $"this.{x.PropertyName} = {x.PropertyName.ToCamelCase()};");
        return string.Join(Environment.NewLine, baseargs.Concat(injects).Select(x => $"        {x}"));
    }

    static string Factory(ModelClassModel model) => @$"
#nullable enable
namespace {model.Namespace};
{model.FactoryClassAccessibility}{(model.FactoryClassAccessibility is null ? "" : " ")}partial class {model.FactoryClassName}{(string.IsNullOrEmpty(model.FactoryInterfaceName) ? "" : $" : {model.FactoryInterfaceName}")}
{{
    public {model.FactoryClassName}({AllConstructorArgs(model)})
    {{
{AssignFactoryArgs(model)}
    }}
    
    {string.Concat(model.Injections.Where(x => !model.BaseConstructorArgs.Any(c => c.FullTypeName == x.FullTypeName)).Select(FactoryInjectionProperty))}
    
    {string.Concat(model.BaseConstructorArgs.Select(ConstructorProperty))}

    public {model.ClassName} Create() => new({FactoryCreateArgs(model)});
}}
";

    static string FactoryInterface(ModelClassModel model) => @$"
namespace {model.Namespace};
{model.FactoryInterfaceAccessibility}{(model.FactoryInterfaceAccessibility is null ? "" : " ")}interface {model.FactoryInterfaceName}
{{
    {model.Namespace}.{model.ClassName} Create();
}}
";


    static string AssignInjection(ModelClassModel model, InjectModel injectModel) => $@"
        this.{injectModel.PropertyName} = {model.BaseConstructorArgs.FirstOrDefault(x => x.FullTypeName == injectModel.FullTypeName).Name ?? injectModel.PropertyName.ToCamelCase()};";

    static string InjectionProperty(InjectModel model) => $@"
    {model.AccessModifier}{(model.AccessModifier is null ? "" : " ")}{model.FullTypeName} {model.PropertyName} {{ get; }}";
    static string FactoryInjectionProperty(InjectModel model) => $@"
    readonly {model.FullTypeName} {model.PropertyName};";

    static string ConstructorProperty((string FullTypeName, string Name) prop) => $@"
    readonly {prop.FullTypeName} {prop.Name};";
}
