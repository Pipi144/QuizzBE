using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/quiz-attempts")]
    [ApiController]
    public class QuizAttemptController : ApiControllerBase
    {
        private readonly IQuizAttemptService _quizAttemptService;

        public QuizAttemptController(IQuizAttemptService quizAttemptService)
        {
            _quizAttemptService = quizAttemptService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuizAttempt([FromBody] QuizAttemptDto.CreateQuizAttemptDto createQuizAttemptDto)
        {
            try
            {
                if (createQuizAttemptDto == null)
                {
                    return BadRequest("Quiz attempt data must be provided.");
                }

                var createdQuizAttempt = await _quizAttemptService.CreateQuizAttemptAsync(createQuizAttemptDto);
                return CreatedAtAction(nameof(GetQuizAttemptById), new { id = createdQuizAttempt.Id }, createdQuizAttempt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while creating the quiz attempt.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizAttemptById(int id)
        {
            try
            {
                var quizAttempt = await _quizAttemptService.GetQuizAttemptByIdAsync(id);
                return Ok(quizAttempt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the quiz attempt.");
            }
        }
    }
}
