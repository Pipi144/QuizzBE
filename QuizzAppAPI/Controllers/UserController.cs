using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ILogger<UserController> logger, ITokenService tokenService)
        {
            _userService = userService;
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet("current-user-info")]
        [Authorize(Policy = "GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var accessToken = _tokenService.ExtractAccessToken(Request.Headers);
                if (accessToken == null)
                {
                    return HandleError("Access token is missing or invalid.", StatusCodes.Status401Unauthorized);
                }

                var result = await _userService.GetCurrentUserInfo(accessToken);
                if (result != null) return Ok(result);

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user info.");
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return HandleError("User ID is required.", StatusCodes.Status400BadRequest);
                }

                var result = await _userService.DeleteUser(id);
                if (result) return NoContent();

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}.", id);
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("user-roles")]
        [Authorize(Policy = "RetrieveUser")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var result = await _userService.GetAllRoles();
                if (result != null) return Ok(result);

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user roles.");
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RetrieveUser")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return HandleError("User ID is required.", StatusCodes.Status400BadRequest);
                }

                var result = await _userService.GetUserById(id);
                if (result != null) return Ok(result);

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}.", id);
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("")]
        [Authorize(Policy = "RetrieveUser")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserDto.GetUserListParamsDto param)
        {
            try
            {
                var result = await _userService.GetAllUsers(param);
                if (result != null) return Ok(result);

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(string id,
            [FromBody] UserDto.UpdateUserParamsDto updateUserParamsDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return HandleError("User ID is required.", StatusCodes.Status400BadRequest);
                }

                if (updateUserParamsDto == null)
                {
                    return HandleError("Request body cannot be null.", StatusCodes.Status400BadRequest);
                }

                var updateResult = await _userService.UpdateUserDetails(id, updateUserParamsDto);
                if (updateResult) return NoContent();

                var userServiceError = _userService.GetLastUserServiceError();
                return HandleServiceError(userServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID {UserId}.", id);
                return HandleError("Internal server error.", StatusCodes.Status500InternalServerError);
            }
        }

        // Helper method for handling service errors
        private IActionResult HandleServiceError(ErrorResponse? serviceError)
        {
            return serviceError != null
                ? HandleError(serviceError.Content, serviceError.StatusCode)
                : HandleError("An unexpected error occurred.", StatusCodes.Status400BadRequest);
        }
    }
}