using Microsoft.Build.Framework;

namespace QuizzAppAPI.Models;

public class Quiz
{
    public int Id { get; set; }
    public string QuizName { get; set; } = null!;
    public int? TimeLimit { get; set; }

    [Required] public string CreatedByUserId { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } =
        new List<QuizQuestion>(); // Navigation property for the join table

    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}