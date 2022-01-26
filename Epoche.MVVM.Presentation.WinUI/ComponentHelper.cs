using System.Reflection;

namespace Epoche.MVVM.Presentation.WinUI;
sealed class ComponentHelper
{
    public ComponentHelper(Assembly[] assemblies)
    {
        if (assemblies is null)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var pairs = assemblies
            .SelectMany(x => x.GetTypes())
            .Select(x =>
            {
                var target = x.GetGenericInterfaces(typeof(ITarget<>)).SingleOrDefault();
                if (target is null)
                {
                    return null;
                }
                return new
                {
                    ViewModelType = target.GenericTypeArguments[0],
                    ComponentType = x
                };
            })
            .ExcludeNull()
            .ToArray();

        ComponentTypesByViewModel = pairs.ToDictionary(x => x.ViewModelType, x => x.ComponentType);
    }

    public Dictionary<Type, Type> ComponentTypesByViewModel { get; }
}
