using System.Text.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;
using UnauthorizedAccessException = QuizzAppAPI.Models.UnauthorizedAccessException;

namespace QuizzAppAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);  // Process the request
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        
        // Handle different exceptions and map them to appropriate HTTP status codes
        context.Response.StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound, // 404 for not found
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // 401 for unauthorized
            ValidationException => StatusCodes.Status400BadRequest, // 400 for validation errors
            _ => StatusCodes.Status500InternalServerError  // 500 for general errors
        };

        var response = new ErrorResponseDTO
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            Details = exception.StackTrace
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}