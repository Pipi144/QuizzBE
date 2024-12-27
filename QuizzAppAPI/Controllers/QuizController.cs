using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }


        // Get quiz by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    return NotFound($"Quiz with ID {id} not found.");
                }

                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the quiz.");
            }
        }


        // Create a new quiz
        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] AddQuizDataDto addQuizDataDto)
        {
            try
            {
                if (addQuizDataDto == null)
                {
                    return BadRequest("Quiz data must be provided.");
                }

                var createdQuiz = await _quizService.CreateQuizAsync(addQuizDataDto);
                Console.WriteLine("new quiz:" + createdQuiz);
                // Ensure route matches GetQuizById
                return CreatedAtAction(nameof(GetQuizById), new { id = createdQuiz.QuizId }, createdQuiz);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        // Get all quizzes
        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync();
                return Ok(quizzes);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // Update an existing quiz
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizDataDto updateQuizDataDto)
        {
            try
            {
                if (updateQuizDataDto == null)
                {
                    return BadRequest("Quiz data must be provided.");
                }

                var updatedQuiz = await _quizService.UpdateQuizAsync(id, updateQuizDataDto);
                return Ok(updatedQuiz);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // Delete a quiz
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                await _quizService.DeleteQuizAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}