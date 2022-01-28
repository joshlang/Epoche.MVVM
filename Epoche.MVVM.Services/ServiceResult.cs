namespace Epoche.MVVM.Services;

public class ServiceResult
{
    public static readonly ServiceResult Success = new();

    internal ServiceResult() { }
    public ServiceResult(ServiceError error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public ServiceError? Error { get; }

    public bool Ok => Error is null;

    public override string ToString() => Error?.ToString() ?? "Ok";
}

public class ServiceResult<T> : ServiceResult where T : class
{
    ServiceResult() { }
    public ServiceResult(ServiceResult errorServiceResult) : this(errorServiceResult.Error!)
    {
    }
    public ServiceResult(T result)
    {
        NullableResult = result;
    }
    public ServiceResult(ServiceError error) : base(error)
    {
    }

    public T? NullableResult { get; }
    public T Result => Ok ? NullableResult! : throw new InvalidOperationException($"Result cannot be accessed because the service result is in an error state ({Error})");
}
