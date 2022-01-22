namespace Epoche.MVVM.ViewModels;

public interface IHaveDefaultContent
{
    /// <summary>
    /// Called when the viewmodel is being navigated to (and at the end of the navigation stack).
    /// If a null is return, we're done.  If a type is returned, it's appended to the navigation stack.
    /// </summary>
    Type? GetDefaultViewModelContentType();
}
