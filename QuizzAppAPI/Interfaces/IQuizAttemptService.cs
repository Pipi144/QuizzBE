using QuizzAppAPI.DTO;

namespace QuizzAppAPI.Interfaces;

public interface IQuizAttemptService
{
    Task<QuizAttemptDto.QuizAttemptDetailDto> CreateQuizAttemptAsync(QuizAttemptDto.CreateQuizAttemptDto createQuizAttemptDto);
    Task<QuizAttemptDto.QuizAttemptDetailDto> GetQuizAttemptByIdAsync(int id);
}