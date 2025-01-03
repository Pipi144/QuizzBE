using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuizService(IQuizRepository quizRepository, IQuestionRepository questionRepository, IMapper mapper)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<QuizDetailDto> CreateQuizAsync(AddQuizDataDto addQuizDataDto)
    {
        try
        {
            // Map the Quiz DTO to the Quiz entity
            var quiz = _mapper.Map<Quiz>(addQuizDataDto);

            // Attach existing questions to the quiz
            foreach (var questionId in addQuizDataDto.QuestionIds)
            {
                var question = await _questionRepository.GetQuestionByIdAsync(questionId);
                if (question == null)
                {
                    throw new KeyNotFoundException($"Question with ID {questionId} not found.");
                }

                // Link the question to the quiz without re-inserting it
                quiz.QuizQuestions.Add(new QuizQuestion
                {
                    Quiz = quiz,
                    QuestionId = question.Id
                });
            }

            // Save the quiz
            var createdQuiz = await _quizRepository.CreateQuizAsync(quiz);
            
            // Fetch the quiz with its related Questions
            var fullQuiz = await _quizRepository.GetQuizByIdAsync(createdQuiz.Id);

            // Return the quiz as a DTO
            return _mapper.Map<QuizDetailDto>(fullQuiz);
        }
        catch (KeyNotFoundException ex)
        {
            throw new ApplicationException($"Error adding quiz: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An unexpected error occurred while creating the quiz.", ex);
        }
    }

    public async Task<QuizDetailDto> GetQuizByIdAsync(int id)
    {
        try
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                throw new KeyNotFoundException($"Quiz with ID {id} not found.");
            }

            return _mapper.Map<QuizDetailDto>(quiz);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"An error occurred while retrieving the quiz with ID {id}.", ex);
        }
    }

    public async Task<QuizWithFullQuestionsDto> GetQuizWithFullQuestionsAsync(int id)
    {
        try
        {
            var quiz = await _quizRepository.GetQuizWithQuestionsAndOptionsAsync(id);
            if (quiz == null)
            {
                throw new KeyNotFoundException($"Quiz with ID {id} not found.");
            }

            return _mapper.Map<QuizWithFullQuestionsDto>(quiz);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"An error occurred while retrieving the quiz with ID {id}.", ex);
        }
    }

    public async Task<PaginatedResult<QuizBasicDto>> GetPaginatedQuizzesAsync(
        string? createdByUserId = null, 
        string? quizName = null, 
        int page = 1, 
        int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and PageSize must be greater than 0.");
            }

            // Get paginated results from the repository
            var paginatedQuizzes = await _quizRepository.GetPaginatedQuizzesAsync(
                createdByUserId, 
                quizName, 
                page, 
                pageSize
            );

            // Map the results to DTOs
            return new PaginatedResult<QuizBasicDto>
            {
                Items = _mapper.Map<List<QuizDetailDto>>(paginatedQuizzes.Items),
                TotalCount = paginatedQuizzes.TotalCount,
                Page = paginatedQuizzes.Page,
                PageSize = paginatedQuizzes.PageSize
            };
        }
        catch (ArgumentException ex)
        {
            throw new ApplicationException($"Invalid pagination parameters: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while retrieving quizzes.", ex);
        }
    }


    public async Task<QuizDetailDto> UpdateQuizAsync(int id, UpdateQuizDataDto updateQuizDataDto)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetQuizByIdAsync(id);
            if (existingQuiz == null)
            {
                throw new KeyNotFoundException($"Quiz with ID {id} not found.");
            }
        
            // Update fields
            existingQuiz.QuizName = updateQuizDataDto.QuizName ?? existingQuiz.QuizName;
            // Explicitly handle TimeLimit
            if (updateQuizDataDto.TimeLimit.HasValue)
            {
                existingQuiz.TimeLimit = updateQuizDataDto.TimeLimit == 0 ? null : (int?)updateQuizDataDto.TimeLimit;
            }
            

            if (updateQuizDataDto.QuestionIds != null)
            {
                existingQuiz.QuizQuestions.Clear();

                foreach (var questionId in updateQuizDataDto.QuestionIds)
                {
                    var question = await _questionRepository.GetQuestionByIdAsync(questionId);
                    if (question == null)
                    {
                        throw new KeyNotFoundException($"Question with ID {questionId} not found.");
                    }

                    existingQuiz.QuizQuestions.Add(new QuizQuestion
                    {
                        Quiz = existingQuiz,
                        QuestionId = question.Id
                    });
                }
            }

            await _quizRepository.UpdateQuizAsync(existingQuiz);
            return _mapper.Map<QuizDetailDto>(existingQuiz);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"An error occurred while updating the quiz with ID {id}.", ex);
        }
    }

    public async Task DeleteQuizAsync(int id)
    {
        try
        {
            await _quizRepository.DeleteQuizAsync(id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"An error occurred while deleting the quiz with ID {id}.", ex);
        }
    }
}