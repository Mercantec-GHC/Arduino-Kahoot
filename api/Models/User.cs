namespace API.Models
{
    public class User : Common
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public string PasswordBackdoor { get; set; }
        // Remove PasswordBackdoor before commercial use
        public required string HashedPassword { get; set; }
        public required string Salt { get; set; }
        public DateTime LastLogin { get; set; }
        

        public List<Quiz> UserQuizzes { get; set; }
    }

    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class CreateUserDTO
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DeleteUserDTO
    {

    }
}
