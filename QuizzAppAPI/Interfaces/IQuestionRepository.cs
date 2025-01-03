using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuestionRepository
{
    Task<PaginatedResult<Question>> GetPaginatedQuestionsAsync(
        string? createdByUserId = null, 
        string? questionText = null, 
        int page = 1, 
        int pageSize = 10);

    Task<IEnumerable<Question?>> GetQuestionsByCreatedByUserIdAsync(string userId);
    Task<Question?> GetQuestionByIdAsync(int id);
    Task<Question?> AddQuestionAsync(Question? question);
    Task<Question?> UpdateQuestionAsync(Question? question);
    Task<bool> DeleteQuestionAsync(int id);

    Task DeleteAllQuestionOptionsAsync(int questionId);
}