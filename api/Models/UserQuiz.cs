namespace API.Models
{
    public class UserQuiz : Common
    {
        public string UserID { get; set; }
        public string QuizID {  get; set; }

        public User user { get; set; }
        public Quiz quiz { get; set; }
    }

    public class UserQuizDTO
    {
        public string UserID { get; set; }
        public string QuizID { get; set; }
    }
}
