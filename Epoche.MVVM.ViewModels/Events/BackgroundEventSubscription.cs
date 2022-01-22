namespace Epoche.MVVM.ViewModels.Events;

class BackgroundEventSubscription : EventSubscription
{
    public BackgroundEventSubscription(DelegateReference actionReference, SubscriptionToken subscriptionToken) : base(actionReference, subscriptionToken)
    {
    }

    public override void InvokeAction(Action action) => Task.Run(action);
}

class BackgroundEventSubscription<TPayload> : EventSubscription<TPayload>
{
    public BackgroundEventSubscription(DelegateReference actionReference, DelegateReference? filterReference, SubscriptionToken subscriptionToken) : base(actionReference, filterReference, subscriptionToken)
    {
    }

    public override void InvokeAction(Action<TPayload> action, TPayload argument) => Task.Run(() => action(argument));
}
