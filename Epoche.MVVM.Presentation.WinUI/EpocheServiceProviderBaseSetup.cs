namespace Epoche.MVVM.Presentation.WinUI;
sealed class EpocheServiceProviderBaseSetup : IEpocheServiceProviderSetup
{
    public static IServiceProvider? ServiceProvider;
    public void SetServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
}
