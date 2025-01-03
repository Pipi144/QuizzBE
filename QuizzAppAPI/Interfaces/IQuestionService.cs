using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuestionService
{
    Task<PaginatedResult<QuestionBasicDto>> GetPaginatedQuestionsAsync(
        string? createdByUserId = null, 
        string? questionText = null, 
        int page = 1, 
        int pageSize = 10);

    Task<QuestionDetailDto> GetQuestionByIdAsync(int id);
    Task<QuestionDetailDto> CreateQuestionAsync(AddQuestionDataDto addQuestionDto);
    Task<QuestionDetailDto> UpdateQuestionAsync(UpdateQuestionDataDto updateQuestionDto);
    Task DeleteQuestionAsync(int id);
}