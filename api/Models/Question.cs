namespace API.Models
{
    public class Question : Common
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
