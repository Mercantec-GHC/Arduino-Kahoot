namespace API.Models
{
    public class Question : Common
    {
        public string Content { get; set; }
        public string QuizID { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
