using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service
{
    public class UserService : IUserService
    {
        private readonly Auth0Settings _auth0Settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;
        private readonly ITokenService _tokenService;
        private ErrorResponse? _lastUserServiceError;

        public UserService(
            IOptions<Auth0Settings> auth0Settings,
            HttpClient httpClient,
            ITokenService tokenService,
            ILogger<UserService> logger)
        {
            _auth0Settings = auth0Settings.Value ?? throw new ArgumentNullException(nameof(auth0Settings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public ErrorResponse? GetLastUserServiceError() => _lastUserServiceError;

        private async Task<bool> SetAuthorizationHeader(string scope)
        {
            try
            {
                var tokenResponse = await _tokenService.GetManageAccessToken(scope).ConfigureAwait(false);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    _logger.LogError("Failed to fetch management token for scope: {Scope}", scope);
                    _lastUserServiceError = _tokenService.GetLastTokenServiceError();
                    return false;
                }

                ConfigureHeaders(tokenResponse.AccessToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting authorization header for scope: {Scope}", scope);
                return false;
            }
        }

        private void ConfigureHeaders(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task<T?> SendRequest<T>(HttpMethod method, string url, object? body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                if (body != null)
                {
                    var content = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (!await HandleResponse(response).ConfigureAwait(false))
                {

                    return default;
                }

                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending request to URL: {Url}", url);
                return default;
            }
        }

        private async Task<bool> HandleResponse(HttpResponseMessage response)
        {
            try
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<Auth0ErrorResponseDto>(responseContent);
                    _lastUserServiceError = new ErrorResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = error?.ErrorDescription ?? responseContent
                    };
                    _logger.LogError("Request failed: {StatusCode}, Response: {Content}", response.StatusCode,
                        responseContent);
                    return false;
                }

                _logger.LogInformation("Request succeeded. Response: {Response}", responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling response.");
                return false;
            }
        }

        public async Task<bool> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("DeleteUser: Invalid user ID provided.");
                    return false;
                }

                if (!await SetAuthorizationHeader("delete:users").ConfigureAwait(false))
                {
                    return false;
                }

                var response = await _httpClient.DeleteAsync($"https://{_auth0Settings.Domain}/api/v2/users/{id}");
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {Id}", id);
                return false;
            }
        }

        public async Task<UserDto.Auth0CurentUserDto?> GetCurrentUserInfo(string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogWarning("GetCurrentUserInfo: Invalid access token provided.");
                    return default;
                }

                ConfigureHeaders(accessToken);
                var userInfo =
                    await SendRequest<UserDto.Auth0CurentUserDto>(HttpMethod.Get,
                        $"https://{_auth0Settings.Domain}/userinfo").ConfigureAwait(false);
                if (userInfo == null)
                {
                    return default;
                }
                if (!await SetAuthorizationHeader("read:users read:roles read:role_members").ConfigureAwait(false))
                {
                    return default;
                }

                var userRoles = await SendRequest<List<UserDto.UserRoleDto>>(HttpMethod.Get,
                    $"https://{_auth0Settings.Domain}/api/v2/users/{userInfo.UserId}/roles").ConfigureAwait(false);
                if (userRoles == null)
                {
                    return default;
                }

                userInfo.UserRoles = userRoles;
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user information.");
                return default;
            }
        }

        public async Task<IEnumerable<UserDto.UserRoleDto>?> GetAllRoles()
        {
            try
            {
                if (!await SetAuthorizationHeader("read:roles").ConfigureAwait(false))
                {
                    return default;
                }

                return await SendRequest<IEnumerable<UserDto.UserRoleDto>>(HttpMethod.Get,
                    $"https://{_auth0Settings.Domain}/api/v2/roles").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all roles.");
                return default;
            }
        }

        private async Task<bool> ClearUserRoles(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("ClearUserRoles: Invalid user ID provided.");
                    return false;
                }

                var userRoles = await SendRequest<List<UserDto.UserRoleDto>>(HttpMethod.Get,
                    $"https://{_auth0Settings.Domain}/api/v2/users/{userId}/roles").ConfigureAwait(false);

                if (userRoles == null || !userRoles.Any())
                {
                    _logger.LogInformation("ClearUserRoles: No roles to clear for user {UserId}.", userId);
                    return true;
                }

                var delRequest = new HttpRequestMessage(HttpMethod.Delete,
                    $"https://{_auth0Settings.Domain}/api/v2/users/{userId}/roles");
                var requestBody = new { roles = userRoles.Select(r => r.RoleId).ToList() };

                var delContent = JsonConvert.SerializeObject(requestBody);
                delRequest.Content = new StringContent(delContent, Encoding.UTF8, "application/json");


                var responseDelete = await _httpClient.SendAsync(delRequest).ConfigureAwait(false);

                return await HandleResponse(responseDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing user roles for user ID: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserDetails(string id, UserDto.UpdateUserParamsDto updateUserParamsDto)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("UpdateUserDetails: Invalid user ID provided.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(updateUserParamsDto.Name) &&
                    string.IsNullOrWhiteSpace(updateUserParamsDto.NickName) &&
                    string.IsNullOrWhiteSpace(updateUserParamsDto.RoleId))
                {
                    _logger.LogWarning("UpdateUserDetails: No updates provided for user {UserId}.", id);
                    return true; // Nothing to update.
                }

                if (!await SetAuthorizationHeader(
                        "update:users update:users_app_metadata read:roles create:role_members").ConfigureAwait(false))
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(updateUserParamsDto.Name) ||
                    !string.IsNullOrWhiteSpace(updateUserParamsDto.NickName))
                {
                    var body = new { name = updateUserParamsDto.Name, nickname = updateUserParamsDto.NickName };
                    var updateUserUrl = $"https://{_auth0Settings.Domain}/api/v2/users/{id}";

                    if (await SendRequest<object>(HttpMethod.Patch, updateUserUrl, body).ConfigureAwait(false) == null)
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(updateUserParamsDto.RoleId))
                {
                    if (!await ClearUserRoles(id).ConfigureAwait(false))
                    {
                        return false;
                    }

                    var assignRoleUrl = $"https://{_auth0Settings.Domain}/api/v2/users/{id}/roles";

                    var assignRoleRequest = new HttpRequestMessage(HttpMethod.Post,
                        $"https://{_auth0Settings.Domain}/api/v2/users/{id}/roles");
                    var requestBody = new { roles = new[] { updateUserParamsDto.RoleId } };

                    var assignRoleContent = JsonConvert.SerializeObject(requestBody);
                    assignRoleRequest.Content = new StringContent(assignRoleContent, Encoding.UTF8, "application/json");


                    var responseAssignRole = await _httpClient.SendAsync(assignRoleRequest).ConfigureAwait(false);
                    if (!await HandleResponse(responseAssignRole).ConfigureAwait(false))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user details for user ID: {Id}", id);
                return false;
            }
        }

        public async Task<UserDto.Auth0GetUserListResponseDto?> GetAllUsers(UserDto.GetUserListParamsDto userListParams)
        {
            try
            {
                if (!await SetAuthorizationHeader("read:roles update:users create:role_members").ConfigureAwait(false))
                {
                    return default;
                }

                var queryString = new StringBuilder(
                    $"page={userListParams.Page}&per_page={userListParams.PageSize}&include_totals=true&search_engine=v3");

                if (!string.IsNullOrWhiteSpace(userListParams.Search))
                {
                    var escapedSearch = Uri.EscapeDataString(userListParams.Search);
                    queryString.Append(escapedSearch.Length >= 3
                        ? $"&q=(email:{escapedSearch}* OR name:{escapedSearch}*)"
                        : $"&q=(email:\"{escapedSearch}\" OR name:\"{escapedSearch}\")");
                }

                return await SendRequest<UserDto.Auth0GetUserListResponseDto>(
                        HttpMethod.Get, $"https://{_auth0Settings.Domain}/api/v2/users?{queryString}")
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users.");
                return default;
            }
        }

        public async Task<UserDto.Auth0BasicUserDto?> GetUserById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("GetUserById: Invalid user ID provided.");
                    return default;
                }

                if (!await SetAuthorizationHeader("read:users read:user_idp_tokens read:roles read:role_members")
                        .ConfigureAwait(false))
                {
                    return default;
                }

                var userInfo =
                    await SendRequest<UserDto.Auth0BasicUserDto>(HttpMethod.Get,
                        $"https://{_auth0Settings.Domain}/api/v2/users/{id}").ConfigureAwait(false);
                if (userInfo == null)
                {
                    return default;
                }

                var userRoles = await SendRequest<List<UserDto.UserRoleDto>>(HttpMethod.Get,
                    $"https://{_auth0Settings.Domain}/api/v2/users/{userInfo.UserId}/roles").ConfigureAwait(false);
                if (userRoles == null)
                {
                    return default;
                }

                userInfo.UserRoles = userRoles;
                
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user by ID: {Id}", id);
                return default;
            }
        }
    }
}