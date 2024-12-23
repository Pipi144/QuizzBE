using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Service;

public class QuestionService: IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        // Get all questions created by a specific user
        public async Task<IEnumerable<QuestionBasicDto>> GetQuestionsByCreatedByUserIdAsync(string userId)
        {
            try
            {
                var questions = await _questionRepository.GetQuestionsByCreatedByUserIdAsync(userId);

                return _mapper.Map<IEnumerable<QuestionBasicDto>>(questions);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException($"No questions found for user with ID {userId}. {ex.Message}" );
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching questions.", ex);
            }
        }

        // Get detailed question by ID
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
                // Fetch the existing question
                var existingQuestion = await _questionRepository.GetQuestionByIdAsync(updateQuestionDto.Id);
                if (existingQuestion == null)
                {
                    throw new NotFoundException($"Question with ID {updateQuestionDto.Id} not found.");
                }

                // Update QuestionText if provided
                if (!string.IsNullOrEmpty(updateQuestionDto.QuestionText))
                {
                    existingQuestion.QuestionText = updateQuestionDto.QuestionText;
                }

                // Handle QuestionOptions update if provided
                if (updateQuestionDto.QuestionOptions != null)
                {
                    // Clear the existing options
                    existingQuestion.QuestionOptions.Clear();

                    // Map and add the updated options
                    foreach (var optionDto in updateQuestionDto.QuestionOptions)
                    {
                        var option = _mapper.Map<QuestionOption>(optionDto); // Map DTO to entity
                        existingQuestion.QuestionOptions.Add(option);
                    }
                }

                // Save updated question
                var updatedQuestion = await _questionRepository.UpdateQuestionAsync(existingQuestion);

                // Map updated question to DTO
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