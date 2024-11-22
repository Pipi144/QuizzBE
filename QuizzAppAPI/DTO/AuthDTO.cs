using Newtonsoft.Json;

namespace QuizzAppAPI.DTO;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class TokenResponseDTO
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


public class RegisterDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? NickName { get; set; }
}

public class RegisterAuth0ResponseDTO
{
    [JsonProperty("_id")]
    public string Id { get; }
    
    [JsonProperty("email_verified")]
    public bool EmailVerified { get; }
    
    [JsonProperty("email")]
    public string Email { get; }
    
    [JsonProperty("username")]
    public string UserName { get; set; }
    
    [JsonProperty("given_name")]
    public string GivenName { get; set; }

    [JsonProperty("family_name")]
    public string FamilyName { get; set; }
    
    [JsonProperty("Name")]
    public string Name { get; set; }
    
    [JsonProperty("nickname")]
    public string NickName { get; set; }
  
    [JsonProperty("picture")]
    public string Picture { get; set; }
}