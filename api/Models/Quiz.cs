namespace API.Models
{
    public class Quiz : Common
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class QuizDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    public class CreateQuizDTO
    {

    }
}
