namespace QuizzAppAPI.Models;

public class Question
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public User CreatedByUser { get; set; }
    public ICollection<QuestionOption> QuestionOptions { get; set; }
}