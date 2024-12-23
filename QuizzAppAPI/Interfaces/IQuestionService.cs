using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<QuestionBasicDto>> GetQuestionsByCreatedByUserIdAsync(string userId);
    Task<QuestionDetailDto> GetQuestionByIdAsync(int id);
    Task<QuestionDetailDto> CreateQuestionAsync(AddQuestionDataDto addQuestionDto);
    Task<QuestionDetailDto> UpdateQuestionAsync(UpdateQuestionDataDto updateQuestionDto);
    Task DeleteQuestionAsync(int id);
}