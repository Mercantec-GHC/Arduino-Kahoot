namespace API.Models
{
    public class Quiz : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserQuiz> UserQuizzes { get; set; }
        public List<Answer> Answers { get; set; }
    }

    
    public class CreateQuizDTO
    {
            public string Name { get; set; }
            public string Description { get; set; }
    }
}
