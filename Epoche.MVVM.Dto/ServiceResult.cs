//namespace Epoche.MVVM.Dto;

//public class ServiceResult
//{
//    public static readonly ServiceResult Success = new();

//    internal ServiceResult() { }
//    public ServiceResult(int code, string message)
//    {
//        ErrorCode = code;
//        ErrorMessage = message ?? throw new ArgumentNullException(nameof(code));
//    }

//    public int? ErrorCode { get; }
//    public string? ErrorMessage { get; }

//    public bool Ok => !ErrorCode.HasValue;

//    public override string ToString() => Ok ? "Ok" : $"({ErrorCode}) {ErrorMessage}";
//}

//public sealed class ServiceResult<T> : ServiceResult where T : class
//{
//    ServiceResult() { }
//    public ServiceResult(ServiceResult errorServiceResult) : base(
//        errorServiceResult.ErrorCode ?? throw new ArgumentOutOfRangeException(nameof(errorServiceResult), "Service result is not in an error state"),
//        errorServiceResult.ErrorMessage!)
//    {
//    }
//    public ServiceResult(T result)
//    {
//        NullableResult = result;
//    }
//    public ServiceResult(int code, string message) : base(code, message)
//    {
//    }

//    public T Result => NullableResult ?? throw new InvalidOperationException();
//    public T? NullableResult { get; }
//}
