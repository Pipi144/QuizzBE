using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

public class QuizAttemptService:IQuizAttemptService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuizAttemptService(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IMapper mapper)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<QuizAttemptDto.QuizAttemptDetailDto> CreateQuizAttemptAsync(QuizAttemptDto.CreateQuizAttemptDto createQuizAttemptDto)
    {
        var quiz = await _quizRepository.GetQuizByIdAsync(createQuizAttemptDto.QuizId);
        if (quiz == null)
        {
            throw new KeyNotFoundException($"Quiz with ID {createQuizAttemptDto.QuizId} not found.");
        }
        int score = 0;
        int correctAnswers = 0;
        foreach (var questionAttempt in createQuizAttemptDto.QuestionAttempts)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionAttempt.QuestionId);
            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {questionAttempt.QuestionId} not found.");
            }

            if (question.IsCorrectAnswer(questionAttempt.SelectedOptionId))
            {
                score++;
                correctAnswers++;
            }
        }

        var quizAttempt = new QuizAttempt
        {
            Score = score,
            TotalQuestions = quiz.QuizQuestions.Count,
            CorrectAnswers = correctAnswers,
            CreatedAt = DateTime.UtcNow,
            AttemptByUserId = createQuizAttemptDto.AttemptByUserId,
            QuizId = createQuizAttemptDto.QuizId
        };

        var createdQuizAttempt = await _quizAttemptRepository.CreateQuizAttemptAsync(quizAttempt);
        return _mapper.Map<QuizAttemptDto.QuizAttemptDetailDto>(createdQuizAttempt);
    }

    public async Task<QuizAttemptDto.QuizAttemptDetailDto> GetQuizAttemptByIdAsync(int id)
    {
        var quizAttempt = await _quizAttemptRepository.GetQuizAttemptByIdAsync(id);
        if (quizAttempt == null)
        {
            throw new KeyNotFoundException($"Quiz attempt with ID {id} not found.");
        }

        return _mapper.Map<QuizAttemptDto.QuizAttemptDetailDto>(quizAttempt);
    }
}