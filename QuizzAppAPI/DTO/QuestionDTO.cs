using QuizzAppAPI.Models;

namespace QuizzAppAPI.DTO;

public class QuestionDTO
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CreatedByUserId { get; set; }
    public ICollection<QuestionOption> QuestionOptions { get; set; }
}