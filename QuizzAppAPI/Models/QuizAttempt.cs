using System.ComponentModel.DataAnnotations;

namespace QuizzAppAPI.Models;

public class QuizAttempt
{
    public int Id { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
    [Required]
    public string AttemptByUserId { get; set; }
    public Quiz Quiz { get; set; }
}