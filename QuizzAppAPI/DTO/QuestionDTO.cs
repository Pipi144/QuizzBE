using QuizzAppAPI.Models;

namespace QuizzAppAPI.DTO;

public class QuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CreatedByUserId { get; set; }
    public ICollection<QuestionOption> QuestionOptions { get; set; }
}