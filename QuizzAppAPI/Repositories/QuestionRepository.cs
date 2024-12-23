using Microsoft.EntityFrameworkCore;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizzAppAPI.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuizDbContext _context;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(QuizDbContext context, ILogger<QuestionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Question>> GetQuestionsByCreatedByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Questions
                    .Where(q => q.CreatedByUserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching questions for user {UserId}", userId);
                return Enumerable.Empty<Question>();
            }
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.QuestionOptions)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching question with ID {Id}", id);
                return null;
            }
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
            try
            {
                // Attach related QuestionOptions to the context
                if (question.QuestionOptions.Any())
                {
                    foreach (var option in question.QuestionOptions)
                    {
                        _context.Entry(option).State = EntityState.Added;
                    }
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the question.");
                throw new ApplicationException("An error occurred while adding the question.", ex);
            }
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            try
            {
                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
                return question;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed for question with ID {Id}", question.Id);
                throw new ApplicationException("An error occurred while updating the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating question with ID {Id}", question.Id);
                throw;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            try
            {
                var question = await _context.Questions.FindAsync(id);
                if (question == null)
                {
                    return false;  // Return false if the question does not exist
                }

                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the question with ID {Id}", id);
                return false;
            }
        }

        // Delete all QuestionOptions associated with a specific question
        public async Task DeleteAllQuestionOptionsAsync(int questionId)
        {
            try
            {
                var options = await _context.QuestionOptions
                    .Where(qo => qo.Question.Id == questionId)
                    .ToListAsync();

                if (options.Any())
                {
                    _context.QuestionOptions.RemoveRange(options);
                    await _context.SaveChangesAsync();
                }
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Question not found for deletion of options.");
                throw;  // Rethrow the NotFoundException
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting question options for question ID {QuestionId}.", questionId);
                throw new ApplicationException("An error occurred while deleting question options.", ex);
            }
        }
    }
}
