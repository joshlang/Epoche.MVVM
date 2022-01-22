namespace Epoche.MVVM.ViewModels.Events;

public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
{
    static long NextId;
    readonly long Id = Interlocked.Increment(ref NextId);

    Action<SubscriptionToken>? UnsubscribeAction;

    public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
    {
        UnsubscribeAction = unsubscribeAction;
    }

    public bool Equals(SubscriptionToken? other) => Id == other?.Id;
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || Equals(obj as SubscriptionToken);
    public override int GetHashCode() => NextId.GetHashCode();

    public virtual void Dispose()
    {
        Interlocked.Exchange(ref UnsubscribeAction, null)?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}
