﻿using Epoche.MVVM.ViewModels.SourceGen.Models;

namespace Epoche.MVVM.ViewModels.SourceGen.Writers;
static class FieldModelWriter
{
    public static string Property(FieldModel model) =>
        !model.GenerateProperty ? "" : $@"

    public {model.FullTypeName} {model.PropertyName}
    {{
        get => this.{model.FieldName};
        set
        {{
            if (Set(
                ref this.{model.FieldName}, 
                value,
                equalityComparer: {model.EqualityComparer ?? "null"},
                onChange: {model.OnChange ?? "null"},
                trackChanges: {(model.TrackChanges ? "true" : "false")},
                cachedPropertyChangedEventArgs: Epoche.MVVM.ViewModels.CachedPropertyChangeEventArgs.{model.PropertyName}))
            {{
                {string.Concat(model.AffectedProperties.Where(x => x != model.PropertyName).Select(AffectedProperty))}
                {string.Concat(model.AffectedCommands.Select(AffectedCommand))}
            }}
        }}
    }}
";

    static string AffectedProperty(string propertyName) => $@"
                this.RaisePropertyChanged(Epoche.MVVM.ViewModels.CachedPropertyChangeEventArgs.{propertyName});";

    static string AffectedCommand(string commandName) => $@"
                this.{commandName}.RaiseCanExecuteChanged();";
}
