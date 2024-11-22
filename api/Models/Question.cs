﻿namespace API.Models
{
    public class Question : Common
    {
        public string Content { get; set; }
        public string QuizID { get; set; }
        public List<Answer> Answers { get; set; }
    }
    public class CreateQuestionDTO
    {
        public string Content { get; set; }
        public string QuizID { get; set; }
        public List<AnswerDTO> Answers { get; set; } = new List<AnswerDTO>();
    }
    public class QuestionResponseDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string QuizID { get; set; }
        public List<AnswerDTO> Answers { get; set; } = new List<AnswerDTO>();
    }
}
