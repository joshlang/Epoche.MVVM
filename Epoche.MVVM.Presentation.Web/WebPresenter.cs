using System.Diagnostics;
using Epoche.MVVM.ViewModels.Navigation.Presentation;
using Microsoft.AspNetCore.Components;

namespace Epoche.MVVM.Presentation.Web;

sealed class WebPresenter : IPresenter
{
    readonly NavigationManager NavigationManager;
    readonly IViewModelManager ViewModelManager;
    readonly PageHelper PageHelper;

    public WebPresenter(
        NavigationManager navigationManager,
        IViewModelManager viewModelManager,
        PageHelper pageHelper)
    {
        NavigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        ViewModelManager = viewModelManager ?? throw new ArgumentNullException(nameof(viewModelManager));
        PageHelper = pageHelper ?? throw new ArgumentNullException(nameof(pageHelper));
    }

    [Conditional("DEBUG")]
    static void DumpStack(NavigableViewModelBase[] navigationStack) => Debug.WriteLine("Presenting: " + string.Join(" -> ", navigationStack.Select(x => x.GetType().Name)));

    public void Present(NavigableViewModelBase[] navigationStack)
    {
        DumpStack(navigationStack);

        var presentationType = navigationStack.LastOrDefault()?.GetType();
        if (presentationType != null &&
            PageHelper.RoutesByViewModelType.TryGetValue(presentationType, out var route))
        {
            /*
             * At this point, we might be at the same route as where we've been told to present.
             * This happens on initial navigation, and when the user uses back/forward browser
             * buttons.
             * 
             * Navigation is called to recover the correct viewmodels, and we want to set it
             * all up but *not* call NavigateTo on the navigation manager.
             * 
             * Or, if we're not on the requested route, we do navigate.
             */

            ViewModelManager.SetNavigationStack(navigationStack);
            if (route.Trim('/') != NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('#')[0])
            {
                NavigationManager.NavigateTo(route);
            }
        }
        else
        {
            Debug.WriteLine($"No route found to present navigation stack: {string.Join(",", navigationStack.EmptyIfNull().Select(x => x?.GetType().Name))}");
        }
    }
}
