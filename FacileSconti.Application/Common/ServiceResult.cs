namespace FacileSconti.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; }

    public static ServiceResult<T> Ok(T data, string message = "") => new() { Success = true, Data = data, Message = message };
    public static ServiceResult<T> Fail(string message) => new() { Success = false, Message = message };
}
