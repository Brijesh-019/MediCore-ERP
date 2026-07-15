using System.Net;
using System.Text.Json;
using MediCore.Shared.Responses;

namespace MediCore.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse.Fail(
            "An unexpected error occurred.",
            context.Response.StatusCode
        );

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}