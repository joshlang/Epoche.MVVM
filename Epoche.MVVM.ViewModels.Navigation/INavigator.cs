namespace Epoche.MVVM.ViewModels.Navigation;

public interface INavigator
{
    NavigationStackChangedEvent NavigationStackChangedEvent { get; }

    void NavigateTo<TViewModel>(NavigationType navigationType) where TViewModel : NavigableViewModelBase;
    void NavigateTo(NavigationType navigationType, Type navigationTarget);

    bool CanGoBack();
    bool CanGoForward();
    bool Forward();

    /// <summary>
    /// Go back, or if that fails, navigate to the fallback viewmodel.
    /// </summary>
    void Back<TFallbackViewModel>() where TFallbackViewModel : NavigableViewModelBase;

    NavigableViewModelBase[] GetNavigationStack();
}
