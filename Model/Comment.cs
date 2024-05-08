using System.ComponentModel.DataAnnotations;

namespace BisleriumServer.Model
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
		public ICollection<CommentLike> Likes { get; set; }
		public ICollection<CommentDislike> Dislikes { get; set; }
		public int LikesCount { get; set; }
		public int DislikesCount { get; set; }
	}

	public class CommentLike
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int CommentId { get; set; }
		public Comment Comment { get; set; }
	}

	public class CommentDislike
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int CommentId { get; set; }
		public Comment Comment { get; set; }
	}
}
