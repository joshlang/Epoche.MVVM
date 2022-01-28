namespace Epoche.MVVM.SourceGen.Models;
class MethodModel
{
    public ModelClassModel ViewModel = default!;
    public string MethodName = default!;
    public string CommandName = default!;
    public string? CanExecuteName;
    public bool GenerateCommand;
    public bool AllowConcurrency;
    public string? ParameterType;
    public bool AsyncCommand;
    public string? AsyncCommandReturnType;
    public string? AsyncCommandTaskName;
    public List<string> ChangedBy = new();
}
