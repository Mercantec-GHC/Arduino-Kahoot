namespace API.Models
{
    public class Answer : Common
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionID { get; set; }
    }
    public class CreateAnswerDTO
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionID { get; set; }
    }
    public class AnswerResponseDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string QuestionID { get; set; }
        public bool IsCorrect { get; set; }
    }
}
