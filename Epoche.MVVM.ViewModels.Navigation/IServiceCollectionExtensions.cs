using Epoche.MVVM.ViewModels.Navigation.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.ViewModels.Navigation;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEpocheViewModelNavigator(this IServiceCollection services)
    {
        services.AddSingleton<INavigator, Navigator>();

        if (!services.Any(x => x.ServiceType == typeof(IPresenter)))
        {
            services.AddSingleton<IPresenter, NullPresenter>();
        }

        return services;
    }

    public static IServiceCollection AddEpocheViewModelPresentationHelpers(this IServiceCollection services, bool scoped = false)
    {
        services.AddTransient(typeof(IViewModelContainer<>), typeof(ViewModelContainerFactory<>));
        if (scoped)
        {
            services.AddScoped<IViewModelManager, ViewModelManager>();
        }
        else
        {
            services.AddSingleton<IViewModelManager, ViewModelManager>();
        }
        return services;
    }
}