using System.Diagnostics;

namespace Epoche.MVVM.ViewModels.Navigation;

sealed class NullPresenter : IPresenter
{
    public void Present(ViewModelBase[] navigationStack) => Debug.WriteLine($"Navigation stack: {string.Join(",", navigationStack.Select(x => x.GetType().Name))}");
}
