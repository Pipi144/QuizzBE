using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt> CreateQuizAttemptAsync(QuizAttempt quizAttempt);
    Task<QuizAttempt?> GetQuizAttemptByIdAsync(int id);
}