namespace Epoche.MVVM.Presentation.WinUI;
class RootComponentHelper : IRootComponentHelper
{
    readonly ChildComponentContainer Container = new();
    public ChildComponentContainer GetRootContainer() => Container;
}
