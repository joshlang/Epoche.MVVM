namespace Epoche.MVVM.ViewModels.Navigation;

public abstract class NavigableViewModelBase : ViewModelBase
{
    bool isActivated;
    protected bool IsActivated
    {
        get => isActivated;
        private set
        {
            if (isActivated != value)
            {
                isActivated = value;
                if (value)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }
            }
        }
    }

    internal void Activate() => IsActivated = true;

    internal void Deactivate() => IsActivated = false;

    internal void NavigateTo() => OnNavigatedTo();

    internal void NavigateFrom() => OnNavigatedFrom();

    // The order of lifecycle method calls: OnActivated(), OnNavigatedTo(), OnNavigatedFrom(), OnDeactivated()
    protected virtual void OnActivated() { }

    protected virtual void OnDeactivated() { }

    protected virtual void OnNavigatedTo() { }

    protected virtual void OnNavigatedFrom() { }
}
