using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (await _db.Users.AnyAsync(u => u.Username == user.Username))
            {
                return Conflict("Username already exists");
            }
            var newUser = new User
            {
                Username = user.Username,
                PasswordHash = _hasher.HashPassword(user, user.PasswordHash)
            };
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
            return Created("/", new { newUser.Id, newUser.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existing == null)
            {
                return Unauthorized();
            }
            var result = _hasher.VerifyHashedPassword(existing, existing.PasswordHash, user.PasswordHash);
            if (result == PasswordVerificationResult.Success)
            {
                return Ok(new { existing.Id, existing.Username });
            }
            return Unauthorized();
        }
    }
}
