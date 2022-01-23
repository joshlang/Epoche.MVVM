using System.ComponentModel;

namespace Epoche.MVVM.ViewModels.Navigation.Presentation;

public interface IViewModelContainer<TViewModel> : IDisposable, INotifyPropertyChanged where TViewModel : NavigableViewModelBase
{
    TViewModel? ViewModel { get; }
}
