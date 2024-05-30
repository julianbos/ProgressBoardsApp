using System.ComponentModel.DataAnnotations;

namespace ProgressBoardsData.Models
{
	public class Project
	{
		[Key]
		public int ProjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int CreatedByUserId { get; set; }
		public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
		public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
	}
}
