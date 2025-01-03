using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IAuthService authService, ILogger<AuthController> logger, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("login")]
        
        public async Task<IActionResult> Login([FromBody] LoginDto data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return HandleError("Validation failed", StatusCodes.Status400BadRequest, string.Join("; ", errors));
            }

            try
            {
                var result = await _authService.Login(data);

                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var auth0Error = _authService.GetLastAuth0Error();
                return auth0Error != null
                    ? HandleError(auth0Error.Content, auth0Error.StatusCode)
                    : HandleError("An unexpected error occurred.", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {Email}", data.Email);
                return HandleError("Internal server error", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return HandleError("Validation failed", StatusCodes.Status400BadRequest,
                    string.Join("; ", errors));
            }

            try
            {
                var result = await _authService.Register(data);

                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var auth0Error = _authService.GetLastAuth0Error();
                return auth0Error != null
                    ? HandleError(auth0Error.Content, auth0Error.StatusCode)
                    : HandleError("An unexpected error occurred.", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", data.Email);
                return HandleError("Internal server error", StatusCodes.Status500InternalServerError);
            }
        }
    }
}