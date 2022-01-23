namespace Epoche.MVVM.ViewModels.Navigation;

public interface IPresenter
{
    /// <summary>
    /// The first item in the stack is the app root.
    /// The last item is the final target.
    /// </summary>
    void Present(NavigableViewModelBase[] navigationStack);
}
