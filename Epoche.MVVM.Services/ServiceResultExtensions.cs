namespace Epoche.MVVM.Services;
public static class ServiceResultExtensions
{
    public static async Task ThrowOnError(this Task<ServiceResult> serviceTask)
    {
        var result = await serviceTask.ConfigureAwait(false);
        if (result.Error is not null)
        {
            throw new EpocheServiceException(result.Error);
        }
    }
    public static async Task<T> ThrowOnError<T>(this Task<ServiceResult<T>> serviceTask) where T : class
    {
        var result = await serviceTask.ConfigureAwait(false);
        if (result.Error is not null)
        {
            throw new EpocheServiceException(result.Error);
        }
        return result.Result;
    }
    public static async Task<T?> ThrowOnErrorAllowingNull<T>(this Task<ServiceResult<T>> serviceTask) where T : class
    {
        var result = await serviceTask.ConfigureAwait(false);
        if (result.Error is not null)
        {
            throw new EpocheServiceException(result.Error);
        }
        return result.NullableResult;
    }
    public static async Task<T> ThrowOnError<T>(this Task<ServiceValueResult<T>> serviceTask) where T : struct
    {
        var result = await serviceTask.ConfigureAwait(false);
        if (result.Error is not null)
        {
            throw new EpocheServiceException(result.Error);
        }
        return result.Result;
    }
    public static async Task<T?> ThrowOnErrorAllowingNull<T>(this Task<ServiceValueResult<T>> serviceTask) where T : struct
    {
        var result = await serviceTask.ConfigureAwait(false);
        if (result.Error is not null)
        {
            throw new EpocheServiceException(result.Error);
        }
        return result.NullableResult;
    }
}
