using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public QuizController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDTO>>> GetQuizzes()
        {
            var Quiz = await _context.Quizzes.Select(quiz => new QuizDTO
            {
                Id = quiz.Id,
                Name = quiz.Name,
                Description = quiz.Description
            }).ToListAsync();

            return Ok(Quiz);
        }


    }
}
