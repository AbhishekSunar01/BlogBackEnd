namespace BisleriumServer.Model.DTO
{

	public class BlogDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Username { get; set; } // From associated User
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }

    public string ImagePath { get; set; }
    public List<CommentDTO> Comments { get; set; }
}

public class CommentDTO
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Username { get; set; } // From associated User
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
}

	// public class BlogDTO
	// {
	// 	public int Id { get; set; }
	// 	public string Title { get; set; }
	// 	public string Description { get; set; }
	// 	public DateTime CreatedAt { get; set; }
	// 	public string Username { get; set; }
	// 	public int LikesCount { get; set; }
	// 	public int DislikesCount { get; set; }
	// 	public int CommentsCount { get; set; }
	// 	// If you want to include some details about the comments:
	// 	public List<SimpleCommentDto> Comments { get; set; }
	// }

	// public class SimpleCommentDto
	// {
	// 	public int Id { get; set; }
	// 	public string Text { get; set; }
	// }

}
