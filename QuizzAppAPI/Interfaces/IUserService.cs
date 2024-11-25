using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IUserService
{
    ErrorResponse? GetLastUserServiceError();

    Task<bool> DeleteUser(string Id, string accessToken);

    Task<UserDTO.UserResponseDTO?> GetCurrentUserInfo(string accessToken);
    
    Task<IEnumerable<UserDTO.UserRoleDTO>?> GetAllRoles();

    Task<bool> AssignUserRole(UserDTO.AssignUserRoleDTO assignRoleDTO);
}