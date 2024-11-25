using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface ITokenService
{
    string? ExtractAccessToken(IHeaderDictionary headers);
    Task<TokenResponseDTO?> GetManageAccessToken();
    
    ErrorResponse? GetLastTokenServiceError();
}