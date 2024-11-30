using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IUserService
{
    ErrorResponse? GetLastUserServiceError();

    Task<bool> DeleteUser(string Id, string accessToken);

    Task<UserDTO.Auth0CurentUserDTO?> GetCurrentUserInfo(string accessToken);
    
    Task<IEnumerable<UserDTO.UserRoleDTO>?> GetAllRoles();

    Task<bool> AssignUserRole(UserDTO.AssignUserRoleDTO assignRoleDTO);
    
    Task<UserDTO.Auth0GetUserListResponseDTO?> GetAllUsers(UserDTO.GetUserListParamsDTO userListParams);
}