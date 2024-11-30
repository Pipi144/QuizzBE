using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase

    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;


        public UserController(IUserService userService, ILogger<AuthController> logger, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("current-user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var accessToken = _tokenService.ExtractAccessToken(Request.Headers);

                if (accessToken == null)
                    return Unauthorized("Access token is missing or invalid.");
                var result = await _userService.GetCurrentUserInfo(accessToken);
                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var userServiceError = _userService.GetLastUserServiceError();
                return userServiceError != null ?
                    // Forward Auth0 status code and error message
                    StatusCode(userServiceError.StatusCode, userServiceError.Content) :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var accessToken = _tokenService.ExtractAccessToken(Request.Headers);

                if (accessToken == null)
                    return Unauthorized("Access token is missing or invalid.");
                var result = await _userService.DeleteUser(id, accessToken);
                if (result != false) return NoContent();
                // Handle errors directly from Auth0
                var userServiceError = _userService.GetLastUserServiceError();
                return userServiceError != null ?
                    // Forward Auth0 status code and error message
                    StatusCode(userServiceError.StatusCode, userServiceError.Content) :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user-roles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var result = await _userService.GetAllRoles();
                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var userServiceError = _userService.GetLastUserServiceError();
                return userServiceError != null
                    ?
                    // Forward Auth0 status code and error message
                    StatusCode(userServiceError.StatusCode, userServiceError.Content)
                    :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser(UserDTO.AssignUserRoleDTO data)
        {
            try
            {
                var result = await _userService.AssignUserRole(data);
                if (result) return NoContent();
                // Handle errors directly from Auth0
                var userServiceError = _userService.GetLastUserServiceError();
                return userServiceError != null
                    ?
                    // Forward Auth0 status code and error message
                    StatusCode(userServiceError.StatusCode, userServiceError.Content)
                    :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery]UserDTO.GetUserListParamsDTO param)
        {
            try
            {
                var result = await _userService.GetAllUsers(param);
                if (result != null) return Ok(result);
                // Handle errors directly from Auth0
                var userServiceError = _userService.GetLastUserServiceError();
                return userServiceError != null
                    ?
                    // Forward Auth0 status code and error message
                    StatusCode(userServiceError.StatusCode, userServiceError.Content)
                    :
                    // Fallback if there's no specific error information
                    BadRequest(new { message = "An unexpected error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}