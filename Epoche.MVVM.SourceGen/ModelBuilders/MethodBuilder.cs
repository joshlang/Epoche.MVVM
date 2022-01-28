using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.ModelBuilders;
static class MethodBuilder
{
    public static MethodModel Create(ModelClassModel viewModel, IMethodSymbol method)
    {
        var model = new MethodModel
        {
            ViewModel = viewModel,
            MethodName = method.Name,
            CommandName = $"{method.Name}Command",
            CanExecuteName = $"Can{method.Name}",
            AsyncCommandTaskName = $"{method.Name}CommandTask"
        };
        foreach (var a in method.GetAttributes())
        {
            var attributeName = a.AttributeClass?.FullTypeName();
            if (attributeName == "Epoche.MVVM.ViewModels.ViewModelBase.CommandAttribute")
            {
                model.GenerateCommand = true;
                var commandName = a.ConstructorArguments.FirstOrDefault().Value as string;
                if (!string.IsNullOrEmpty(commandName))
                {
                    model.CommandName = commandName!;
                }
                if (method.Parameters.Length > 0)
                {
                    var paramType = method.Parameters[0].Type;
                    model.ParameterType = paramType.FullTypeName();
                    if (paramType.IsReferenceType && paramType.NullableAnnotation == NullableAnnotation.Annotated)
                    {
                        model.ParameterType += "?";
                    }
                }
                model.AsyncCommand = method.ReturnType.FullTypeName().StartsWith("System.Threading.Tasks.Task");
                if (model.AsyncCommand)
                {
                    var returnTypeSymbol = method.ReturnType as INamedTypeSymbol;
                    if (returnTypeSymbol?.TypeArguments.Length > 0)
                    {
                        model.AsyncCommandReturnType = returnTypeSymbol.TypeArguments[0].FullTypeName();
                        if (returnTypeSymbol.TypeArguments[0].IsReferenceType && returnTypeSymbol.TypeArguments[0].NullableAnnotation == NullableAnnotation.Annotated)
                        {
                            model.AsyncCommandReturnType += "?";
                        }
                    }
                }
                foreach (var named in a.NamedArguments)
                {
                    switch (named.Key)
                    {
                        case "AllowConcurrency":
                            model.AllowConcurrency = (bool?)named.Value.Value ?? false;
                            break;
                        case "CanExecute":
                            model.CanExecuteName = named.Value.Value as string;
                            break;
                        case "TaskName":
                            model.AsyncCommandTaskName = named.Value.Value as string;
                            break;
                    }
                }
            }
            else if (attributeName == "Epoche.MVVM.ViewModels.ViewModelBase.ChangedByAttribute")
            {
                model.ChangedBy.AddRange(a.ConstructorArguments.SelectMany(x => x.Values).Select(x => (string)x.Value!).Where(x => x is not null));
            }
        }
        return model;
    }
}
