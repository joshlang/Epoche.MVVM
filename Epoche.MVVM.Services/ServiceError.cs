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
    }
    public ServiceError(Exception exception, string message) : this(message)
    {
        Exception = exception;
    }

    public string Message { get; }
    public Exception? Exception { get; }

    public override string ToString() => Exception is null ? Message : $"{Exception.GetType().Name}: {Message}";
}
