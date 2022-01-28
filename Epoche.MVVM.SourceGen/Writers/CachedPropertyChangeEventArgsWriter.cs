using Epoche.MVVM.SourceGen.Models;

namespace Epoche.MVVM.SourceGen.Writers;
static class CachedPropertyChangeEventArgsWriter
{
    public static void Write(GeneratorExecutionContext context, GenerateModel generateModel)
    {
        var propertyNames = new HashSet<string>(generateModel
            .ViewModelClassModels
            .SelectMany(x => x.Fields)
            .Where(x => x.GenerateProperty)
            .SelectMany(x => x.AffectedProperties.Concat(new[] { x.PropertyName })))
            .Concat(
                generateModel
                .ViewModelClassModels
                .SelectMany(x => x.Methods)
                .Where(x => x.GenerateCommand && x.AsyncCommand && !x.AllowConcurrency && x.AsyncCommandTaskName is not null)
                .Select(x => x.AsyncCommandTaskName!));        
        
        var s = $@"
using System.ComponentModel;
namespace Epoche.MVVM;
static class CachedPropertyChangeEventArgs
{{
    {string.Concat(propertyNames.Select(WriteProperty))}
}}
";
        context.AddSource("CachedPropertyChangeEventArgs.g.cs", s);
    }

    static string WriteProperty(string propertyName) => @$"
    public static readonly PropertyChangedEventArgs {propertyName} = new(""{propertyName}"");";
}
