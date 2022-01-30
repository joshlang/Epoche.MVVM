using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class CommandAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, MethodModel methodModel, AttributeData attributeData)
    {
        var model = new CommandAttributeModel
        {
            AttributeData = attributeData
        };

        if (attributeData.ConstructorArguments.Length > 0)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Name":
                    model.Name = named.Value.Value as string;
                    break;
                case "AllowConcurrency":
                    model.AllowConcurrency = (bool?)named.Value.Value ?? false;
                    break;
                case "CanExecute":
                    model.CanExecute = named.Value.Value as string;
                    break;
                case "TaskName":
                    model.TaskName = named.Value.Value as string;
                    break;
            }
        }

        methodModel.CommandAttribute = model;
    }
}
