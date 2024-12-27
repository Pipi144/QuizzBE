using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuizRepository
{
    Task<Quiz> CreateQuizAsync(Quiz quiz);
    Task<Quiz> GetQuizByIdAsync(int id);
    Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
    Task<Quiz> UpdateQuizAsync(Quiz quiz);
    Task DeleteQuizAsync(int id);
}