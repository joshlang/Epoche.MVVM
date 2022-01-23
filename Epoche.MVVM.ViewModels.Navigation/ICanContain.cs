namespace Epoche.MVVM.ViewModels.Navigation;

/// <summary>
/// Each TViewModel can be referenced by zero or one containers which ICanContain the TViewModel
/// </summary>
public interface ICanContain<TViewModel> where TViewModel : NavigableViewModelBase
{
}
