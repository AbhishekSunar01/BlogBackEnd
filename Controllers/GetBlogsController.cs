
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using BisleriumServer.Data;
// using BisleriumServer.Model;

// namespace BisleriumServer.Controllers
// {
// 	[ApiController]
// 	[Route("api/[controller]")]
// 	public class GetBlogsController : ControllerBase
// 	{
// 		private readonly ApplicationDbContext _context;

// 		public GetBlogsController(ApplicationDbContext context)
// 		{
// 			_context = context;
// 		}

// 		// Get random blogs
// 		[HttpGet("random")]
// 		public async Task<IActionResult> GetRandomBlogs()
// 		{
// 			var blogs = await _context.Blogs
// 				.OrderBy(b => Guid.NewGuid())
// 				.Take(10)
// 				.Include(b => b.User)
// 				.Include(b => b.Comments)
// 				.ToListAsync();

// 			return Ok(blogs.Select(b => MapBlogToDto(b)));
// 		}

// 		// Get blogs by recency
// 		[HttpGet("recent")]
// 		public async Task<IActionResult> GetRecentBlogs()
// 		{
// 			var blogs = await _context.Blogs
// 				.OrderByDescending(b => b.CreatedAt)
// 				.Take(10)
// 				.Include(b => b.User)
// 				.Include(b => b.Comments)
// 				.ToListAsync();

// 			return Ok(blogs.Select(b => MapBlogToDto(b)));
// 		}

// 		// Get blogs by popularity (likes and comments)
// 		[HttpGet("popular")]
// 		public async Task<IActionResult> GetPopularBlogs()
// 		{
// 			var blogs = await _context.Blogs
// 				.Include(b => b.User)
// 				.Include(b => b.Comments)
// 				.OrderByDescending(b => b.Likes.Count + b.Comments.Count)
// 				.Take(10)
// 				.ToListAsync();

// 			return Ok(blogs.Select(b => MapBlogToDto(b)));
// 		}

// 		private dynamic MapBlogToDto(Blog blog)
// 		{
// 			return new
// 			{
// 				Id = blog.Id,
// 				Title = blog.Title,
// 				Description = blog.Description,
// 				CreatedAt = blog.CreatedAt,
// 				Username = blog.User?.Username,
// 				LikesCount = blog.Likes?.Count ?? 0,
// 				DislikesCount = blog.Dislikes?.Count ?? 0,
// 				CommentsCount = blog.Comments?.Count ?? 0,
// 				Comments = blog.Comments?.Select(c => new {
// 					c.Id,
// 					c.Text,
// 					UserName = c.User?.Username,
// 					LikesCount = c.Likes?.Count ?? 0,
// 					DislikesCount = c.Dislikes?.Count ?? 0
// 				}),
// 				ImageUrl = blog.ImagePath
// 			};
// 		}
// 	}
// }


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
                .Include(b => b.Likes)
                .Include(b => b.Dislikes)
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
                .Include(b => b.Likes)
                .Include(b => b.Dislikes)
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
                .Include(b => b.Likes)
                .Include(b => b.Dislikes)
                .OrderByDescending(b => b.Likes.Count + b.Comments.Count)
                .Take(10)
                .ToListAsync();

            return Ok(blogs.Select(b => MapBlogToDto(b)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id)
                .Include(b => b.User)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .Include(b => b.Likes)
                .Include(b => b.Dislikes)
                .Select(b => MapBlogToDto(b))
                .FirstOrDefaultAsync();

            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            return Ok(blog);
        }

        // private dynamic MapBlogToDto(Blog blog)
        // {
        //     return new
        //     {
        //         Id = blog.Id,
        //         Title = blog.Title,
        //         Description = blog.Description,
        //         CreatedAt = blog.CreatedAt,
        //         Username = blog.User?.Username,
        //         LikesCount = blog.Likes?.Count() ?? 0,
        //         DislikesCount = blog.Dislikes?.Count() ?? 0,
        //         CommentsCount = blog.Comments?.Count() ?? 0,
        //         Comments = blog.Comments?.Select(c => new {
        //             c.Id,
        //             c.Text,
        //             UserName = c.User?.Username,
        //             LikesCount = c.Likes?.Count() ?? 0,
        //             DislikesCount = c.Dislikes?.Count() ?? 0
        //         }).ToList(),
        //         ImageUrl = blog.ImagePath
        //     };
        // }

         private static dynamic MapBlogToDto(Blog blog)
        {
            return new
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                CreatedAt = blog.CreatedAt,
                Username = blog.User?.Username,
                LikesCount = blog.Likes?.Count() ?? 0,
                DislikesCount = blog.Dislikes?.Count() ?? 0,
                CommentsCount = blog.Comments?.Count() ?? 0,
                Comments = blog.Comments?.Select(c => new {
                    c.Id,
                    c.Text,
                    UserName = c.User?.Username,
                    LikesCount = c.Likes?.Count() ?? 0,
                    DislikesCount = c.Dislikes?.Count() ?? 0
                }).ToList(),
                ImageUrl = blog.ImagePath
            };
        }
    }
}
