namespace Epoche.MVVM.ViewModels.Events;

class DispatcherEventSubscription : EventSubscription
{
    readonly SynchronizationContext Context;

    public DispatcherEventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken, SynchronizationContext context) : base(actionReference, subscriptionToken)
    {
        Context = context;
    }

    public override void InvokeAction(Action action) => Context.Post(o => action(), null);
}

class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
{
    readonly SynchronizationContext Context;

    public DispatcherEventSubscription(DelegateReference actionReference, DelegateReference? filterReference, SubscriptionToken subscriptionToken, SynchronizationContext context) : base(actionReference, filterReference, subscriptionToken)
    {
        Context = context;
    }

    public override void InvokeAction(Action<TPayload> action, TPayload argument) => Context.Post(o => action((TPayload)o!), argument);
}
