namespace MediCore.Shared.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(
        T? data,
        string message = "Request completed successfully.",
        int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Fail(
        string message,
        int statusCode = 400,
        T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }
}