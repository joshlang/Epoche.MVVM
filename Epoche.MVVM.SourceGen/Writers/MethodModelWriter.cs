using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.Writers;
static class MethodModelWriter
{
    public static string CreateCommand(MethodModel model) =>
        !model.GenerateCommand ? "" : $@"
        this.{model.CommandName} = new(
            {CreateCommandExecute(model)},
            {CreateCanCommandExecute(model)});
";

    static string CreateCommandExecute(MethodModel model) =>
        !model.AsyncCommand ? $"this.{model.MethodName}" :
        model.AllowConcurrency ? (model.ParameterType is null ? $"() => this.{model.MethodName}()" : $"x => this.{model.MethodName}(x)") :
        $@"
            async {(model.ParameterType is null ? "()" : "x")} => 
            {{
                if (this.{model.AsyncCommandTaskName}?.IsCompleted == false) {{ return; }}
                var task = this.{model.MethodName}({(model.ParameterType is null ? "" : "x")});
                var notify = new Epoche.MVVM.Models.NotifyTask{(string.IsNullOrEmpty(model.AsyncCommandReturnType) ? "" : $"<{model.AsyncCommandReturnType}>")}(task);
                this.{model.AsyncCommandTaskName} = notify;
                this.{model.CommandName}!.RaiseCanExecuteChanged();
                await notify.TaskCompletion;
                this.{model.CommandName}!.RaiseCanExecuteChanged();
            }}";

    static string CreateCanCommandExecute(MethodModel model) =>
        !model.AsyncCommand || model.AllowConcurrency || model.AsyncCommandTaskName is null ?
        (string.IsNullOrEmpty(model.CanExecuteName) ? "null" : $"this.{model.CanExecuteName}"!) :
        $@"
            {(model.ParameterType is null ? "()" : "x")} =>
            {{
                if (this.{model.AsyncCommandTaskName}?.IsCompleted == false) {{ return false; }}
                return this.{(string.IsNullOrEmpty(model.CanExecuteName) ? "true" : $"{model.CanExecuteName}({(model.ParameterType is null ? "" : "x")})")};
            }}";

    public static string CommandProperty(MethodModel model) =>
        !model.GenerateCommand ? "" : @$"
    public Epoche.MVVM.ViewModels.DelegateCommand{(model.ParameterType is null ? "" : $"<{model.ParameterType}>")} {model.CommandName} {{ get; }}";

    public static string CommandTaskProperty(MethodModel model) =>
        !model.GenerateCommand || !model.AsyncCommand || model.AllowConcurrency || model.AsyncCommandTaskName is null ? "" : @$"
    Epoche.MVVM.Models.NotifyTask{(string.IsNullOrEmpty(model.AsyncCommandReturnType) ? "" : $"<{model.AsyncCommandReturnType}>")}? {model.AsyncCommandTaskName.ToCamelCase()};
    public Epoche.MVVM.Models.NotifyTask{(string.IsNullOrEmpty(model.AsyncCommandReturnType) ? "" : $"<{model.AsyncCommandReturnType}>")}? {model.AsyncCommandTaskName}
    {{
        get => this.{model.AsyncCommandTaskName.ToCamelCase()};
        set => this.Set(ref {model.AsyncCommandTaskName.ToCamelCase()}, value, trackChanges: false, cachedPropertyChangedEventArgs: Epoche.MVVM.CachedPropertyChangeEventArgs.{model.AsyncCommandTaskName});
    }}
";
}
