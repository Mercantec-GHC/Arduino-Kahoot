using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
                : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
    }
}
