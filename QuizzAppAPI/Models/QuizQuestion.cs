namespace QuizzAppAPI.Models;

public class QuizQuestion
{
    public int QuestionId { get; set; }
    public int QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public Question Question { get; set; }
}