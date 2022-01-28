namespace Epoche.MVVM.Events;

class UIEventSubscription : EventSubscription
{
    readonly SynchronizationContext Context;

    public UIEventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken, SynchronizationContext context) : base(actionReference, subscriptionToken)
    {
        Context = context;
    }

    public override Action? GetExecutionStrategy()
    {
        var a = Action;
        return a is null ? null : () => Context.Post(o => a(), null);
    }
}

class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
{
    readonly SynchronizationContext Context;

    public DispatcherEventSubscription(DelegateReference actionReference, DelegateReference? filterReference, SubscriptionToken subscriptionToken, SynchronizationContext context) : base(actionReference, filterReference, subscriptionToken)
    {
        Context = context;
    }

    public override Action<TPayload>? GetExecutionStrategy()
    {
        var a = Action;
        return a is null ? null : argument => Context.Post(o => a((TPayload)o!), argument);
    }
}
