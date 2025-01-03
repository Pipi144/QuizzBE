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

    public async Task<Quiz?> CreateQuizAsync(Quiz? quiz)
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

    public async Task<Quiz?> GetQuizByIdAsync(int id)
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

    public async Task<Quiz?> GetQuizWithQuestionsAndOptionsAsync(int id)
    {
        try
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .ThenInclude(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.Id == id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching quiz with ID {Id}", id);
            throw new ApplicationException("An error occurred while retrieving the quiz.", ex);
        }
    }


    public async Task<PaginatedResult<Quiz>> GetPaginatedQuizzesAsync(
        string? createdByUserId = null,
        string? quizName = null,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and PageSize must be greater than 0.");
            }

            var query = _context.Quizzes.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(createdByUserId))
            {
                query = query.Where(q => q.CreatedByUserId == createdByUserId);
            }

            if (!string.IsNullOrEmpty(quizName))
            {
                query = query.Where(q => EF.Functions.ILike(q.QuizName, $"%{quizName}%"));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination and include related data
            var items = await query
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .OrderByDescending(q => q.CreatedAt) // Sort by ID or another field
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Quiz>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (ArgumentException ex)
        {
            throw new ApplicationException($"Invalid pagination parameters: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated quizzes.");
            throw new ApplicationException("An error occurred while retrieving quizzes.", ex);
        }
    }


    public async Task<Quiz?> UpdateQuizAsync(Quiz? quiz)
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