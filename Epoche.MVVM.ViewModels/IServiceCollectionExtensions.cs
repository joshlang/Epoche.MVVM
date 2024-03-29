﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.ViewModels;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddViewModelsInAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var viewModelType in ViewModelHelpers.GetViewModelTypes(assembly))
        {
            services.AddTransient(viewModelType);
        }

        return services;
    }
}
