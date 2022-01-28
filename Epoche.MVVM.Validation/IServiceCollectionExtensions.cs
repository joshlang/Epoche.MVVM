using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Validation;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddValidatorsInAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var (ValidatorInterface, ValidatorType) in ValidatorHelpers.FindValidatorsInAssembly(assembly))
        {
            services.AddSingleton(ValidatorInterface, ValidatorType);
            services.AddSingleton(ValidatorType);
        }
        return services;
    }
}