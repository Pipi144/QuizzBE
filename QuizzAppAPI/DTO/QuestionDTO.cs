using QuizzAppAPI.Models;

namespace QuizzAppAPI.DTO;

public class QuestionBasicDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CreatedByUserId { get; set; }
}

public class QuestionOptionBasicDto
{
    public string OptionText { get; set; }
    public bool IsCorrectAnswer { get; set; }
}
public class QuestionDetailDto : QuestionBasicDto
{
    public ICollection<QuestionOptionBasicDto> QuestionOptions { get; set; }
}

public class AddQuestionDataDto
{
    public string QuestionText { get; set; }
    public string CreatedByUserId { get; set; }
    public ICollection<AddQuestionOptionDataDto> QuestionOptions { get; set; }
}

public class UpdateQuestionDataDto
{
    public int Id { get; set; }
    public string? QuestionText { get; set; }
    public ICollection<UpdateQuestionOptionDataDto>? QuestionOptions { get; set; }
}

public class UpdateQuestionOptionDataDto
{
    public string OptionText { get; set; }
    public bool IsCorrectAnswer { get; set; }
}
public class QuestionOptionDto
{
    public int Id { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrectAnswer { get; set; }
}

public class AddQuestionOptionDataDto
{
    public string OptionText { get; set; }
    public bool IsCorrectAnswer { get; set; }
}