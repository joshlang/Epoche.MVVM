using System.Reflection;
using Epoche.MVVM.ViewModels.Events;

namespace Epoche.MVVM.ViewModels.Navigation;

sealed class Navigator : INavigator
{
    readonly HashSet<Assembly> ScannedAssemblies = new();
    readonly Dictionary<Type, Type> ContainerTypesByType = new();

    readonly IPresenter Presenter;
    readonly IServiceProvider ServiceProvider;

    readonly Stack<NavigableViewModelBase[]> HistoryNavigationStacks = new();
    NavigableViewModelBase[] NavigationStack = Array.Empty<NavigableViewModelBase>();
    readonly Stack<NavigableViewModelBase[]> ForwardNavigationStacks = new();

    public Navigator(
        IPresenter presenter,
        IServiceProvider serviceProvider,
        IEventAggregator eventAggregator)
    {
        if (eventAggregator is null)
        {
            throw new ArgumentNullException(nameof(eventAggregator));
        }

        Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        NavigationStackChangedEvent = eventAggregator.GetEvent<NavigationStackChangedEvent>();
    }

    public NavigationStackChangedEvent NavigationStackChangedEvent { get; }

    public bool CanGoBack() => HistoryNavigationStacks.Count > 0;

    public bool CanGoForward() => ForwardNavigationStacks.Count > 0;

    bool Back()
    {
        if (HistoryNavigationStacks.Count == 0)
        {
            return false;
        }

        var oldNavStack = NavigationStack;
        ForwardNavigationStacks.Push(NavigationStack);
        NavigationStack = HistoryNavigationStacks.Pop();
        HandleActivationsAndNavigations(oldNavStack, NavigationStack);
        NavigationStackChangedEvent.Publish();
        Presenter.Present(NavigationStack);
        return true;
    }

    public bool Forward()
    {
        if (ForwardNavigationStacks.Count == 0)
        {
            return false;
        }

        var oldNavStack = NavigationStack;
        HistoryNavigationStacks.Push(NavigationStack);
        NavigationStack = ForwardNavigationStacks.Pop();
        HandleActivationsAndNavigations(oldNavStack, NavigationStack);
        NavigationStackChangedEvent.Publish();
        Presenter.Present(NavigationStack);
        return true;
    }

    public void NavigateTo<TViewModel>(NavigationType navigationType) where TViewModel : NavigableViewModelBase =>
        NavigateTo(navigationType: navigationType, navigationTarget: typeof(TViewModel));

    public void NavigateTo(NavigationType navigationType, Type navigationTarget)
    {
        if (NavigationStack.LastOrDefault()?.GetType() == navigationTarget)
        {
            NavigationStackChangedEvent.Publish();
            return;
        }

        var oldNavStack = NavigationStack;
        SetupNavStacks(navigationType, navigationTarget);
        HandleActivationsAndNavigations(oldNavStack, NavigationStack);
        NavigationStackChangedEvent.Publish();
        Presenter.Present(NavigationStack);
    }

    void SetupNavStacks(NavigationType navigationType, Type navigationTarget)
    {
        if (!navigationTarget.IsSubclassOf(typeof(NavigableViewModelBase)))
        {
            throw new InvalidOperationException($"{navigationTarget.Name} is not navigable");
        }

        var newNavStack = AppendDefaultContent(GetNavStack(navigationTarget));
        switch (navigationType)
        {
            case NavigationType.NewState:
                HistoryNavigationStacks.Clear();
                ForwardNavigationStacks.Clear();
                break;
            case NavigationType.PushState:
                HistoryNavigationStacks.Push(NavigationStack);
                ForwardNavigationStacks.Clear();
                break;
        }
        NavigationStack = newNavStack;
    }

    static void HandleActivationsAndNavigations(NavigableViewModelBase[] oldNavStack, NavigableViewModelBase[] newNavStack)
    {
        int index;
        for (index = 0; index < oldNavStack.Length; ++index)
        {
            if (index >= newNavStack.Length || !object.ReferenceEquals(oldNavStack[index], newNavStack[index]))
            {
                break;
            }
        }

        if (oldNavStack.Length > 1)
        {
            oldNavStack.Last().NavigateFrom();
        }

        for (var x = index; x < oldNavStack.Length; ++x)
        {
            var viewModel = oldNavStack[x];
            viewModel.Deactivate();
        }

        for (var x = index; x < newNavStack.Length; ++x)
        {
            var viewModel = newNavStack[x];
            viewModel.Activate();
        }

        if (newNavStack.Length > 1)
        {
            newNavStack.Last().NavigateTo();
        }
    }

    NavigableViewModelBase Create(Type viewModelType) => (NavigableViewModelBase?)ServiceProvider.GetService(viewModelType) ?? throw new InvalidOperationException();

    NavigableViewModelBase[] GetNavStack(Type viewModelType)
    {
        for (var x = NavigationStack.Length - 1; x >= 0; --x)
        {
            if (NavigationStack[x].GetType() == viewModelType)
            {
                return NavigationStack
                    .Take(x + 1)
                    .ToArray();
            }
        }
        var canContain = typeof(ICanContain<>).MakeGenericType(viewModelType);
        for (var x = NavigationStack.Length - 1; x >= 0; --x)
        {
            if (canContain.IsAssignableFrom(NavigationStack[x].GetType()))
            {
                return NavigationStack
                    .Take(x + 1)
                    .Concat(new[] { Create(viewModelType) })
                    .ToArray();
            }
        }
        if (GetContainerTypeByType(viewModelType) is Type containerType)
        {
            return GetNavStack(containerType)
                .Concat(new[] { Create(viewModelType) })
                .ToArray();
        }
        return new[] { Create(viewModelType) };
    }

    NavigableViewModelBase[] AppendDefaultContent(NavigableViewModelBase[] navigationStack)
    {
        while (navigationStack.Last() is IHaveDefaultContent haveDefaultContent)
        {
            var contentType = haveDefaultContent.GetDefaultViewModelContentType();
            if (contentType is null)
            {
                break;
            }
            navigationStack = navigationStack
                .Concat(new[] { Create(contentType) })
                .ToArray();
        }
        return navigationStack;
    }

    public NavigableViewModelBase[] GetNavigationStack() => NavigationStack.ToArray();

    public NavigationStackChangedEvent GetNavigationStackChangedEvent() => NavigationStackChangedEvent;

    public void Back<TFallbackViewModel>() where TFallbackViewModel : NavigableViewModelBase
    {
        if (!Back())
        {
            NavigateTo<TFallbackViewModel>(NavigationType.ReplaceState);
        }
    }

    Type? GetContainerTypeByType(Type viewModelType)
    {
        if (ContainerTypesByType.TryGetValue(viewModelType, out var containerType))
        {
            return containerType;
        }

        var assembly = viewModelType.Assembly;
        if (!ScannedAssemblies.Add(assembly))
        {
            return null;
        }

        foreach (var pair in ViewModelHelpers.GetViewModelTypes(assembly)
            .Select(x => new
            {
                CanContain = x
                    .GetGenericInterfaces(typeof(ICanContain<>))
                    .Select(i => i.GenericTypeArguments[0]),
                Type = x
            })
            .SelectMany(x => x.CanContain.Select(c => new
            {
                x.Type,
                CanContain = c
            })))
        {
            ContainerTypesByType[pair.CanContain] = pair.Type;
        }

        ContainerTypesByType.TryGetValue(viewModelType, out containerType);
        return containerType;
    }
}
