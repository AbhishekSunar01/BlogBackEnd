// using BisleriumServer.Data;
// using BisleriumServer.Model;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;


// namespace BisleriumServer.Controllers
// {
// 	[Authorize]
// 	[ApiController]
// 	[Route("api/[controller]")]
// 	public class LikeBlogController : Controller
// 	{
// 		private readonly ApplicationDbContext _context;

// 		public LikeBlogController(ApplicationDbContext context)
// 		{
// 			_context = context;
// 		}

// 		// POST: api/LikeBlog
// 		[HttpPost]
// 		public async Task<IActionResult> ToggleLike(int blogId, int userId)
// 		{

// 			// Check if the blog exists
// 			var blog = await _context.Blogs.FindAsync(blogId);
// 			if (blog == null)
// 			{
// 				return NotFound();
// 			}

// 			// Check if the user has already liked the blog
// 			var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);
// 			if (existingLike != null)
// 			{
// 				// If a like exists, remove it and decrement the likesCount
// 				_context.Likes.Remove(existingLike);
// 				blog.LikesCount--;
// 			}
// 			else
// 			{
// 				// If no like exists, create a new one and increment the likesCount
// 				var like = new Like
// 				{
// 					BlogId = blogId,
// 					UserId = userId
// 				};

// 				_context.Likes.Add(like);
// 				blog.LikesCount++;
// 			}

// 			await _context.SaveChangesAsync();

// 			return Ok();
// 		}


// 	}
// }

using BisleriumServer.Data;
using BisleriumServer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BisleriumServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LikeBlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikeBlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/LikeBlog
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int blogId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Check if the blog exists
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            // Check if the user has already liked the blog
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);
            if (existingLike != null)
            {
                // If a like exists, remove it
                _context.Likes.Remove(existingLike);
            }
            else
            {
                // If no like exists, create a new one
                var like = new Like
                {
                    BlogId = blogId,
                    UserId = userId
                };
                _context.Likes.Add(like);
            }

            await _context.SaveChangesAsync();

            // Return the current count of likes to ensure client-side consistency
            var currentLikesCount = await _context.Likes.CountAsync(l => l.BlogId == blogId);
            return Ok(new { LikesCount = currentLikesCount });
        }
    }
}
