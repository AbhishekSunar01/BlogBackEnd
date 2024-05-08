using System.ComponentModel.DataAnnotations;

namespace BisleriumServer.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Dislike> Dislikes { get; set; }
        public ICollection<Comment> Comments { get; set; }
		public ICollection<CommentLike> CommentLikes { get; set; }
		public ICollection<CommentDislike> CommentDislikes { get; set; }

	}
}
