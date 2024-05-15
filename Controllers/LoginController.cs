using BisleriumServer.Data;
using BisleriumServer.Model;
using BisleriumServer.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BisleriumServer.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/user/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser == null)
                {
                    return NotFound("User not found");
                }

                // Check if the password is correct
                if (existingUser.Password != model.Password)
                {
                    return Unauthorized("Invalid password");
                }

                // Create and save the token
                string token = CreateToken(existingUser);
                existingUser.Token = token; // Update the Token property of the User entity

                // Add the user to the context (assuming you want to update the user in the database)
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                var response = new LoginResponse
                {
                    Username = existingUser.Username,
                    Token = token,
                    Role = existingUser.Role,
                    UserId = existingUser.Id
                };

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        public class LoginResponse
        {
            public string Username { get; set; }
            public string Token { get; set; }
            public string Role { get; set; }
            public int UserId { get; set; }
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            // Generate a secure key if not already set
            var tokenKey = _configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(tokenKey))
            {
                tokenKey = GenerateSecureKey(64); // Generate a new secure key if not set
                _configuration["AppSettings:Token"] = tokenKey; // Update the configuration with the new key
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                 claims: claims,
                 expires: DateTime.Now.AddDays(1),
                 signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // The cookie is not accessible via JavaScript
                Secure = true, // The cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // The cookie is sent only to the same site as the one that originated it
                Expires = DateTime.Now.AddDays(1) // The cookie expires after 1 day
            };

            Response.Cookies.Append("jwt", jwt, cookieOptions);

            return jwt;
        }

        public static string GenerateSecureKey(int length)
        {
            using var rng = new RNGCryptoServiceProvider();
            var randomNumber = new byte[length];
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
