using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;

namespace QuizzAppAPI.DTO;

public class UserDTO
{
    public class Auth0CurentUserDTO
    {
        [JsonProperty("sub")] public string? Sub { get; set; }

        [JsonProperty("name")] public string? Name { get; set; }

        [JsonProperty("nickname")] public string? NickName { get; set; }

        [JsonProperty("family_name")] public string? FamilyName { get; set; }

        [JsonProperty("given_name")] public string? GivenName { get; set; }

        [JsonProperty("middle_name")] public string? MiddleName { get; set; }

        [JsonProperty("email")] public string? Email { get; set; }

        [JsonProperty("email_verified")] public bool EmailVerified { get; set; }

        [JsonProperty("gender")] public string? Gender { get; set; }

        [JsonProperty("birthdate")] public string? BirthDate { get; set; }

        [JsonProperty("phone_number")] public int? PhoneNumber { get; set; }

        [JsonProperty("phone_number_verified")]
        public bool? PhoneNumberVerified { get; set; }

        [JsonProperty("updated_at")] public string? UpdateAt { get; set; }

        public IEnumerable<UserRoleDTO>? UserRoles { get; set; }
    }

    public class UserRoleDTO
    {
        [JsonProperty("id")] public string RoleId { get; set; }

        [JsonProperty("name")] public string RoleName { get; set; }

        [JsonProperty("description")] public string RoleDescription { get; set; }
    }

    public class AssignUserRoleDTO
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class Auth0UserListItemDTO
    {
        [JsonProperty("email")] public string? Email { get; set; }

        [JsonProperty("email_verified")] public string? EmailVerified { get; set; }

        [JsonProperty("name")] public string? Name { get; set; }

        [JsonProperty("created_at")] public string? CreatedAt { get; set; }

        [JsonProperty("updated_at")] public string? UpdatedAt { get; set; }

        [JsonProperty("picture")] public string? PictureUrl { get; set; }

        [JsonProperty("nickname")] public string? NickName { get; set; }

        [JsonProperty("last_login")] public string? LastLogin { get; set; }
    }

    public class Auth0GetUserListResponseDTO
    {
        [JsonProperty("start")] public int Start { get; set; }
        [JsonProperty("limit")] public int PageSize { get; set; }
        [JsonProperty("total")] public int Total { get; set; }

        [JsonProperty("users")]
        public IEnumerable<Auth0UserListItemDTO> Users { get; set; } = new List<Auth0UserListItemDTO>();
    }

    public class GetUserListParamsDTO
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; } = null;
    }
}