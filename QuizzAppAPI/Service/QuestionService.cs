using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuestionService(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    // Get all questions 
    public async Task<PaginatedResult<QuestionBasicDto>> GetPaginatedQuestionsAsync(
        string? createdByUserId = null,
        string? questionText = null,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and PageSize must be greater than 0.");
            }

            var paginatedQuestions = await _questionRepository.GetPaginatedQuestionsAsync(
                createdByUserId,
                questionText,
                page,
                pageSize);

           

            return new PaginatedResult<QuestionBasicDto>
            {
                Items = _mapper.Map<List<QuestionBasicDto>>(paginatedQuestions.Items),
                TotalCount = paginatedQuestions.TotalCount,
                Page = paginatedQuestions.Page,
                PageSize = paginatedQuestions.PageSize
            };
        }
        catch (ArgumentException ex)
        {
            throw new ApplicationException($"Invalid pagination parameters: {ex.Message}", ex);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException($"Error retrieving questions: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An unexpected error occurred while retrieving questions.", ex);
        }
    }



    public async Task<QuestionDetailDto> GetQuestionByIdAsync(int id)
    {
        try
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);

            return _mapper.Map<QuestionDetailDto>(question);
        }
        catch (NotFoundException ex)
        {
            throw new NotFoundException($"Question with ID {id} not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while fetching the question.", ex);
        }
    }

    // Create a new question
    public async Task<QuestionDetailDto> CreateQuestionAsync(AddQuestionDataDto addQuestionDto)
    {
        try
        {
            var question = _mapper.Map<Question>(addQuestionDto);
            var createdQuestion = await _questionRepository.AddQuestionAsync(question);
            return _mapper.Map<QuestionDetailDto>(createdQuestion);
        }
        catch (ValidationException ex)
        {
            throw new ValidationException($"Invalid data provided for creating the question. {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while creating the question.", ex);
        }
    }

    // Update an existing question
    public async Task<QuestionDetailDto> UpdateQuestionAsync(UpdateQuestionDataDto updateQuestionDto)
    {
        try
        {
            var existingQuestion = await _questionRepository.GetQuestionByIdAsync(updateQuestionDto.Id);
            if (existingQuestion == null)
                throw new KeyNotFoundException($"Question with ID {updateQuestionDto.Id} not found.");

            if (!string.IsNullOrEmpty(updateQuestionDto.QuestionText))
                existingQuestion.QuestionText = updateQuestionDto.QuestionText;

            if (updateQuestionDto.QuestionOptions != null)
            {
                existingQuestion.QuestionOptions = updateQuestionDto.QuestionOptions
                    .Select(optionDto => _mapper.Map<QuestionOption>(optionDto))
                    .ToList();
            }

            var updatedQuestion = await _questionRepository.UpdateQuestionAsync(existingQuestion);
            return _mapper.Map<QuestionDetailDto>(updatedQuestion);
        }
        catch (NotFoundException ex)
        {
            throw;
        }
        catch (ValidationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An unexpected error occurred while updating the question.", ex);
        }
    }


    // Delete a question by ID
    public async Task DeleteQuestionAsync(int id)
    {
        try
        {
            await _questionRepository.DeleteQuestionAsync(id);
        }
        catch (NotFoundException ex)
        {
            throw new NotFoundException($"Question with ID {id} not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while deleting the question.", ex);
        }
    }
}