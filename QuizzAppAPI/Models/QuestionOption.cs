using Microsoft.Build.Framework;

namespace QuizzAppAPI.Models;

public class QuestionOption
{
    public int Id { get; set; }

    [Required]
    public string OptionText { get; set; }

    public bool IsCorrectAnswer { get; set; }

    public int QuestionId { get; set; }

    public Question Question { get; set; }
}