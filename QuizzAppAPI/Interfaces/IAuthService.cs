using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> Login(string email, string password);
}