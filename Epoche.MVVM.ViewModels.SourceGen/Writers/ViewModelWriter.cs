using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.Writers;
static class ViewModelWriter
{
    public static void Write(GeneratorExecutionContext context, ViewModelClassModel model)
    {
        context.AddSource($"{model.ClassName}.g.cs", ViewModel(model));
        if (model.WithFactory)
        {
            context.AddSource($"{model.FactoryInterfaceName}.g.cs", FactoryInterface(model));
            context.AddSource($"{model.FactoryClassName}.g.cs", Factory(model));
        }
    }

    static string ViewModel(ViewModelClassModel model) => @$"
#nullable enable
namespace {model.Namespace};
partial class {model.ClassName}
{{
    public {model.ClassName}({string.Concat(model.Injections.Select(ConstructorArg)).TrimEnd(',')})
    {{
        {string.Concat(model.Injections.Select(AssignInjection))}
        {string.Concat(model.Methods.Select(MethodModelWriter.CreateCommand))}
        OnInitialize();
    }}
    
    partial void OnInitialize();

    {string.Concat(model.Injections.Select(InjectionProperty))}

    {string.Concat(model.Fields.Select(FieldModelWriter.Property))}

    {string.Concat(model.Methods.Select(MethodModelWriter.CommandProperty))}

    {string.Concat(model.Methods.Select(MethodModelWriter.CommandTaskProperty))}
}}
";

    static string Factory(ViewModelClassModel model) => @$"
#nullable enable
namespace {model.Namespace};
{model.FactoryClassAccessibility}{(model.FactoryClassAccessibility is null ? "" : " ")}partial class {model.FactoryClassName}
{{
    public {model.FactoryClassName}({string.Concat(model.Injections.Select(ConstructorArg)).TrimEnd(',')})
    {{
        {string.Concat(model.Injections.Select(AssignInjection))}
    }}
    
    {string.Concat(model.Injections.Select(InjectionProperty))}

    public {model.ClassName} Create() => new({string.Join(", ", model.Injections.Select(x => $"this.{x.PropertyName}"))});
}}
";

    static string FactoryInterface(ViewModelClassModel model) => @$"
namespace {model.Namespace};
{model.FactoryInterfaceAccessibility}{(model.FactoryInterfaceAccessibility is null ? "" : " ")}interface {model.FactoryInterfaceName}
{{
    {model.Namespace}.{model.ClassName} Create();
}}
";

    static string ConstructorArg(InjectModel model) => $@"
        {model.FullTypeName} {model.PropertyName.ToCamelCase()},";

    static string AssignInjection(InjectModel model) => $@"
        this.{model.PropertyName} = {model.PropertyName.ToCamelCase()};";

    static string InjectionProperty(InjectModel model) => $@"
    {model.AccessModifier}{(model.AccessModifier is null ? "" : " ")}{model.FullTypeName} {model.PropertyName} {{ get; }}
";
}
