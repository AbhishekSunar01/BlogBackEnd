/*using BisleriumServer.Data;
using BisleriumServer.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BisleriumServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GetBlogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GetBlogsController(ApplicationDbContext context)
        {
            _context = context;
        }

		// GET: api/blog
		[HttpGet]
		public async Task<IActionResult> GetAllBlogs()
		{
			var blogs = await _context.Blogs
				.Include(b => b.User)
				.Include(b => b.Comments) // Make sure to include comments
				.ToListAsync();

			if (blogs == null || !blogs.Any())
			{
				return NotFound("No blogs found.");
			}

			var blogDTOs = blogs.Select(b => new BlogDTO
			{
				Id = b.Id,
				Title = b.Title,
				Description = b.Description,
				CreatedAt = b.CreatedAt,
				Username = b.User.Username,
				LikesCount = b.LikesCount,
				DislikesCount = b.DislikesCount,
				CommentsCount = b.Comments.Count, // Calculate the count of comments
				Comments = b.Comments.Select(c => new SimpleCommentDto
				{
					Id = c.Id,
					Text = c.Text
				}).ToList() // Map comments if necessary
			}).ToList();

			return Ok(blogDTOs);
		}



	}
}
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BisleriumServer.Data;
using BisleriumServer.Model;

namespace BisleriumServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GetBlogsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public GetBlogsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Get random blogs
		[HttpGet("random")]
		public async Task<IActionResult> GetRandomBlogs()
		{
			var blogs = await _context.Blogs
				.OrderBy(b => Guid.NewGuid())
				.Take(10)
				.Include(b => b.User)
				.Include(b => b.Comments)
				.ToListAsync();

			return Ok(blogs.Select(b => MapBlogToDto(b)));
		}

		// Get blogs by recency
		[HttpGet("recent")]
		public async Task<IActionResult> GetRecentBlogs()
		{
			var blogs = await _context.Blogs
				.OrderByDescending(b => b.CreatedAt)
				.Take(10)
				.Include(b => b.User)
				.Include(b => b.Comments)
				.ToListAsync();

			return Ok(blogs.Select(b => MapBlogToDto(b)));
		}

		// Get blogs by popularity (likes and comments)
		[HttpGet("popular")]
		public async Task<IActionResult> GetPopularBlogs()
		{
			var blogs = await _context.Blogs
				.Include(b => b.User)
				.Include(b => b.Comments)
				.OrderByDescending(b => b.Likes.Count + b.Comments.Count)
				.Take(10)
				.ToListAsync();

			return Ok(blogs.Select(b => MapBlogToDto(b)));
		}

		private dynamic MapBlogToDto(Blog blog)
		{
			return new
			{
				Id = blog.Id,
				Title = blog.Title,
				Description = blog.Description,
				CreatedAt = blog.CreatedAt,
				Username = blog.User?.Username,
				LikesCount = blog.Likes?.Count ?? 0,
				DislikesCount = blog.Dislikes?.Count ?? 0,
				CommentsCount = blog.Comments?.Count ?? 0,
				Comments = blog.Comments?.Select(c => new {
					c.Id,
					c.Text,
					UserName = c.User?.Username,
					LikesCount = c.Likes?.Count ?? 0,
					DislikesCount = c.Dislikes?.Count ?? 0
				}),
				ImageUrl = blog.ImagePath
			};
		}
	}
}
