using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;
using Microsoft.Extensions.Logging;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/question")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        // GET: api/question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionBasicDto>>> GetQuestionsByUserId([FromQuery] string userId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByCreatedByUserIdAsync(userId);
                return Ok(questions);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Questions not found for user {UserId}", userId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET: api/question/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestion(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                return Ok(question);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Question with ID {Id} not found.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST: api/question
        [HttpPost]
        public async Task<ActionResult<QuestionDetailDto>> CreateQuestion([FromBody] AddQuestionDataDto addQuestionDto)
        {
            try
            {
                var createdQuestion = await _questionService.CreateQuestionAsync(addQuestionDto);
                return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation failed while creating question.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the question.");
                return StatusCode(500, new { message = "An unexpected error occurred while creating the question." });
            }
        }

        // PUT: api/question/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDataDto updateQuestionDto)
        {
            if (id != updateQuestionDto.Id)
            {
                return BadRequest(new { message = "The question ID does not match the route parameter." });
            }

            try
            {
                var updatedQuestion = await _questionService.UpdateQuestionAsync(updateQuestionDto);

                if (updatedQuestion == null)
                {
                    return NotFound(new { message = $"Question with ID {id} not found." });
                }

                return Ok(updatedQuestion);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error while updating question.");
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Question not found while updating.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating question.");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the question." });
            }
        }
        
        
        // DELETE: api/question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Question with ID {Id} not found for deletion.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting the question.");
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the question." });
            }
        }
    }
}
