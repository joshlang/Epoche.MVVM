namespace Epoche.MVVM.Models;

public interface IModelInitializer<TDto>
{
    void Initialize(TDto dto);
}
