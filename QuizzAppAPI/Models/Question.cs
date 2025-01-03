using Microsoft.Build.Framework;

namespace QuizzAppAPI.Models;

public class Question
{
    public int Id { get; set; }

    [Required] public string QuestionText { get; set; }

    [Required] public string CreatedByUserId { get; set; }
    
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    public ICollection<QuizQuestion> QuizQuestions { get; set; } =
        new List<QuizQuestion>(); // Navigation property for the join table
    
    // Method to check if a given option ID matches the correct answer
    public bool IsCorrectAnswer(int? optionId)
    {
        if (optionId == null)
        {
            return false; // Null optionId cannot be correct
        }
        var correctOption = QuestionOptions.FirstOrDefault(o => o.IsCorrectAnswer);
        return correctOption != null && correctOption.Id == optionId;
    }
}