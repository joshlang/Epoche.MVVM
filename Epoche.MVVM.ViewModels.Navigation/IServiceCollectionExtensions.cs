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
}