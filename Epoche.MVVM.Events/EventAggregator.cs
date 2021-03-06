namespace Epoche.MVVM.Events;

class EventAggregator : IEventAggregator
{
    readonly Dictionary<Type, PubSubEvent> PubSubEventsByType = new();
    readonly Dictionary<Type, object> GenericPubSubEventsByType = new();
    readonly SynchronizationContext? Context = SynchronizationContext.Current;

    public TEvent GetActionEvent<TEvent>() where TEvent : PubSubEvent, new()
    {
        lock (PubSubEventsByType)
        {
            return (TEvent)(PubSubEventsByType.TryGetValue(typeof(TEvent), out var existingEvent) ?
                existingEvent :
                PubSubEventsByType[typeof(TEvent)] = new TEvent { SynchronizationContext = Context });
        }
    }
    public TEvent GetDataEvent<TEvent>() where TEvent : PubSubEventGenericBase, new()
    {
        lock (GenericPubSubEventsByType)
        {
            return (TEvent)(GenericPubSubEventsByType.TryGetValue(typeof(TEvent), out var existingEvent) ?
                existingEvent :
                GenericPubSubEventsByType[typeof(TEvent)] = new TEvent { SynchronizationContext = Context });
        }
    }
}
