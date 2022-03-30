namespace Epoche.MVVM.Services;
public class ServiceError
{
    public ServiceError(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
    public ServiceError(Exception exception) : this(exception.Message ?? exception.GetType().Name)
    {
        Exception = exception;
        if (exception is EpocheServiceException ese)
        {
            Exception = ese.Error.Exception;
            Message = ese.Error.Message;
        }
    }
    public ServiceError(Exception exception, string message) : this(message)
    {
        Exception = exception;
        if (exception is EpocheServiceException ese)
        {
            Exception = ese.Error.Exception;
        }
    }

    public string Message { get; }
    public Exception? Exception { get; }

    public override string ToString() => Exception is null ? Message : $"{Exception.GetType().Name}: {Message}";

    public static implicit operator ServiceError(Exception e) => e is EpocheServiceException ese ? ese.Error : new(e);
}
