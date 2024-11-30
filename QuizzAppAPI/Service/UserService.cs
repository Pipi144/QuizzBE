using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

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
        _auth0Settings = auth0Settings.Value;
        _httpClient = httpClient;
        _logger = logger;
        _tokenService = tokenService;
    }

    private void ConfigureHeaders(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private async Task<bool> HandleResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _lastUserServiceError = new ErrorResponse
            {
                StatusCode = (int)response.StatusCode,
                Content = responseContent
            };
            _logger.LogError("Request failed: {StatusCode}, Response: {Content}", response.StatusCode, responseContent);
            return false;
        }

        _logger.LogInformation("Request succeeded. Response: {Response}", responseContent);
        return true;
    }

    public ErrorResponse? GetLastUserServiceError() => _lastUserServiceError;

    public async Task<bool> DeleteUser(string id, string accessToken)
    {
        try
        {
            ConfigureHeaders(accessToken);

            var response = await _httpClient.DeleteAsync($"https://{_auth0Settings.Domain}/api/v2/users/{id}");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while deleting user: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDTO.Auth0CurentUserDTO?> GetCurrentUserInfo(string accessToken)
    {
        try
        {
            ConfigureHeaders(accessToken);

            // Fetch user info
            var userInfoResponse = await _httpClient.GetAsync($"https://{_auth0Settings.Domain}/userinfo");
            if (!await HandleResponse(userInfoResponse))
                return default;

            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<UserDTO.Auth0CurentUserDTO>(userInfoContent);

            // Fetch user roles
            var managementTokenResponse =
                await _tokenService.GetManageAccessToken("read:users read:roles read:role_members");
            if (managementTokenResponse == null || string.IsNullOrEmpty(managementTokenResponse.AccessToken))
            {
                _logger.LogError("Failed to fetch management token for user roles.");
                _lastUserServiceError = _tokenService.GetLastTokenServiceError();
                return default;
            }

            ConfigureHeaders(managementTokenResponse.AccessToken);
            var userRolesResponse =
                await _httpClient.GetAsync($"https://{_auth0Settings.Domain}/api/v2/users/{userInfo?.Sub}/roles");
            if (!await HandleResponse(userRolesResponse))
                return default;

            var userRolesContent = await userRolesResponse.Content.ReadAsStringAsync();
            userInfo!.UserRoles = JsonConvert.DeserializeObject<List<UserDTO.UserRoleDTO>>(userRolesContent);

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while fetching current user info.");
            throw;
        }
    }

    public async Task<IEnumerable<UserDTO.UserRoleDTO>?> GetAllRoles()
    {
        try
        {
            var managementTokenResponse = await _tokenService.GetManageAccessToken("read:roles");
            if (managementTokenResponse == null || string.IsNullOrEmpty(managementTokenResponse.AccessToken))
            {
                _logger.LogError("Failed to fetch management token for roles.");
                _lastUserServiceError = _tokenService.GetLastTokenServiceError();
                return default;
            }

            ConfigureHeaders(managementTokenResponse.AccessToken);
            var response = await _httpClient.GetAsync($"https://{_auth0Settings.Domain}/api/v2/roles");
            if (!await HandleResponse(response))
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<UserDTO.UserRoleDTO>>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while fetching roles.");
            throw;
        }
    }


    public async Task<bool> AssignUserRole(UserDTO.AssignUserRoleDTO assignRoleDTO)
    {
        try
        {
            var managementTokenResponse =
                await _tokenService.GetManageAccessToken("read:roles update:users create:role_members");
            if (managementTokenResponse == null || string.IsNullOrEmpty(managementTokenResponse.AccessToken))
            {
                _logger.LogError("Failed to fetch management token for assigning user roles.");
                _lastUserServiceError = _tokenService.GetLastTokenServiceError();
                return false;
            }

            ConfigureHeaders(managementTokenResponse.AccessToken);
            var requestBody = new { roles = new[] { assignRoleDTO.RoleId } };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"https://{_auth0Settings.Domain}/api/v2/users/auth0|{assignRoleDTO.UserId}/roles", content);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while assigning user roles: {RoleId} to {UserId}",
                assignRoleDTO.RoleId, assignRoleDTO.UserId);
            throw;
        }
    }

    public async Task<UserDTO.Auth0GetUserListResponseDTO?> GetAllUsers(
        UserDTO.GetUserListParamsDTO userListParams)
    {
        try
        {
            Console.WriteLine("PARAMS:", userListParams);
            var managementTokenResponse =
                await _tokenService.GetManageAccessToken("read:roles update:users create:role_members");
            if (managementTokenResponse == null || string.IsNullOrEmpty(managementTokenResponse.AccessToken))
            {
                _logger.LogError("Failed to fetch management token for assigning user roles.");
                _lastUserServiceError = _tokenService.GetLastTokenServiceError();
                return default;
            }

            ConfigureHeaders(managementTokenResponse.AccessToken);
            // Construct query string parameters
            var queryString =
                new StringBuilder(
                    $"page={userListParams.Page}&per_page={userListParams.PageSize}&include_totals=true&search_engine=v3");
            if (!string.IsNullOrWhiteSpace(userListParams.Search))
            {
                var escapedSearch = Uri.EscapeDataString(userListParams.Search);
                if (escapedSearch.Length >= 3)
                {
                    // Wildcard search for substring matching (requires at least 3 characters)
                    queryString.Append($"&q=(email:{escapedSearch}* OR name:{escapedSearch}*)");
                }
                else
                {
                    // For shorter search strings, fallback to exact match
                    queryString.Append($"&q=(email:\"{escapedSearch}\" OR name:\"{escapedSearch}\")");
                }
            }

            // Make the GET request
            var response = await _httpClient.GetAsync($"https://{_auth0Settings.Domain}/api/v2/users?{queryString}");
            if (!await HandleResponse(response))
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RESP:", responseContent);
            return JsonConvert.DeserializeObject<UserDTO.Auth0GetUserListResponseDTO>(responseContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}