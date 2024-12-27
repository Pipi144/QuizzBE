using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Interfaces;

public interface IQuizService
{
    Task<QuizDetailDto> CreateQuizAsync(AddQuizDataDto addQuizDataDto);
    Task<QuizDetailDto> GetQuizByIdAsync(int id);
    Task<IEnumerable<QuizDetailDto>> GetAllQuizzesAsync();
    Task<QuizDetailDto> UpdateQuizAsync(int id, UpdateQuizDataDto updateQuizDataDto);
    Task DeleteQuizAsync(int id);
}