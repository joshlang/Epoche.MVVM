using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Events;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEventAggregator(this IServiceCollection services) => services.AddSingleton<IEventAggregator, EventAggregator>();
}
