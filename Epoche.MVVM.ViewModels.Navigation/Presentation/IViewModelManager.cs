namespace Epoche.MVVM.ViewModels.Navigation.Presentation;

public interface IViewModelManager
{
    IViewModelContainer<TViewModel> GetContainer<TViewModel>() where TViewModel : NavigableViewModelBase;
    void SetNavigationStack(NavigableViewModelBase[] navigationStack);
}