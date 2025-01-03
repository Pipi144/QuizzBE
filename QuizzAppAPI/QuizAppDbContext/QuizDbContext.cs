using QuizzAppAPI.Models;

namespace QuizzAppAPI.QuizAppDbContext;

using Microsoft.EntityFrameworkCore;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
    }

    public DbSet<Quiz?> Quizzes { get; set; }
    public DbSet<Question?> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure the many-to-many relationship between Quiz and Question
        modelBuilder.Entity<QuizQuestion>()
            .HasKey(qq => new { qq.QuizId, qq.QuestionId }); // Composite key

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Quiz)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuizId);

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Question)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuestionId);
    }
}