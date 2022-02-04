namespace Epoche.MVVM.Events;

public class PubSubEvent
{
    readonly List<EventSubscription> Subscriptions = new();
    public SynchronizationContext? SynchronizationContext { get; internal set; }

    public SubscriptionToken Subscribe(Action action) => Subscribe(action, SubscriptionOption.UI);
    public SubscriptionToken Subscribe(Action action, SubscriptionOption threadOption) => Subscribe(action, threadOption, false);
    public SubscriptionToken Subscribe(Action action, bool keepSubscriberReferenceAlive) => Subscribe(action, SubscriptionOption.UI, keepSubscriberReferenceAlive);
    public SubscriptionToken Subscribe(Action action, SubscriptionOption threadOption, bool keepSubscriberReferenceAlive)
    {
        var actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
        var token = new SubscriptionToken(Unsubscribe);
        var subscription = threadOption switch
        {
            SubscriptionOption.Asynchronous => new AsyncEventSubscription(actionReference, token),
            SubscriptionOption.UI when SynchronizationContext is not null => new UIEventSubscription(actionReference, token, SynchronizationContext),
            _ => new EventSubscription(actionReference, token)
        };
        lock (Subscriptions)
        {
            Subscriptions.Add(subscription);
        }
        return token;
    }

    public void Publish()
    {
        var executionStrategies = PruneAndReturnStrategies();
        foreach (var executionStrategy in executionStrategies)
        {
            executionStrategy();
        }
    }
    public void Unsubscribe(SubscriptionToken token)
    {
        lock (Subscriptions)
        {
            var subscription = Subscriptions.FirstOrDefault(x => x.SubscriptionToken.Equals(token));
            if (subscription is not null)
            {
                Subscriptions.Remove(subscription);
            }
        }
    }
    public void Unsubscribe(Action subscriber)
    {
        lock (Subscriptions)
        {
            Subscriptions.RemoveAll(evt => evt.Action == subscriber);
        }
    }
    List<Action> PruneAndReturnStrategies()
    {
        lock (Subscriptions)
        {
            var returnList = new List<Action>(Subscriptions.Count);
            for (var i = Subscriptions.Count - 1; i >= 0; i--)
            {
                var listItem = Subscriptions[i].GetExecutionStrategy();
                if (listItem is null)
                {
                    Subscriptions.RemoveAt(i);
                }
                else
                {
                    returnList.Add(listItem);
                }
            }
            return returnList;
        }
    }
}

public abstract class PubSubEventGenericBase
{
    internal PubSubEventGenericBase() { }
    public SynchronizationContext? SynchronizationContext { get; internal set; }
}
public class PubSubEvent<TPayload> : PubSubEventGenericBase
{
    readonly List<EventSubscription<TPayload>> Subscriptions = new();

    public SubscriptionToken Subscribe(Action<TPayload> action) => Subscribe(action, SubscriptionOption.Synchronous);
    public SubscriptionToken Subscribe(Action<TPayload> action, Predicate<TPayload> filter) => Subscribe(action, SubscriptionOption.Synchronous, false, filter);
    public SubscriptionToken Subscribe(Action<TPayload> action, SubscriptionOption threadOption) => Subscribe(action, threadOption, false);
    public SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive) => Subscribe(action, SubscriptionOption.Synchronous, keepSubscriberReferenceAlive);
    public SubscriptionToken Subscribe(Action<TPayload> action, SubscriptionOption threadOption, bool keepSubscriberReferenceAlive) => Subscribe(action, threadOption, keepSubscriberReferenceAlive, null);
    public SubscriptionToken Subscribe(Action<TPayload> action, SubscriptionOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload>? filter)
    {
        var actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
        var filterReference = filter is null ? null : new DelegateReference(filter, keepSubscriberReferenceAlive);
        var token = new SubscriptionToken(Unsubscribe);
        var subscription = threadOption switch
        {
            SubscriptionOption.Asynchronous => new BackgroundEventSubscription<TPayload>(actionReference, filterReference, token),
            SubscriptionOption.UI when SynchronizationContext is not null => new DispatcherEventSubscription<TPayload>(actionReference, filterReference, token, SynchronizationContext),
            _ => new EventSubscription<TPayload>(actionReference, filterReference, token)
        };
        lock (Subscriptions)
        {
            Subscriptions.Add(subscription);
        }
        return token;
    }

    public void Publish(TPayload payload)
    {
        var executionStrategies = PruneAndReturnStrategies();
        foreach (var executionStrategy in executionStrategies)
        {
            executionStrategy(payload);
        }
    }
    public void Unsubscribe(SubscriptionToken token)
    {
        lock (Subscriptions)
        {
            var subscription = Subscriptions.FirstOrDefault(x => x.SubscriptionToken.Equals(token));
            if (subscription is not null)
            {
                Subscriptions.Remove(subscription);
            }
        }
    }
    public void Unsubscribe(Action<TPayload> subscriber)
    {
        lock (Subscriptions)
        {
            Subscriptions.RemoveAll(evt => evt.Action == subscriber);
        }
    }
    List<Action<TPayload>> PruneAndReturnStrategies()
    {
        lock (Subscriptions)
        {
            var returnList = new List<Action<TPayload>>(Subscriptions.Count);
            for (var i = Subscriptions.Count - 1; i >= 0; i--)
            {
                var listItem = Subscriptions[i].GetExecutionStrategy();
                if (listItem is null)
                {
                    Subscriptions.RemoveAt(i);
                }
                else
                {
                    returnList.Add(listItem);
                }
            }
            return returnList;
        }
    }
}
