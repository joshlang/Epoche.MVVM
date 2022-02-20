using Epoche.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Epoche.MVVM.Presentation.WinUI;
public abstract class EpocheUserControlBase<TViewModel> : UserControl where TViewModel : ViewModelBase
{
    public TViewModel ViewModel { get; } = EpocheServiceProviderBaseSetup.ServiceProvider?.GetRequiredService<TViewModel>() ?? throw new InvalidOperationException($"Use {nameof(IEpocheServiceProviderSetup)}.{nameof(IEpocheServiceProviderSetup.SetServiceProvider)} before using {nameof(EpocheUserControlBase<ViewModelBase>)}");
}
