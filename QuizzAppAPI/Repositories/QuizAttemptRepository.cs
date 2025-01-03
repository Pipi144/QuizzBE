using Microsoft.EntityFrameworkCore;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;

namespace QuizzAppAPI.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly QuizDbContext _context;
    private readonly ILogger<QuizAttemptRepository> _logger;

    public QuizAttemptRepository(QuizDbContext context, ILogger<QuizAttemptRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<QuizAttempt> CreateQuizAttemptAsync(QuizAttempt quizAttempt)
    {
        try
        {
            _context.QuizAttempts.Add(quizAttempt);
            await _context.SaveChangesAsync();
            return quizAttempt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz attempt.");
            throw new ApplicationException("An error occurred while creating the quiz attempt.", ex);
        }
    }

    public async Task<QuizAttempt?> GetQuizAttemptByIdAsync(int id)
    {
        try
        {
            return await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                .FirstOrDefaultAsync(qa => qa.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quiz attempt with ID {Id}", id);
            throw new ApplicationException($"An error occurred while retrieving the quiz attempt with ID {id}.", ex);
        }
    }
}