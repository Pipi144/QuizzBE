using System.Net.Http.Headers;
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


    public ErrorResponse? GetLastUserServiceError()
    {
        return _lastUserServiceError;
    }

    public async Task<bool> DeleteUser(string Id, string accessToken)
    {
        try
        {
            // Set default headers for every request
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.DeleteAsync(
                $"https://{_auth0Settings.Domain}/api/v2/users/{Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                // Store the last error details
                _lastUserServiceError = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
                return false;
            }

            _logger.LogInformation("Auth0 Response: {Response}", responseContent);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<UserDTO.UserResponseDTO?> GetCurrentUserInfo(string accessToken)
    {
        try
        {
            // Set default headers for every request
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine(accessToken);
            var response = await _httpClient.GetAsync(
                $"https://{_auth0Settings.Domain}/userinfo");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                // Store the last error details
                _lastUserServiceError = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
                return default;
            }

            _logger.LogInformation("Auth0 Response: {Response}", responseContent);

            return JsonConvert.DeserializeObject<UserDTO.UserResponseDTO>(responseContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<UserDTO.UserRoleDTO>?> GetAllRoles()
    {
        try
        {
            // Get manage access token
            var managementTokenResponse = await _tokenService.GetManageAccessToken();
            if (managementTokenResponse == null || string.IsNullOrEmpty(managementTokenResponse.AccessToken))
            {
                // If getting the management token fails, return error details
                _logger.LogError("Failed to retrieve management token after successful login.");
                return default; // Error details are already set in _lastAuth0Error by PostAuth0Request
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", managementTokenResponse.AccessToken);
            var response = await _httpClient.GetAsync(
                $"https://{_auth0Settings.Domain}/api/v2/roles");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                // Store the last error details
                _lastUserServiceError = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
                return default;
            }
            return JsonConvert.DeserializeObject<IEnumerable<UserDTO.UserRoleDTO>>(responseContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}