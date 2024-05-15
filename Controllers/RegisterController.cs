using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BisleriumServer.Model.DTO;
using BisleriumServer.Data;
using BisleriumServer.Model;

namespace BisleriumServer.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }
        // POST: api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username is already taken
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    return Conflict("Username already exists");
                }

                // Check if the email is already taken
                existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Conflict("Email already exists");
                }

                // Create a new user from the register model
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };

                // Add the new user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok("User registered successfully");
            }

            return BadRequest(ModelState);
        }

    }
}
