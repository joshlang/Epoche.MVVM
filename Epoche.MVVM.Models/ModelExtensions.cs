namespace Epoche.MVVM.Models;

public static class ModelExtensions
{
    public static TModel With<TModel, TDto>(this TModel model, TDto dto) where TModel : IModelInitializer<TDto>
    {
        model.Initialize(dto);
        return model;
    }
}
