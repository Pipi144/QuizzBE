using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuizRepository
{
    Task<Quiz?> CreateQuizAsync(Quiz? quiz);
    Task<Quiz?> GetQuizByIdAsync(int id);
    Task<Quiz?> GetQuizWithQuestionsAndOptionsAsync(int id);
    Task<PaginatedResult<Quiz>> GetPaginatedQuizzesAsync(
        string? createdByUserId = null, 
        string? questionText = null, 
        int page = 1, 
        int pageSize = 10);
    Task<Quiz?> UpdateQuizAsync(Quiz? quiz);
    Task DeleteQuizAsync(int id);
}