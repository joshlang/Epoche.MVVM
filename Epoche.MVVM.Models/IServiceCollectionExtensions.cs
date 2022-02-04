using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Models;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddModelFactoriesInAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract))
        {
            foreach (var interfaceType in type.GetGenericInterfaces(typeof(IModelFactory<>)))
            {
                services.AddSingleton(interfaceType, type);
            }
        }
        return services;
    }
}
