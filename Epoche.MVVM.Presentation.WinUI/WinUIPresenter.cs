using Epoche.MVVM.ViewModels.Navigation.Presentation;

namespace Epoche.MVVM.Presentation.WinUI;
class WinUIPresenter : IPresenter
{
    readonly IViewModelManager ViewModelManager;
    readonly IRootComponentHelper RootComponentHelper;
    readonly ComponentHelper ComponentHelper;

    public WinUIPresenter(
        ComponentHelper componentHelper,
        IRootComponentHelper rootComponentHelper,
        IViewModelManager viewModelManager)
    {
        ComponentHelper = componentHelper ?? throw new ArgumentNullException(nameof(componentHelper));
        RootComponentHelper = rootComponentHelper ?? throw new ArgumentNullException(nameof(rootComponentHelper));
        ViewModelManager = viewModelManager ?? throw new ArgumentNullException(nameof(viewModelManager));
    }


    readonly Dictionary<Type, ITarget> TargetsByViewModelType = new();

    public void Present(NavigableViewModelBase[] navigationStack)
    {
        ViewModelManager.SetNavigationStack(navigationStack);

        var currentStack = navigationStack.Select(x => x.GetType()).ToHashSet();

        foreach (var old in TargetsByViewModelType.Where(x => !currentStack.Contains(x.Key)).ToList())
        {
            TargetsByViewModelType[old.Key].ChildComponentContainer.ChildComponent = null;
            TargetsByViewModelType.Remove(old.Key);
        }
        foreach (var type in currentStack.Where(x => !TargetsByViewModelType.ContainsKey(x)).ToList())
        {
            ComponentHelper.ComponentTypesByViewModel.TryGetValue(type, out var componentType);
            if (componentType is null)
            {
                throw new InvalidOperationException($"No component type is known which implements ITarget<{type.Name}>");
            }
            TargetsByViewModelType[type] = Activator.CreateInstance(componentType) as ITarget ?? throw new InvalidOperationException($"Type {componentType.Name} could not be activated for type {type.Name}");
        }
        if (navigationStack.Length > 0)
        {
            TargetsByViewModelType[navigationStack[^1].GetType()].ChildComponentContainer.ChildComponent = null;
        }
        for (var x = navigationStack.Length - 2; x >= 0; --x)
        {
            TargetsByViewModelType[navigationStack[x].GetType()].ChildComponentContainer.ChildComponent = TargetsByViewModelType[navigationStack[x + 1].GetType()];
        }
        RootComponentHelper.GetRootContainer().ChildComponent = navigationStack.Length == 0 ? null : TargetsByViewModelType[navigationStack[0].GetType()];
    }
}
