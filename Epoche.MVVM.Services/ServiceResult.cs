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
    public static implicit operator ServiceResult(Exception e) => new(e);
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
    public static implicit operator ServiceResult<T>(Exception e) => new((ServiceError)e);
    public static implicit operator ServiceResult<T>(T result) => new(result);
}

public class ServiceValueResult<T> : ServiceResult where T : struct
{
    ServiceValueResult() { }
    public ServiceValueResult(ServiceResult errorServiceResult) : this(errorServiceResult.Error!)
    {
    }
    public ServiceValueResult(T result)
    {
        NullableResult = result;
    }
    public ServiceValueResult(ServiceError error) : base(error)
    {
    }

    public T? NullableResult { get; }
    public T Result => Ok ? NullableResult!.Value : throw new InvalidOperationException($"Result cannot be accessed because the service result is in an error state ({Error})");
    public static implicit operator ServiceValueResult<T>(Exception e) => new((ServiceError)e);
    public static implicit operator ServiceValueResult<T>(T result) => new(result);
}
