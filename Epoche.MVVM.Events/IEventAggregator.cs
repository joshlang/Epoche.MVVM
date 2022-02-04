namespace Epoche.MVVM.Events;

// The Prism project was the starting point for all this
public interface IEventAggregator
{
    TEvent GetActionEvent<TEvent>() where TEvent : PubSubEvent, new();
    TEvent GetDataEvent<TEvent>() where TEvent : PubSubEventGenericBase, new();
}
