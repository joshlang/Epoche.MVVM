namespace Epoche.MVVM.Models;
public interface IModelFactory<TModel> where TModel : ModelBase
{
    TModel Create();
}
