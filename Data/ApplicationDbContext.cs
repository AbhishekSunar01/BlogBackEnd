using Microsoft.EntityFrameworkCore;
using BisleriumServer.Model;

namespace BisleriumServer.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Blog> Blogs { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Dislike> Dislikes { get; set; }
		public DbSet<CommentLike> CommentLikes { get; set; }
		public DbSet<CommentDislike> CommentDislikes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configuration for the Blog and User relationship
			modelBuilder.Entity<Blog>()
				.HasOne(b => b.User)
				.WithMany(u => u.Blogs)
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Cascade);  // Cascade delete blogs when a user is deleted

			// Configuration for the Blog and Comments relationship
			modelBuilder.Entity<Blog>()
				.HasMany(b => b.Comments)
				.WithOne(c => c.Blog)
				.HasForeignKey(c => c.BlogId)
				.OnDelete(DeleteBehavior.Cascade);  // Cascade delete comments when a blog is deleted

			// Configuration for the Comment and User relationship
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // No action on user deletion to avoid multiple cascade paths

			// Configuration for Likes relationship
			modelBuilder.Entity<Blog>()
				.HasMany(b => b.Likes)
				.WithOne(l => l.Blog)
				.HasForeignKey(l => l.BlogId)
				.OnDelete(DeleteBehavior.Cascade);  // Cascade delete likes when a blog is deleted

			modelBuilder.Entity<User>()
				.HasMany(u => u.Likes)
				.WithOne(l => l.User)
				.HasForeignKey(l => l.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // No action on user deletion to avoid multiple cascade paths

			// Configuration for Dislikes relationship
			modelBuilder.Entity<Blog>()
				.HasMany(b => b.Dislikes)
				.WithOne(d => d.Blog)
				.HasForeignKey(d => d.BlogId)
				.OnDelete(DeleteBehavior.Cascade);  // Cascade delete dislikes when a blog is deleted

			modelBuilder.Entity<User>()
				.HasMany(u => u.Dislikes)
				.WithOne(d => d.User)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // No action on user deletion to avoid multiple cascade paths

			modelBuilder.Entity<Comment>()
					.HasMany(c => c.Likes)
					.WithOne(cl => cl.Comment)
					.HasForeignKey(cl => cl.CommentId)
					.OnDelete(DeleteBehavior.Cascade);  // Cascade delete comment likes when a comment is deleted

			modelBuilder.Entity<User>()
				.HasMany(u => u.CommentLikes)
				.WithOne(cl => cl.User)
				.HasForeignKey(cl => cl.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // No action on user deletion to avoid multiple cascade paths

			// Configuration for Comment Dislikes relationship
			modelBuilder.Entity<Comment>()
				.HasMany(c => c.Dislikes)
				.WithOne(cd => cd.Comment)
				.HasForeignKey(cd => cd.CommentId)
				.OnDelete(DeleteBehavior.Cascade);  // Cascade delete comment dislikes when a comment is deleted

			modelBuilder.Entity<User>()
				.HasMany(u => u.CommentDislikes)
				.WithOne(cd => cd.User)
				.HasForeignKey(cd => cd.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // No action on user deletion to avoid multiple cascade paths
		}
	}
}
