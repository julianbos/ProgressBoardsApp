using System.ComponentModel.DataAnnotations;

namespace ProgressBoardsData.Models
{
	public class Task
	{
		[Key]
		public int TaskId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public Status Status { get; set; } = Status.TODO;
		public DateTime DueDate { get; set; } = DateTime.UtcNow;
		public int? AssignedUserId { get; set; }
		public virtual User AssignedUser { get; set; }
		public int ReporterId {  get; set; }
		public virtual User Reporter { get; set; }
		public int ProjectId { get; set; }
		public virtual Project Project { get; set; }
	}

	public enum Status
	{
		TODO,
		DOING,
		DONE
	}
}
