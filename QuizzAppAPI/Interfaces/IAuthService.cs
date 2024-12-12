using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto?> Login(LoginDto data);

    Task<RegisterResponseDto?> Register(RegisterDto data);

    ErrorResponse? GetLastAuth0Error();
    
}