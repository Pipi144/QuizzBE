using Microsoft.Build.Framework;

namespace QuizzAppAPI.Models;

public class Quiz
{
    public int Id { get; set; }
    public string QuizName { get; set; } = null!;
    public int? TimeLimit { get; set; }
    [Required]
    public string CreatedByUserId { get; set; }
    public ICollection<Question> Questions { get; set; }
    public ICollection<QuizAttempt> QuizAttempts { get; set; }
}