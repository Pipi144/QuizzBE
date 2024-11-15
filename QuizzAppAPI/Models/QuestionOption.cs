namespace QuizzAppAPI.Models;

public class QuestionOption
{
    public int Id { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public Question Question { get; set; }
}