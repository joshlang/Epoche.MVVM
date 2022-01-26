using System.ComponentModel;

namespace Epoche.MVVM.Presentation.WinUI;
public class ChildComponentContainer : INotifyPropertyChanged
{
    static readonly PropertyChangedEventArgs ChildComponentChangedArgs = new(nameof(ChildComponent));

    public event PropertyChangedEventHandler? PropertyChanged;

    object? childComponent;
    public object? ChildComponent
    {
        get => childComponent;
        internal set
        {
            if (ReferenceEquals(childComponent, value))
            {
                return;
            }
            childComponent = value;
            PropertyChanged?.Invoke(this, ChildComponentChangedArgs);
        }
    }
}
