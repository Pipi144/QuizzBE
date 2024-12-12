using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;

namespace QuizzAppAPI.DTO;

public class UserDto
{
    public class Auth0BasicUserDto
    {
        [JsonProperty("user_id")] public string? UserId { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("nickname")] public string? NickName { get; set; }
        [JsonProperty("family_name")] public string? FamilyName { get; set; }
        [JsonProperty("given_name")] public string? GivenName { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("email_verified")] public bool EmailVerified { get; set; }
        [JsonProperty("updated_at")] public string? UpdatedAt { get; set; }
        [JsonProperty("picture")] public string? PictureUrl { get; set; }
        public IEnumerable<UserRoleDto>? UserRoles { get; set; }
    }

    public class Auth0CurentUserDto : Auth0BasicUserDto
    {
        [JsonProperty("sub")] public string? UserId { get; set; }
        [JsonProperty("gender")] public string? Gender { get; set; }
        [JsonProperty("birthdate")] public string? BirthDate { get; set; }
        [JsonProperty("phone_number")] public int? PhoneNumber { get; set; }
        [JsonProperty("phone_number_verified")]
        public bool? PhoneNumberVerified { get; set; }

        
    }

    public class UserRoleDto
    {
        [JsonProperty("id")] public string RoleId { get; set; }

        [JsonProperty("name")] public string RoleName { get; set; }

        [JsonProperty("description")] public string RoleDescription { get; set; }
    }

    public class AssignUserRoleDto
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
    

    public class Auth0GetUserListResponseDto
    {
        [JsonProperty("start")] public int Start { get; set; }
        [JsonProperty("limit")] public int PageSize { get; set; }
        [JsonProperty("total")] public int Total { get; set; }

        [JsonProperty("users")]
        public IEnumerable<Auth0BasicUserDto> Users { get; set; } = new List<Auth0BasicUserDto>();
    }

    public class GetUserListParamsDto
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; } = null;
    }

    public class UpdateUserParamsDto
    {
        public string? Name { get; set; }
        public string? NickName { get; set; }   
        public string? RoleId { get; set; }
    }
}