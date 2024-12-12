using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
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


    private async Task<T?> PostAuth0Request<T>(string url, object requestBody, MediaTypeHeaderValue? mediaType = null)
    {
        try
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody, jsonSettings),
                Encoding.UTF8);
            content.Headers.ContentType = mediaType ?? new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<Auth0ErrorResponseDto>(responseContent);
                // Store the last error details
                _lastAuth0Error = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = error?.ErrorDescription ?? responseContent,
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

    

    public async Task<RegisterResponseDto?> Register(RegisterDto data)
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

        return await PostAuth0Request<RegisterResponseDto>(
            $"https://{_auth0Settings.Domain}/dbconnections/signup",
            requestBody
        );
    }

    public async Task<TokenResponseDto?> Login(LoginDto data)
    {
        try
        {
            var requestBody = new
            {
                username = data.Email,
                password = data.Password,
                client_id = _auth0Settings.ClientId,
                client_secret = _auth0Settings.ClientSecret,
                audience = _auth0Settings.Audience,
                grant_type = "password",
                connection = _auth0Settings.Connection,
                scope = "openid profile email"
            };
            // Call the login endpoint to get user-specific token
            return await PostAuth0Request<TokenResponseDto>(
                $"https://{_auth0Settings.Domain}/oauth/token",
                requestBody
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ErrorResponse? GetLastAuth0Error()
    {
        return _lastAuth0Error;
    }

    
}

       