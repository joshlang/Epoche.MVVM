using System.ComponentModel;

namespace Epoche.MVVM.ViewModels.Navigation.Presentation;

abstract class ViewModelContainer : INotifyPropertyChanged, IDisposable
{
    static readonly PropertyChangedEventArgs ViewModelEventArgs = new(nameof(ViewModelContainer<NavigableViewModelBase>.ViewModel));

    public event PropertyChangedEventHandler? PropertyChanged;

    protected ViewModelBase? CurrentViewModel { get; private set; }

    public void SetViewModel(ViewModelBase? viewModel)
    {
        if (object.ReferenceEquals(viewModel, CurrentViewModel))
        {
            return;
        }
        CurrentViewModel = viewModel;
        PropertyChanged?.Invoke(this, ViewModelEventArgs);
    }

    public void Dispose() => PropertyChanged = null;
}

sealed class ViewModelContainer<TViewModel> :
    ViewModelContainer,
    IViewModelContainer<TViewModel>
    where TViewModel : NavigableViewModelBase
{
    public TViewModel? ViewModel => (TViewModel?)CurrentViewModel;
}
