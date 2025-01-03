using Microsoft.Build.Framework;

namespace QuizzAppAPI.DTO;

public class QuizAttemptDto
{
    public class CreateQuizAttemptDto
    {
        [Required]
        public string AttemptByUserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public List<QuestionAttemptDto> QuestionAttempts { get; set; }
    }

    public class QuestionAttemptDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int? SelectedOptionId { get; set; }
    }

    public class QuizAttemptDetailDto
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AttemptByUserId { get; set; }
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }

    public class QuestionAttemptResultDto
    {
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; }
    }
}