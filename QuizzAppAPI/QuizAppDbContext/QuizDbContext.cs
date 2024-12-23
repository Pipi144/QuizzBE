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
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Question -> QuestionOption relationship with Cascade Delete
        // Configure Question -> QuestionOption relationship with Cascade Delete
        modelBuilder.Entity<Question>()
            .HasMany(q => q.QuestionOptions)           // A Question has many QuestionOptions
            .WithOne(qo => qo.Question)                // Each QuestionOption belongs to a Question
            .HasForeignKey(qo => qo.QuestionId)        // Foreign key in QuestionOption
            .OnDelete(DeleteBehavior.Cascade);         // Enable cascade delete
    }
}