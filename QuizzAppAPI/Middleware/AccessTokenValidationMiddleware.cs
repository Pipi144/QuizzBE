using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Middleware;

public class AccessTokenValidationMiddleware(RequestDelegate next, ILogger<AccessTokenValidationMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var excludedPaths = new[] { "/api/auth/register", "/api/auth/login" }; // Paths to exclude
        var requestPath = context.Request.Path.Value;

        if (!excludedPaths.Contains(requestPath, StringComparer.OrdinalIgnoreCase))
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                logger.LogWarning("Authorization header is missing.");
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, new ErrorResponseDTO
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized",
                    Details = "Missing access token."
                });
                return;
            }

            // Check if the Authorization header contains a Bearer token
            if (!authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Invalid Authorization header format.");
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, new ErrorResponseDTO
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized",
                    Details = "Invalid token format."
                });
                return;
            }
        }

        await next(context);
    }

    private async Task WriteErrorResponse(HttpContext context, int statusCode, ErrorResponseDTO errorResponse)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse, options);

        await context.Response.WriteAsync(jsonResponse);
    }
}
