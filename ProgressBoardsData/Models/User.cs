using System.ComponentModel.DataAnnotations;

namespace ProgressBoardsData.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }

		public virtual ICollection<ProjectUser> Projects { get; set; } = new List<ProjectUser>();
		public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
		public virtual ICollection<Task> ReportedTasks { get; set; } = new List<Task>();
	}
}