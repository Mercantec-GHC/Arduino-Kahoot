using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {

        private readonly AppDBContext _context;

        public AnswerController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Answer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerResponseDTO>>> GetAnswers()
        {
            var answers = await _context.Answers
                .ToListAsync();

            return answers.Select(q => new AnswerResponseDTO
            {
                Id = q.Id,
                Content = q.Content,
                QuestionID = q.QuestionID
            }).ToList();
        }

        // GET api/Answers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerResponseDTO>> GetAnswer(string id)
        {
            var answer = await _context.Answers
                .FirstOrDefaultAsync(q => q.Id == id);

            if (answer == null)
            {
                return NotFound();
            }

            return new AnswerResponseDTO
            {
                Id = answer.Id,
                Content = answer.Content,
                QuestionID = answer.QuestionID,
            };
        }

        // POST api/Answers
        [HttpPost("createAnswer")]
        public async Task<ActionResult<Question>> PostQuestion([FromBody] CreateAnswerDTO createAnswerDTO)
        {

            if (string.IsNullOrEmpty(createAnswerDTO.QuestionID) || string.IsNullOrEmpty(createAnswerDTO.Content))
            {
                return BadRequest("QuestionID and Content are required.");
            }

            var question = await _context.Questions
                .Include(q => q.Answers) // Include related answers
                .FirstOrDefaultAsync(q => q.Id == createAnswerDTO.QuestionID);

            var answer = new Answer
            {
                Id = Guid.NewGuid().ToString("N"),
                Content = createAnswerDTO.Content,
                QuestionID = createAnswerDTO.QuestionID,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };

            question.Answers.Add(answer);

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return Ok($"Question added to quiz {question.Content}");
        }

        // PUT api/<AnswerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AnswerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
