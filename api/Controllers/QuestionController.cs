
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using API.Context;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {

        private readonly AppDBContext _context;

        public QuestionController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionResponseDTO>>> GetQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Answers)
                .ToListAsync();

            return questions.Select(q => new QuestionResponseDTO
            {
                Id = q.Id,
                Content = q.Content,
                QuizID = q.QuizID,
                Answers = q.Answers.Select(a => new Answer
                {
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    Id = a.Id
                }).ToList()
            }).ToList();
        }

        // GET: api/Questions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionResponseDTO>> GetQuestion(string id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return new QuestionResponseDTO
            {
                Id = question.Id,
                Content = question.Content,
                QuizID = question.QuizID,
                Answers = question.Answers.Select(a => new Answer
                {
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    Id = a.Id
                }).ToList()
            };
        }

        // POST: api/Questions
        [HttpPost("createQuestion")]
        public async Task<ActionResult<Question>> PostQuestion([FromBody]CreateQuestionDTO createQuestionDTO)
        {

            if (string.IsNullOrEmpty(createQuestionDTO.QuizID) || string.IsNullOrEmpty(createQuestionDTO.Content))
            {
                return BadRequest("QuizID and Content are required.");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions) // Include related questions
                .FirstOrDefaultAsync(q => q.Id == createQuestionDTO.QuizID);

            var question = new Question
            {
                Id = Guid.NewGuid().ToString("N"),
                Content = createQuestionDTO.Content,
                QuizID = createQuestionDTO.QuizID,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };

            quiz.Questions.Add(question);

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok($"Question added to quiz {quiz.Name}");
        }

        // UPDATE: api/Questions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, CreateQuestionDTO updateQuestionDTO)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            // Update fields
            question.Content = updateQuestionDTO.Content;
            question.QuizID = updateQuestionDTO.QuizID;

            

            question.UpdatedAt = DateTime.UtcNow; // Update timestamp

            _context.Entry(question).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Questions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
