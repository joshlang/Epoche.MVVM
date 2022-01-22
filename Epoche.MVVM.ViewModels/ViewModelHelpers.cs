using System.Reflection;

namespace Epoche.MVVM.ViewModels;

public static class ViewModelHelpers
{
    public static Type[] GetViewModelTypes(Assembly assembly) =>
        assembly
        .GetTypes()
        .Where(x => !x.IsAbstract)
        .Where(x => x.IsSubclassOf(typeof(ViewModelBase)))
        .ToArray();
}
