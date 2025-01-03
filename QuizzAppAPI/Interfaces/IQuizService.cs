using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuizService
{
    Task<QuizDetailDto> CreateQuizAsync(AddQuizDataDto addQuizDataDto);
    Task<QuizDetailDto> GetQuizByIdAsync(int id);
    Task<QuizWithFullQuestionsDto> GetQuizWithFullQuestionsAsync(int id);
    
    Task<PaginatedResult<QuizBasicDto>> GetPaginatedQuizzesAsync(
        string? createdByUserId = null, 
        string? questionText = null, 
        int page = 1, 
        int pageSize = 10);
    Task<QuizDetailDto> UpdateQuizAsync(int id, UpdateQuizDataDto updateQuizDataDto);
    Task DeleteQuizAsync(int id);
}