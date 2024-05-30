using Microsoft.EntityFrameworkCore;
using ProgressBoardsData.Models;

namespace ProgressBoardsData
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Models.Task> Tasks { get; set; }
		public DbSet<ProjectUser> ProjectUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Models.Task>()
				.HasOne(t => t.AssignedUser)
				.WithMany(u => u.Tasks)
				.HasForeignKey(t => t.AssignedUserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Models.Task>()
				.HasOne(t => t.Reporter)
				.WithMany(u => u.ReportedTasks)
				.HasForeignKey(t => t.ReporterId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ProjectUser>()
				.HasKey(pu => new { pu.ProjectId, pu.UserId });

			modelBuilder.Entity<ProjectUser>()
				.HasOne(pu => pu.Project)
				.WithMany(p => p.ProjectUsers)
				.HasForeignKey(pu => pu.ProjectId);

			modelBuilder.Entity<ProjectUser>()
				.HasOne(pu => pu.User)
				.WithMany(u => u.Projects)
				.HasForeignKey(pu => pu.UserId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
