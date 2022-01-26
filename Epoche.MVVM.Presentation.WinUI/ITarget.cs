using Epoche.MVVM.ViewModels.Navigation.Presentation;

namespace Epoche.MVVM.Presentation.WinUI;

public interface ITarget
{
    public ChildComponentContainer ChildComponentContainer { get; }
}
public interface ITarget<TViewModel> : ITarget where TViewModel : NavigableViewModelBase
{
    public IViewModelContainer<TViewModel> ViewModelContainer { get; }
}
