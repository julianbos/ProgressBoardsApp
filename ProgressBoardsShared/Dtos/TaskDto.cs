namespace ProgressBoardsShared.Dtos
{
	public class TaskDto
	{
		public int TaskId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public StatusDto Status { get; set; } = StatusDto.TODO;
		public DateTime DueDate { get; set; } = DateTime.UtcNow;
		public int? AssignedUserId { get; set; }
		public int ReporterId { get; set; }
		public int ProjectId { get; set; }
	}

}