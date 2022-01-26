using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Presentation.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEpocheWebPresenter(this IServiceCollection services) => services.AddSingleton<IPresenter, WebPresenter>();
    public static IServiceCollection AddEpocheWebPages(this IServiceCollection services, params Assembly[] pageAssemblies) => services.AddSingleton(new PageHelper(pageAssemblies));
}
