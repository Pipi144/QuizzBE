using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDTO?> Login(LoginDTO data);

    Task<RegisterResponseDTO?> Register(RegisterDTO data);

    ErrorResponse? GetLastAuth0Error();

    Task DeleteUser(string Id);
    
    Task<UserResponseDTO?> GetUserInfo(string accessToken);
    
}