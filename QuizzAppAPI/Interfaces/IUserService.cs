using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IUserService
{
    ErrorResponse? GetLastUserServiceError();

    Task<bool> DeleteUser(string id);

    Task<UserDto.Auth0CurentUserDto?> GetCurrentUserInfo(string accessToken);
    
    Task<IEnumerable<UserDto.UserRoleDto>?> GetAllRoles();
    
    Task<bool> UpdateUserDetails(string id, UserDto.UpdateUserParamsDto updateUserParamsDto);
    
    Task<UserDto.Auth0GetUserListResponseDto?> GetAllUsers(UserDto.GetUserListParamsDto userListParams);
    
    Task<UserDto.Auth0BasicUserDto?> GetUserById(string id);
}