namespace Epoche.MVVM.Services;
public sealed class EpocheServiceException : Exception
{
    public readonly ServiceError Error;
    public EpocheServiceException(ServiceError error) : base(error.Message, error.Exception)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
