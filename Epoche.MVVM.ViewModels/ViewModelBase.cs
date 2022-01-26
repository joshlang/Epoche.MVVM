using Epoche.MVVM.Models;

namespace Epoche.MVVM.ViewModels;

public abstract partial class ViewModelBase : ModelBase
{
    protected ViewModelBase()
    {
        this.OnInitialize();
    }

    protected virtual void OnInitialize() { }
}
