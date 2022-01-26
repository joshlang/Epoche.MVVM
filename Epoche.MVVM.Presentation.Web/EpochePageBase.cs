using System.ComponentModel;
using Epoche.MVVM.ViewModels.Navigation.Presentation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Epoche.MVVM.Presentation.Web;

public abstract class EpochePageBase<TViewModel> : ComponentBase, IDisposable, ITarget<TViewModel>
    where TViewModel : NavigableViewModelBase
{
    protected bool Set<T>(ref T property, T value, in EventCallback<T> propertyChanged, IEqualityComparer<T>? equalityComparer = null)
    {
        if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(property, value))
        {
            return false;
        }

        property = value;
        if (propertyChanged.HasDelegate)
        {
            propertyChanged.InvokeAsync(value).SwallowExceptions();
        }
        return true;
    }

    [Microsoft.AspNetCore.Components.Inject]
    PageHelper PageHelper { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    IViewModelContainer<TViewModel> ViewModelContainer { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    INavigator Navigator { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    IServiceProvider ServiceProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment? Body { get; set; }

    protected override void OnInitialized()
    {
        ViewModelContainer.PropertyChanged += OnPropertyChanged;
        if (ViewModelContainer.ViewModel is null)
        {
            /*
             * This happens when the user first lands on our site, or they use the browser's
             * back/forward button.
             * 
             * So, we need to set up the viewmodels based on the route.
             * 
             * If that succeeds, we re-inject the ViewModelContainer, and it should have
             * a viewmodel.
             */
            if (!RecoverViewModelsFromRoute())
            {
                var shortestPathViewModel = PageHelper.ViewModelTypesByRoute.OrderBy(x => x.Key.Length).Select(x => x.Value).FirstOrDefault();
                if (shortestPathViewModel != null)
                {
                    Navigator.NavigateTo(NavigationType.ReplaceState, shortestPathViewModel);
                }
            }
            ViewModelContainer = ServiceProvider.GetRequiredService<IViewModelContainer<TViewModel>>();

            if (ViewModelContainer.ViewModel is null)
            {
                // If we don't have a viewmodel, we don't want to render anything
                Body = null;
            }
        }
        base.OnInitialized();
    }

    void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModelContainer.ViewModel))
        {
            StateHasChanged();
        }
    }

    protected TViewModel ViewModel => ViewModelContainer.ViewModel ?? throw new InvalidOperationException("ViewModel is null");

    public void Dispose()
    {
        if (ViewModelContainer != null)
        {
            ViewModelContainer.PropertyChanged -= OnPropertyChanged;
        }
        GC.SuppressFinalize(this);
    }

    bool RecoverViewModelsFromRoute()
    {
        var route = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('#')[0];
        while (!string.IsNullOrEmpty(route))
        {
            if (PageHelper.ViewModelTypesByRoute.TryGetValue(route, out var targetType))
            {
                Navigator.NavigateTo(NavigationType.ReplaceState, targetType);
                return true;
            }
            route = route[..(route.LastIndexOf('/') + 1)].TrimEnd('/');
        }
        return false;
    }
}
