namespace Epoche.MVVM.ViewModels.Navigation.Presentation;
class Presenter : IPresenter
{
    readonly IViewModelManager ViewModelManager;

    public Presenter(IViewModelManager viewModelManager)
    {
        ViewModelManager = viewModelManager ?? throw new ArgumentNullException(nameof(viewModelManager));
    }

    public void Present(NavigableViewModelBase[] navigationStack) => ViewModelManager.SetNavigationStack(navigationStack);
}
