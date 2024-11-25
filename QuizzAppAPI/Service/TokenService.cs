using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

public class TokenService: ITokenService
{
    private readonly Auth0Settings _auth0Settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TokenService> _logger;
    private ErrorResponse _lastTokenServiceError;
    public TokenService(
        IOptions<Auth0Settings> auth0Settings,
        HttpClient httpClient,
        ILogger<TokenService> logger)
    {
        _auth0Settings = auth0Settings.Value;
        _httpClient = httpClient;
        _logger = logger;
    }
    public string? ExtractAccessToken(IHeaderDictionary headers)
    {
        var authHeader = headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            // Extract the token by removing "Bearer " prefix
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null;
    }

    public async Task<TokenResponseDTO?> GetManageAccessToken()
    {
        try
        {
            var requestBody = new
            {
                client_id = _auth0Settings.ClientId,
                client_secret = _auth0Settings.ClientSecret,
                audience = $"https://{_auth0Settings.Domain}/api/v2/",
                grant_type = "client_credentials",
                connection = _auth0Settings.Connection,
            };
            
            var content = new StringContent(JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync($"https://{_auth0Settings.Domain}/oauth/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseContent);
            if (!response.IsSuccessStatusCode)
            {
                // Store the last error details
                _lastTokenServiceError = new ErrorResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent
                };
                return default;
            }

            _logger.LogInformation("Auth0 Response: {Response}", responseContent);

            return JsonConvert.DeserializeObject<TokenResponseDTO>(responseContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ErrorResponse? GetLastTokenServiceError()
    {
        return _lastTokenServiceError;
    }
}