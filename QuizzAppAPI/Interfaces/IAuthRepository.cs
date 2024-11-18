using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthRepository
{
    User GetUserByEmailAndPassword(string email, string password);
    IEnumerable<User> GetAllUsers();
}