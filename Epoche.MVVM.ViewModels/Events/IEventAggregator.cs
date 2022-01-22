namespace Epoche.MVVM.ViewModels.Events;

// The Prism project was the starting point for all this
public interface IEventAggregator
{
    TEvent GetEvent<TEvent>() where TEvent : PubSubEvent, new();
    TEvent GetEvent<TEvent, T>() where TEvent : PubSubEvent<T>, new();
}
