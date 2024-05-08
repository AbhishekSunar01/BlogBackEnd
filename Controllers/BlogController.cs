﻿using BisleriumServer.Data;
using BisleriumServer.Model;
using BisleriumServer.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BisleriumServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ApplicationDbContext context, ILogger<BlogController> logger)
        {
            _context = context;
            _logger = logger;
        }

		[HttpPost]
		public async Task<IActionResult> CreateBlog([FromForm] BlogModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);  // Create the directory if it doesn't exist
			}

			string filePath = null;
			if (model.Image != null)
			{
				filePath = Path.Combine(directoryPath, model.Image.FileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await model.Image.CopyToAsync(stream);
				}
			}

			var blog = new Blog
			{
				Title = model.Title,
				Description = model.Description,
				ImagePath = filePath,
				UserId = GetCurrentUserId() // Implement your method to get the current user ID
			};

			_context.Blogs.Add(blog);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetBlog", new { id = blog.Id }, blog);
		}



		[HttpGet("{id}")]
		public async Task<ActionResult<CommentReadDto>> GetComment(int id)
		{
			var comment = await _context.Comments
				.Include(c => c.User)
				.Include(c => c.Blog)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (comment == null)
			{
				return NotFound($"Comment with ID {id} not found.");
			}

			var commentReadDto = new CommentReadDto
			{
				Id = comment.Id,
				Text = comment.Text,
				LikesCount = comment.Likes.Count,  // Ensure these navigation properties are configured
				DislikesCount = comment.Dislikes.Count,
				User = new SimpleUserDto
				{
					Id = comment.User.Id,
					Username = comment.User.Username
				},
				Blog = new SimpleBlogDto
				{
					Id = comment.Blog.Id,
					Title = comment.Blog.Title
				}
			};

			return Ok(commentReadDto);
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


        // PUT: api/blog/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            // Update the blog properties
            blog.Title = model.Title;
            blog.Description = model.Description;

            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
