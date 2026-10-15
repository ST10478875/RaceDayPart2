using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.models;

namespace RaceDay.API.Controllers
{//start of namespace
   
    // Handles user registration, password hashing, and login authentication for Organisers and Participants.
  
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {//start of class

        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        
        // Registers a new user with a hashed password and specified role ("Organiser" or "Participant").
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validate unique email/username
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email is already registered." });
            }

            // Create user entity
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role // Must be "Organiser" or "Participant"
            };

            // Securely hash the password before saving
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful!", userId = user.Id, user.Role });
        }

        
        // Authenticates a user against their stored password hash and returns their role profile.
       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Verify the incoming password against the stored hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // In a full session setup, session tokens or cookies would be established here
            return Ok(new { message = "Login successful!", userId = user.Id, username = user.Username, role = user.Role });
        }
    }

    // Data Transfer Objects (DTOs) for incoming request payloads
    public class RegisterDto
    {//start of class

        //properties for that stores
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Participant"; // Default to Participant

    }//end of class

    public class LoginDto
    {//start of class

        //properties that stores
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }//end of class
}//end of namespace