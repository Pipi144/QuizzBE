using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;
using QuizzAppAPI.Service;

namespace QuizzAppAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
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
                var result = await _authService.Login(data.Email, data.Password);

                if (result == null)
                {
                    // Handle errors directly from Auth0
                    var auth0Error = _authService.GetLastAuth0Error();
                    if (auth0Error != null)
                    {
                        // Forward Auth0 status code and error message
                        return StatusCode(auth0Error.StatusCode, auth0Error.Content);
                    }

                    // Fallback if there's no specific error information
                    return BadRequest(new { message = "An unexpected error occurred." });
                }

                return Ok(result);
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

                if (result == null)
                {
                    // Handle errors directly from Auth0
                    var auth0Error = _authService.GetLastAuth0Error();
                    if (auth0Error != null)
                    {
                        // Forward Auth0 status code and error message
                        return StatusCode(auth0Error.StatusCode, auth0Error.Content);
                    }

                    // Fallback if there's no specific error information
                    return BadRequest(new { message = "An unexpected error occurred." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", data.Email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}