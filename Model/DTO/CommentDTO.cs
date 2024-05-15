namespace BisleriumServer.Model.DTO
{
	public class CommentCreateDto
	{
		public string Text { get; set; }
		public int BlogId { get; set; }  // Assuming the BlogId is needed to link the comment to the blog
	}

	public class CommentReadDto
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public int LikesCount { get; set; }
		public int DislikesCount { get; set; }
		public SimpleUserDto User { get; set; }  // Reduced user information
		public SimpleBlogDto Blog { get; set; }  // Reduced blog information
	}

	public class SimpleUserDto
	{
		public int Id { get; set; }
		public string Username { get; set; }
	}

	public class SimpleBlogDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
	}

	public class CommentUpdateDto
	{
		public string Text { get; set; }
	}


}
