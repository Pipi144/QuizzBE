using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Controllers;

public abstract class ApiControllerBase: ControllerBase
{
    protected IActionResult HandleError(string message, int statusCode, string? details = null)
    {
        var errorResponse = new ErrorResponseDTO()
        {
            Message = message,
            StatusCode = statusCode,
            Details = details
        };

        return StatusCode(statusCode, errorResponse);
    }
}