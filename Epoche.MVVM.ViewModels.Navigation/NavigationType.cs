namespace Epoche.MVVM.ViewModels.Navigation;

public enum NavigationType
{
    /// <summary>
    /// Going "Back" will return to the current state
    /// </summary>
    PushState,
    /// <summary>
    /// Going "Back" will return to the state before the current state (ie, the current state is not saved)
    /// </summary>
    ReplaceState,
    /// <summary>
    /// Clears all state history - you cannot go "Back"
    /// </summary>
    NewState
}
