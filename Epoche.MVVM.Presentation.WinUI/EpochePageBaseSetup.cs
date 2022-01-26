namespace Epoche.MVVM.Presentation.WinUI;
sealed class EpochePageBaseSetup : IEpochePageBaseSetup
{
    public static IServiceProvider? ServiceProvider;
    public void SetServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
}
