using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace QuizzAppAPI.DTO;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDTO
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
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

public class RegisterResponseDTO
{
    [JsonProperty("_id")] 
    public string Id { get; set; }
    [JsonProperty("email_verified")] 
    public bool EmailVerified { get; set; }
    [JsonProperty("email")] 
    public string? Email { get; set; }
    [JsonProperty("username")] 
    public string? UserName { get; set; }
    [JsonProperty("given_name")] 
    public string? GivenName { get; set; }
    [JsonProperty("family_name")] 
    public string? FamilyName { get; set; }
    [JsonProperty("name")] 
    public string? Name { get; set; }
    [JsonProperty("nickname")] 
    public string? NickName { get; set; }

    [JsonProperty("picture")] 
    public string? Picture { get; set; }
}


public class UserResponseDTO
{
[JsonProperty("sub")] 
public string? Sub { get; set; }

[JsonProperty("name")] 
public string? Name { get; set; }

[JsonProperty("nickname")] 
public string? NickName { get; set; }

[JsonProperty("family_name")] 
public string? FamilyName { get; set; }

[JsonProperty("given_name")] 
public string? GivenName { get; set; }

[JsonProperty("middle_name")] 
public string? MiddleName { get; set; }

[JsonProperty("email")] 
public string? Email { get; set; }

[JsonProperty("email_verified")] 
public bool EmailVerified { get; set; }

[JsonProperty("gender")] 
public string? Gender { get; set; }

[JsonProperty("birthdate")] 
public string? BirthDate { get; set; }

[JsonProperty("phone_number")] 
public int? PhoneNumber { get; set; }

[JsonProperty("phone_number_verified")] 
public bool? PhoneNumberVerified { get; set; }

[JsonProperty("updated_at")] 
public string? UpdateAt { get; set; }
}