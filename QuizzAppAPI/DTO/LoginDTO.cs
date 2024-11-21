using Newtonsoft.Json;

namespace QuizzAppAPI.DTO;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}