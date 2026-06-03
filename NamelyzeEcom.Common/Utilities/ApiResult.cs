namespace NamelyzeEcom.Common.Utilities;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResult<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ApiResult<T> { Success = true, Message = message, Data = data };
    }

    public static ApiResult<T> FailureResult(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResult<T> { Success = false, Message = message, Errors = errors };
    }
}
