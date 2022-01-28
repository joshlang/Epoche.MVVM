namespace Epoche.MVVM.Events;

class AsyncEventSubscription : EventSubscription
{
    public AsyncEventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken) : base(actionReference, subscriptionToken)
    {
    }

    public override Action? GetExecutionStrategy()
    {
        var a = Action;
        return a is null ? null : () => Task.Run(a);
    }
}

class BackgroundEventSubscription<TPayload> : EventSubscription<TPayload>
{
    public BackgroundEventSubscription(DelegateReference actionReference, DelegateReference? filterReference, SubscriptionToken subscriptionToken) : base(actionReference, filterReference, subscriptionToken)
    {
    }

    public override Action<TPayload>? GetExecutionStrategy()
    {
        var a = Action;
        return a is null ? null : argument => Task.Run(() => a(argument));
    }
}
