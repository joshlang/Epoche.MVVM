﻿using System.Reflection;
using Epoche.MVVM.ViewModels.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.ViewModels;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddViewModelsInAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in ViewModelHelpers.GetViewModelTypes(assembly))
        {
            services.AddTransient(type);
        }

        return services;
    }

    public static IServiceCollection AddEventAggregator(this IServiceCollection services) => services.AddSingleton<IEventAggregator, EventAggregator>();
}