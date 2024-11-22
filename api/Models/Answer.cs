namespace API.Models
{
    public class Answer : Common
    {
        public string AnswerContext { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionID { get; set; }
    }
    public class AnswerDTO
    {
        public string AnswerContext { get; set; }
        public bool IsCorrect { get; set; }
    }
}
