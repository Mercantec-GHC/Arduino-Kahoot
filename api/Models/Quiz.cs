namespace API.Models
{
    public class Quiz : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserQuiz> UserQuizzes { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    
    public class CreateQuizDTO
    {
            public string Name { get; set; }
            public string Description { get; set; }
    }
}
