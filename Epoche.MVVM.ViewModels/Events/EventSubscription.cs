namespace Epoche.MVVM.ViewModels.Events;

class EventSubscription
{
    protected readonly DelegateReference ActionReference;

    public EventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken)
    {
        ActionReference = actionReference ?? throw new ArgumentNullException(nameof(actionReference));
        SubscriptionToken = subscriptionToken ?? throw new ArgumentNullException(nameof(subscriptionToken));
    }

    public Action? Action => (Action?)ActionReference.Target;
    public SubscriptionToken SubscriptionToken { get; set; }

    public Action? GetExecutionStrategy() => Action;

    public virtual void InvokeAction(Action action) => action?.Invoke();
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

    public Action<TPayload>? Action => (Action<TPayload>?)ActionReference.Target;
    public SubscriptionToken SubscriptionToken { get; set; }

    public Action<TPayload>? GetExecutionStrategyWithPayload()
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

    public virtual void InvokeAction(Action<TPayload> action, TPayload argument) => action?.Invoke(argument);
}
