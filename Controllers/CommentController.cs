using Microsoft.AspNetCore.Mvc;
using BisleriumServer.Data;
using BisleriumServer.Model;
using BisleriumServer.Model.DTO;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BisleriumServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<BlogController> _logger;

		public CommentController(ApplicationDbContext context, ILogger<BlogController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpPost]
		public async Task<ActionResult<CommentReadDto>> CreateComment([FromBody] CommentCreateDto commentDto)
		{
			var userId = GetCurrentUserId();  // Ensure this method correctly extracts the user ID

			var comment = new Comment
			{
				Text = commentDto.Text,
				BlogId = commentDto.BlogId,
				UserId = userId,  // Set the user ID of the commenter
			};

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

			var user = await _context.Users.FindAsync(userId);
			var blog = await _context.Blogs.FindAsync(commentDto.BlogId);

			var commentReadDto = new CommentReadDto
			{
				Id = comment.Id,
				Text = comment.Text,
				LikesCount = 0,  // New comment will have no likes initially
				DislikesCount = 0,  // New comment will have no dislikes initially
				User = new SimpleUserDto
				{
					Id = user.Id,
					Username = user.Username
				},
				Blog = new SimpleBlogDto
				{
					Id = blog.Id,
					Title = blog.Title
				}
			};

			return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, commentReadDto);
		}


		// Helper method to get the current user's ID
		private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is authenticated.");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    _logger.LogInformation($"User ID claim found: {userIdClaim.Value}");
                    return int.Parse(userIdClaim.Value);
                }
                else
                {
                    _logger.LogWarning("User ID claim not found.");
                }
            }
            else
            {
                _logger.LogWarning("User is not authenticated.");
            }

            throw new InvalidOperationException("User is not authenticated or user ID claim is not found.");
        }

		[HttpGet("{id}")]
		public async Task<ActionResult<CommentReadDto>> GetComment(int id)
		{
			var comment = await _context.Comments
				.Include(c => c.User)  // Ensures User data is loaded
				.Include(c => c.Blog)  // Ensures Blog data is loaded
				.FirstOrDefaultAsync(c => c.Id == id);


			if (comment == null)
			{
				_logger.LogError($"Comment with ID {id} not found.");
				return NotFound($"Comment with ID {id} not found.");
			}

			var commentReadDto = new CommentReadDto
			{
				Id = comment.Id,
				Text = comment.Text,
				LikesCount = comment.Likes?.Count ?? 0,  // Safe navigation and coalescing operator
				DislikesCount = comment.Dislikes?.Count ?? 0,
				User = comment.User != null ? new SimpleUserDto
				{
					Id = comment.User.Id,
					Username = comment.User.Username
				} : null,  // Check if User is null
				Blog = comment.Blog != null ? new SimpleBlogDto
				{
					Id = comment.Blog.Id,
					Title = comment.Blog.Title
				} : null   // Check if Blog is null
			};

			_logger.LogInformation("Fetching comment ID: {Id}", id);
			if (comment == null)
			{
				_logger.LogError("No comment found with ID: {Id}", id);
				return NotFound($"Comment with ID {id} not found.");
			}
			_logger.LogInformation("Comment data: {Comment}, User: {UserId}, Blog: {BlogId}", comment.Text, comment.User?.Id, comment.Blog?.Id);



			return Ok(commentReadDto);
		}

	}
}
