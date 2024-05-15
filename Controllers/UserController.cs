using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BisleriumServer.Data;
using BisleriumServer.Model;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BisleriumServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized("User is not authenticated");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new { user.Id, user.Username, user.Email });
        }

        // PUT: api/user/update-username
        [HttpPut("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto model)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized("User is not authenticated");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Username = model.NewUsername;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/user/update-password
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized("User is not authenticated");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Here you should hash the new password before saving it
            user.Password = HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return NoContent();
        }

                // DELETE: api/user
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Account deleted successfully.");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }

        private string HashPassword(string password)
        {
            // Implement password hashing here
            return password; // Placeholder for hash function
        }

        
    }
}
