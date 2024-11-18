using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Interfaces;

public interface IAuthService
{
    UserDTO Authenticate(string email, string password);
    IEnumerable<UserDTO> GetAllUsers();
}