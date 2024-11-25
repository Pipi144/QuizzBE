using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IAuthService authService, ILogger<AuthController> logger, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.Login(data);

                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var auth0Error = _authService.GetLastAuth0Error();
                return auth0Error != null ?
                    // Forward Auth0 status code and error message
                    StatusCode(auth0Error.StatusCode, auth0Error.Content) :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {Email}", data.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.Register(data);

                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var auth0Error = _authService.GetLastAuth0Error();
                return auth0Error != null ?
                    // Forward Auth0 status code and error message
                    StatusCode(auth0Error.StatusCode, auth0Error.Content) :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", data.Email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}