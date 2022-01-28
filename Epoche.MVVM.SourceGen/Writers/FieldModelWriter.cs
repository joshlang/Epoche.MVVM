using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.Writers;
static class FieldModelWriter
{
    public static string Property(FieldModel model) =>
        !model.GenerateProperty ? "" : $@"

    public {model.FullTypeName} {model.PropertyName} {GetSet(model)}
";

    static string GetSet(FieldModel model) =>
        model.IsReadOnly ? 
        $"=> this.{model.FieldName};" : 
        $@"
    {{
        get => this.{model.FieldName};
        {(model.PrivateSetter ? "private " : "")}set
        {{
            if (this.Set(
                ref this.{model.FieldName}, 
                value,
                equalityComparer: {model.EqualityComparer ?? "null"},
                onChange: {model.OnChange ?? "null"},
                trackChanges: {(model.TrackChanges ? "true" : "false")},
                cachedPropertyChangedEventArgs: Epoche.MVVM.CachedPropertyChangeEventArgs.{model.PropertyName}))
            {{
                {string.Concat(model.AffectedProperties.Where(x => x != model.PropertyName).Select(AffectedProperty))}
                {string.Concat(model.AffectedCommands.Select(AffectedCommand))}
            }}
        }}
    }}";

    static string AffectedProperty(string propertyName) => $@"
                this.RaisePropertyChanged(Epoche.MVVM.CachedPropertyChangeEventArgs.{propertyName});";

    static string AffectedCommand(string commandName) => $@"
                this.{commandName}.RaiseCanExecuteChanged();";
}
