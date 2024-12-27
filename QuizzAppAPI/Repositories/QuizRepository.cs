using Microsoft.EntityFrameworkCore;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;

namespace QuizzAppAPI.Repositories;

public class QuizRepository: IQuizRepository
{
    private readonly QuizDbContext _context;
    private readonly ILogger<QuizRepository> _logger;

    public QuizRepository(QuizDbContext context, ILogger<QuizRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        try
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz.");
            throw new ApplicationException("An error occurred while creating the quiz.", ex);
        }
    }

    public async Task<Quiz> GetQuizByIdAsync(int id)
    {
        try
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving quiz with ID {id}.");
            throw new ApplicationException($"An error occurred while retrieving the quiz with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
    {
        try
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all quizzes.");
            throw new ApplicationException("An error occurred while retrieving all quizzes.", ex);
        }
    }

    public async Task<Quiz> UpdateQuizAsync(Quiz quiz)
    {
        try
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating quiz with ID {quiz.Id}.");
            throw new ApplicationException($"An error occurred while updating the quiz with ID {quiz.Id}.", ex);
        }
    }

    public async Task DeleteQuizAsync(int id)
    {
        try
        {
            var quiz = await GetQuizByIdAsync(id);
            if (quiz == null)
            {
                throw new KeyNotFoundException($"Quiz with ID {id} not found.");
            }
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting quiz with ID {id}.");
            throw new ApplicationException($"An error occurred while deleting the quiz with ID {id}.", ex);
        }
    }
}