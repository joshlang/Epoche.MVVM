using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.ViewModels;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEpocheViewModels(this IServiceCollection services, bool scoped = false)
    {
        if (!services.Any(x => x.ServiceType == typeof(IPresenter)))
        {
            if (scoped)
            {
                services.AddScoped<IPresenter, NullPresenter>();
            }
            else
            {
                services.AddSingleton<IPresenter, NullPresenter>();
            }
        }
        if (!services.Any(x => x.ServiceType == typeof(INavigator)))
        {
            if (scoped)
            {
                services.AddScoped<INavigator, Navigator>();
            }
            else
            {
                services.AddSingleton<INavigator, Navigator>();
            }
        }

        return services
            .AddSingleton<INotificationService, NullNotificationService>();
    }

    public static IServiceCollection AddViewModelsInAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in ViewModelHelpers.GetViewModelTypes(assembly))
        {
            services.AddTransient(type);
        }

        return services;
    }
}
