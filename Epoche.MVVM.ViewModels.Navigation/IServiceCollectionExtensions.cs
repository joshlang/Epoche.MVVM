using Epoche.MVVM.ViewModels.Navigation.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.ViewModels.Navigation;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEpocheViewModelNavigator(this IServiceCollection services) => services.AddSingleton<INavigator, Navigator>();

    public static IServiceCollection AddEpocheViewModelNullPresenter(this IServiceCollection services, bool overwriteExistingPresenter)
    {
        if (overwriteExistingPresenter && services.Any(x => x.ServiceType == typeof(IPresenter)))
        {
            return services;
        }
        return services.AddSingleton<IPresenter, NullPresenter>();
    }
    
    public static IServiceCollection AddEpocheViewModelPresenter(this IServiceCollection services) => services.AddSingleton<IPresenter, Presenter>();

    public static IServiceCollection AddEpocheViewModelPresentationHelpers(this IServiceCollection services) => services
        .AddTransient(typeof(IViewModelContainer<>), typeof(ViewModelContainerFactory<>))
        .AddSingleton<IViewModelManager, ViewModelManager>();
}