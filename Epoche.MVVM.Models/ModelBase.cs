using System.Runtime.CompilerServices;

namespace Epoche.MVVM.Models;

public abstract class ModelBase : INotifyPropertyChanged, IRevertibleChangeTracking
{
    static readonly PropertyChangedEventArgs IsChangedPropertyChangedEventArgs = new(nameof(IsChanged));

    public event PropertyChangedEventHandler? PropertyChanged;
    readonly Dictionary<string, object?> OriginalValues = new();

    bool isChanged;
    public bool IsChanged
    {
        get => isChanged;
        private set => Set(ref isChanged, value, trackChanges: false, cachedPropertyChangedEventArgs: IsChangedPropertyChangedEventArgs);
    }

    public void AcceptChanges()
    {
        OriginalValues.Clear();
        IsChanged = false;
    }

    public void RejectChanges()
    {
        var thisType = GetType();
        foreach (var original in OriginalValues.ToArray())
        {
            thisType.GetProperty(original.Key)?.SetValue(this, original.Value);
        }
        AcceptChanges();
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected void RaisePropertiesChanged(params string[] propertyNames)
    {
        var pc = PropertyChanged;
        if (pc is not null)
        {
            foreach (var propertyName in propertyNames)
            {
                pc(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    protected void RaisePropertyChanged(PropertyChangedEventArgs property) => PropertyChanged?.Invoke(this, property);

    protected void RaisePropertiesChanged(params PropertyChangedEventArgs[] properties)
    {
        var pc = PropertyChanged;
        if (pc is not null)
        {
            foreach (var property in properties)
            {
                pc(this, property);
            }
        }
    }
        
    protected bool Set<T>(
        ref T property,
        T value,
        IEqualityComparer<T>? equalityComparer = null,
        Action? onChange = null,
        [CallerMemberName] string? propertyName = null,
        bool trackChanges = true,
        PropertyChangedEventArgs? cachedPropertyChangedEventArgs = null)
    {
        equalityComparer ??= EqualityComparer<T>.Default;
        if (equalityComparer.Equals(property, value))
        {
            return false;
        }

#if false
            string DebugToString(object? o) => o is null ? "[null]" : o.ToString().StartsWith("Symetria.") ? o.GetType().Name : o.ToString();
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: {GetType().Name}.{propertyName} changing {DebugToString(property)} -> {DebugToString(value)}");
#endif

        if (trackChanges && propertyName is not null)
        {
            if (OriginalValues.TryGetValue(propertyName, out var original))
            {
                if (equalityComparer.Equals((T)original!, value))
                {
                    OriginalValues.Remove(propertyName);
                }
            }
            else
            {
                OriginalValues[propertyName] = property;
            }
        }

        property = value;
        onChange?.Invoke();
        if (cachedPropertyChangedEventArgs is not null)
        {
            RaisePropertyChanged(cachedPropertyChangedEventArgs);
        }
        else
        {
            RaisePropertyChanged(propertyName);
        }
        if (trackChanges)
        {
            IsChanged = OriginalValues.Count > 0;
        }
        return true;
    }
}
