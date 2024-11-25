using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDTO?> Login(LoginDTO data);

    Task<RegisterResponseDTO?> Register(RegisterDTO data);

    ErrorResponse? GetLastAuth0Error();
    
}