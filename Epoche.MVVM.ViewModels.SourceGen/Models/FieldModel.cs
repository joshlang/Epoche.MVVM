namespace Epoche.MVVM.ViewModels.SourceGen.Models;
class FieldModel
{
    public ViewModelClassModel ViewModel = default!;
    public string FieldName = default!;
    public string PropertyName = default!;
    public string FullTypeName = default!;
    public bool IsReadOnly;
    public bool PrivateSetter;
    public bool GenerateProperty;
    public string? OnChange;
    public string? EqualityComparer;
    public bool TrackChanges = true;
    public HashSet<string> AffectedProperties = new();
    public HashSet<string> AffectedCommands = new();
}
