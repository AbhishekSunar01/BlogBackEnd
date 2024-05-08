using BisleriumServer.Data;
using BisleriumServer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BisleriumServer.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class DislikeBlogController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DislikeBlogController(ApplicationDbContext context)
		{
			_context = context;
		}

		// POST: api/LikeBlog
		[HttpPost]
		public async Task<IActionResult> ToggleDislike(int blogId, int userId)
		{

			// Check if the blog exists
			var blog = await _context.Blogs.FindAsync(blogId);
			if (blog == null)
			{
				return NotFound();
			}

			// Check if the user has already disliked the blog
			var existingDislike = await _context.Dislikes.FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);
			if (existingDislike != null)
			{
				// If a dislike exists, remove it and decrement the dislikesCount
				_context.Dislikes.Remove(existingDislike);
				blog.DislikesCount--;
			}
			else
			{
				// If no dislike exists, create a new one and increment the dislikesCount
				var dislike = new Dislike
				{
					BlogId = blogId,
					UserId = userId
				};

				_context.Dislikes.Add(dislike);
				blog.DislikesCount++;
			}

			await _context.SaveChangesAsync();

			return Ok();
		}


	}
}
