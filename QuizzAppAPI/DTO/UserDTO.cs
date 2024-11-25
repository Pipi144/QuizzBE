using Newtonsoft.Json;

namespace QuizzAppAPI.DTO;

public class UserDTO
{
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

    public class UserRoleDTO
    {
        [JsonProperty("id")]
        public string RoleId { get; set; }
        
        [JsonProperty("name")]
        public string RoleName { get; set; }
        
        [JsonProperty("description")]
        public string RoleDescription { get; set; }
    }

    public class AssignUserRoleDTO
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}