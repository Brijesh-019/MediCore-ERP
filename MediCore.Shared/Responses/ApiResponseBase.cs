namespace MediCore.Shared.Responses;

public class ApiResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public static ApiResponse Ok(
        string message = "Request completed successfully.",
        int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResponse Fail(
        string message,
        int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}