namespace Epoche.MVVM.Models.Validation;

public sealed class ValidatingModelErrors : ModelBase
{
    static readonly PropertyChangedEventArgs[] ErrorsPropertyChangedEventArgs = new[]
    {
        new PropertyChangedEventArgs(nameof(Errors)),
        new PropertyChangedEventArgs(nameof(Error))
    };

    string[] errors = Array.Empty<string>();
    public string[] Errors
    {
        get => errors;
        set
        {
            errors = value ?? Array.Empty<string>();
            RaisePropertiesChanged(ErrorsPropertyChangedEventArgs);
        }
    }
    public string? Error => errors.FirstOrDefault();
}
