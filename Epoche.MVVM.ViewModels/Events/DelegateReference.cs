using System.Reflection;

namespace Epoche.MVVM.ViewModels.Events;

class DelegateReference
{
    readonly Delegate? _Delegate;
    readonly WeakReference? WeakReference;
    readonly MethodInfo? Method;
    readonly Type? DelegateType;

    public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
    {
        if (@delegate is null)
        {
            throw new ArgumentNullException(nameof(@delegate));
        }

        if (keepReferenceAlive)
        {
            _Delegate = @delegate;
        }
        else
        {
            Method = @delegate.GetMethodInfo();
            if (Method.IsStatic)
            {
                _Delegate = Method.CreateDelegate(DelegateType!, null);
            }
            else
            {
                WeakReference = new WeakReference(@delegate.Target);
                DelegateType = @delegate.GetType();
            }
        }
    }

    public Delegate? Target
    {
        get
        {
            if (_Delegate is not null)
            {
                return _Delegate;
            }
            var target = WeakReference!.Target;
            if (target is not null)
            {
                return Method!.CreateDelegate(DelegateType!, target);
            }
            return null;
        }
    }
}
