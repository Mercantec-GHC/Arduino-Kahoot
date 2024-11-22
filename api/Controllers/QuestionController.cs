
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
                Answers = q.Answers.Select(a => new AnswerDTO
                {
                    Text = a.AnswerContext,
                    IsCorrect = a.IsCorrect
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
                Answers = question.Answers.Select(a => new AnswerDTO
                {
                    AnswerContext = a.AnswerContext,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };
        }

        // POST api/<QuestionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // POST: api/Questions
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDTO createQuestionDTO)
        {
            var question = new Question
            {
                Content = createQuestionDTO.Content,
                QuizID = createQuestionDTO.QuizID,
                Answers = createQuestionDTO.Answers.Select(a => new Answer
                {
                    AnswerContext = a.AnswerContext,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

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

            // Update answers
            question.Answers.Clear();
            question.Answers.AddRange(updateQuestionDTO.Answers.Select(a => new Answer
            {
                AnswerContext = a.AnswerContext,
                IsCorrect = a.IsCorrect
            }));

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
