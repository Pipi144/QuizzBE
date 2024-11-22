using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

public class AuthService : IAuthService
{
    private readonly Auth0Settings _auth0Settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private ErrorResponse? _lastAuth0Error;
    public AuthService(
        IOptions<Auth0Settings> auth0Settings,
        HttpClient httpClient,
        ILogger<AuthService> logger)
    {
        _auth0Settings = auth0Settings.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    private async Task<T?> SendAuth0Request<T>(string url, object requestBody)
    {
        try
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody,jsonSettings),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                // Store the last error details
                _lastAuth0Error = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
                return default;
            }
            _logger.LogInformation("Auth0 Response: {Response}", responseContent);
    
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Auth0 request");
            throw;
        }
    }

    public async Task<RegisterAuth0ResponseDTO?> Register(RegisterDTO data)
    {
        var requestBody = new 
        {
            email = data.Email,
            password = data.Password,
            client_id = _auth0Settings.ClientId,
            client_secret = _auth0Settings.ClientSecret,
            audience = _auth0Settings.Audience,
            connection = _auth0Settings.Connection,
            username = !string.IsNullOrEmpty(data.UserName) ? data.UserName : null,
            given_name = !string.IsNullOrEmpty(data.GivenName) ? data.GivenName : null,
            family_name = !string.IsNullOrEmpty(data.FamilyName) ? data.FamilyName : null,
            name = !string.IsNullOrEmpty(data.Name) ? data.Name : null,
            nickname = !string.IsNullOrEmpty(data.NickName) ? data.NickName : null
        };
        
        return await SendAuth0Request<RegisterAuth0ResponseDTO>(
            $"https://{_auth0Settings.Domain}/dbconnections/signup",
            requestBody
        );
    }

    public async Task<TokenResponseDTO?> Login(string email, string password)
    {
        var requestBody = new
        {
            username = email,
            password = password,
            client_id = _auth0Settings.ClientId,
            client_secret = _auth0Settings.ClientSecret,
            audience = _auth0Settings.Audience,
            grant_type = "password",
            connection = _auth0Settings.Connection,
        };
        return await SendAuth0Request<TokenResponseDTO>(
            $"https://{_auth0Settings.Domain}/oauth/token",
            requestBody
        );
    }
    
    public ErrorResponse? GetLastAuth0Error()
    {
        return _lastAuth0Error;
    }
}