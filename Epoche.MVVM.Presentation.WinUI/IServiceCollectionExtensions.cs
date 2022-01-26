using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Presentation.WinUI;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEpocheWinUIPresenter(this IServiceCollection services) => services.AddSingleton<IPresenter, WinUIPresenter>();
    public static IServiceCollection AddEpocheWinUIControls(this IServiceCollection services, params Assembly[] assemblies) => services
        .AddSingleton<IRootComponentHelper, RootComponentHelper>()
        .AddSingleton(new ComponentHelper(assemblies));
    public static IServiceCollection AddEpocheWinUIPageBaseSetup(this IServiceCollection services) => services.AddSingleton<IEpochePageBaseSetup, EpochePageBaseSetup>();
}
