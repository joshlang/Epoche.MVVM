namespace Epoche.MVVM.ViewModels.Events;

class EventSubscription
{
    readonly DelegateReference ActionReference;

    public EventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken)
    {
        ActionReference = actionReference ?? throw new ArgumentNullException(nameof(actionReference));
        SubscriptionToken = subscriptionToken ?? throw new ArgumentNullException(nameof(subscriptionToken));
    }

    internal Action? Action => (Action?)ActionReference.Target;
    public SubscriptionToken SubscriptionToken { get; set; }

    public virtual Action? GetExecutionStrategy() => Action;
}

class EventSubscription<TPayload>
{
    readonly DelegateReference ActionReference;
    readonly DelegateReference? FilterReference;
    public EventSubscription(DelegateReference actionReference, DelegateReference? filterReference, SubscriptionToken subscriptionToken)
    {
        ActionReference = actionReference ?? throw new ArgumentNullException(nameof(actionReference));
        FilterReference = filterReference;
        SubscriptionToken = subscriptionToken ?? throw new ArgumentNullException(nameof(subscriptionToken));
    }

    internal Action<TPayload>? Action => (Action<TPayload>?)ActionReference.Target;
    public SubscriptionToken SubscriptionToken { get; set; }

    public virtual Action<TPayload>? GetExecutionStrategy()
    {
        var action = Action;
        if (action is null) { return null; }
        var filter = (Predicate<TPayload>?)FilterReference?.Target;
        if (filter is null) { return action; }
        return x =>
        {
            if (filter?.Invoke(x) == false) { return; }
            action(x);
        };
    }
}
