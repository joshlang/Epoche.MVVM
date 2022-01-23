namespace Epoche.MVVM.ViewModels.Navigation.Presentation;

sealed class ViewModelManager : IViewModelManager
{
    readonly Dictionary<Type, ViewModelContainer> ContainersByType = new();

    public void SetNavigationStack(NavigableViewModelBase[] navigationStack)
    {
        if (navigationStack is null)
        {
            return;
        }
        var newStackByType = navigationStack.ToDictionary(x => x.GetType());
        foreach (var notInNavStack in ContainersByType.Keys.Where(x => !newStackByType.ContainsKey(x)).ToArray())
        {
            ContainersByType[notInNavStack].Dispose();
            ContainersByType.Remove(notInNavStack);
        }
        foreach (var viewModel in navigationStack)
        {
            var viewModelType = viewModel.GetType();
            if (!ContainersByType.TryGetValue(viewModelType, out var container))
            {
                ContainersByType[viewModelType] = container = Activator.CreateInstance(typeof(ViewModelContainer<>).MakeGenericType(viewModelType)) as ViewModelContainer ?? throw new InvalidOperationException();
            }
            container.SetViewModel(viewModel);
        }
    }

    public IViewModelContainer<TViewModel> GetContainer<TViewModel>() where TViewModel : NavigableViewModelBase =>
        (ViewModelContainer<TViewModel>)(
            ContainersByType.TryGetValue(typeof(TViewModel), out var container)
            ? container
            : ContainersByType[typeof(TViewModel)] = Activator.CreateInstance(typeof(ViewModelContainer<>).MakeGenericType(typeof(TViewModel))) as ViewModelContainer ?? throw new InvalidOperationException());
}
