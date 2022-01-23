using System.ComponentModel;

namespace Epoche.MVVM.ViewModels.Navigation.Presentation;

/// <summary>
/// This is just a proxy around ViewModelContainer so we can dependency inject the container
/// </summary>
sealed class ViewModelContainerFactory<TViewModel> : IViewModelContainer<TViewModel> where TViewModel : NavigableViewModelBase
{
    readonly IViewModelContainer<TViewModel> Container;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ViewModelContainerFactory(IViewModelManager viewModelManager)
    {
        if (viewModelManager is null)
        {
            throw new ArgumentNullException(nameof(viewModelManager));
        }
        Container = viewModelManager.GetContainer<TViewModel>();
        Container.PropertyChanged += (sender, e) => PropertyChanged?.Invoke(this, e);
    }

    public TViewModel? ViewModel => Container.ViewModel;

    public void Dispose()
    {
        PropertyChanged = null;
        Container.Dispose();
    }
}
