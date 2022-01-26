using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Epoche.MVVM.Presentation.Web;

sealed class PageHelper
{
    public PageHelper(Assembly[] assemblies)
    {
        if (assemblies is null)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        PageRouting = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract)
            .Where(x => x.IsSubclassOf(typeof(ComponentBase)))
            .Select(x =>
            {
                var routes = x.GetCustomAttributes(typeof(RouteAttribute), false);
                var target = x.GetGenericInterfaces(typeof(ITarget<>)).SingleOrDefault();
                if (routes.Length == 0 || target is null)
                {
                    return null;
                }
                return new
                {
                    TargetType = target.GenericTypeArguments[0],
                    routes
                        .Cast<RouteAttribute>()
                        .OrderBy(y => y.Template.Length)
                        .First()
                        .Template
                };
            })
            .ExcludeNull()
            .Select(x => (x.Template, x.TargetType))
            .ToArray();

        ViewModelTypesByRoute = PageRouting.ToDictionary(x => x.RouteTemplate.TrimStart('/'), x => x.TargetViewModelType);
        RoutesByViewModelType = PageRouting.ToDictionary(x => x.TargetViewModelType, x => x.RouteTemplate);
    }

    public (string RouteTemplate, Type TargetViewModelType)[] PageRouting { get; }
    public Dictionary<string, Type> ViewModelTypesByRoute { get; }
    public Dictionary<Type, string> RoutesByViewModelType { get; }
}
