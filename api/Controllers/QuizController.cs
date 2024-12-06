namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public QuizController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            return await _context.Quizzes
                .Include(q => q.UserQuizzes)
                .Include(q => q.Questions)
                    .ThenInclude(question => question.Answers)
                .ToListAsync();
        }

        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(string id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.UserQuizzes)
                .Include(q => q.Questions)
                    .ThenInclude(question => question.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return quiz;
        }

        // POST:api/Quiz
        [HttpPost("createQuiz")]
        public async Task<ActionResult<Quiz>> CreateQuiz(CreateQuizDTO createQuizDTO)
        {
            // Map DTO to the Quiz model
            var quiz = new Quiz
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = createQuizDTO.Name,
                Description = createQuizDTO.Description,
                IsPublic = createQuizDTO.IsPublic,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        }

        // PUT: api/Quiz/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(string id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return BadRequest();
            }

            quiz.UpdatedAt = DateTime.UtcNow; // Update timestamp
            _context.Entry(quiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Quiz/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(string id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private Quiz MapCreateQuizDTO(CreateQuizDTO createQuizDTO)
        {
            return new Quiz
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = createQuizDTO.Name,
                Description = createQuizDTO.Description,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
        }
        private bool QuizExists(string id)
        {
            return _context.Quizzes.Any(q => q.Id == id);
        }
    }
}
