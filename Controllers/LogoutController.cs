using BisleriumServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BisleriumServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LogoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Get the user from the context (replace this with your actual logic to get the user)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == Request.Cookies["jwt"]);

            if (user == null)
            {
                return Unauthorized();
            }

            // Delete the token from the user table
            user.Token = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Delete the jwt cookie
            Response.Cookies.Delete("jwt");

            string message = "You have been logged out successfully";

            return Ok(message);
        }

    }
}
