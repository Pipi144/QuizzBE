using QuizzAppAPI.Models;

namespace QuizzAppAPI.DTO;

public class AddQuizDataDto
{
    public string QuizName { get; set; }
    public int? TimeLimit { get; set; }
    public string CreatedByUserId { get; set; }
    
    public List<int> QuestionIds { get; set; }
}

public class QuizBasicDto
{
    public int QuizId { get; set; }
    public string QuizName { get; set; }
    public int? TimeLimit { get; set; }
    public int NumberOfQuestions { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class QuizDetailDto : QuizBasicDto
{
    public ICollection<QuestionBasicDto> Questions { get; set; }
}

public class QuizWithFullQuestionsDto : QuizBasicDto
{
    public ICollection<QuestionDetailDto> Questions { get; set; }
}

public class UpdateQuizDataDto
{
    public string? QuizName { get; set; }
    public int? TimeLimit { get; set; }
    public List<int>? QuestionIds { get; set; }
}