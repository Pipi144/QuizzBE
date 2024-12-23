using Microsoft.Build.Framework;

namespace QuizzAppAPI.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public string QuestionText { get; set; }

    [Required]
    public string CreatedByUserId { get; set; }

    public ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
}