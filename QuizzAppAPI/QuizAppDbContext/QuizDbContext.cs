using QuizzAppAPI.Models;

namespace QuizzAppAPI.QuizAppDbContext;
using Microsoft.EntityFrameworkCore;


public class QuizDbContext:DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options): base(options)
    {
        
    }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
}