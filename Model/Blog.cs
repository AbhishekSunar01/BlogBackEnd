using System.ComponentModel.DataAnnotations;

namespace BisleriumServer.Model
{
	public class Blog
	{
		[Key]
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? ImagePath { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public int UserId { get; set; }
		public User? User { get; set; }
		public ICollection<Like>? Likes { get; set; }
		public int LikesCount { get; set; }
		public ICollection<Dislike>? Dislikes { get; set; }
		public int DislikesCount { get; set; }
		public ICollection<Comment>? Comments { get; set; }
	}
}
