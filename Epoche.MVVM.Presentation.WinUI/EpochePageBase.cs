using Epoche.MVVM.ViewModels.Navigation.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Epoche.MVVM.Presentation.WinUI;

public abstract class EpochePageBase<TViewModel> : Page, ITarget<TViewModel> where TViewModel : NavigableViewModelBase
{
    public IViewModelContainer<TViewModel> ViewModelContainer { get; } = EpocheServiceProviderBaseSetup.ServiceProvider?.GetRequiredService<IViewModelContainer<TViewModel>>() ?? throw new InvalidOperationException($"Use {nameof(IEpocheServiceProviderSetup)}.{nameof(IEpocheServiceProviderSetup.SetServiceProvider)} before using {nameof(EpochePageBase<NavigableViewModelBase>)}");
    public ChildComponentContainer ChildComponentContainer { get; } = new();
}
