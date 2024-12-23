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
                await _context.Questions.AddAsync(question);
                await _context.SaveChangesAsync();
                return question;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding the question.", ex);
            }
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingOptions = _context.QuestionOptions.Where(qo => qo.QuestionId == question.Id);
                _context.QuestionOptions.RemoveRange(existingOptions);

                if (question.QuestionOptions != null)
                {
                    foreach (var option in question.QuestionOptions)
                    {
                        option.Id = 0; // Ensure EF treats these as new entities
                        _context.QuestionOptions.Add(option);
                    }
                }

                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return question;
            }
            catch
            {
                await transaction.RollbackAsync();
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
                    return false; // Return false if the question does not exist
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

                }
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Question not found for deletion of options.");
                throw; // Rethrow the NotFoundException
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting question options for question ID {QuestionId}.",
                    questionId);
                throw new ApplicationException("An error occurred while deleting question options.", ex);
            }
        }
    }
}