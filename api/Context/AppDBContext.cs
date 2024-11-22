using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context
{
    public class AppDBContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }        
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserQuiz> UserQuizzes { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options)
                : base(options)
        {
        }
    }
}
