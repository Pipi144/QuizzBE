using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDTO?> Login(string email, string password);

    Task<RegisterAuth0ResponseDTO?> Register(RegisterDTO data);

    ErrorResponse? GetLastAuth0Error();
}