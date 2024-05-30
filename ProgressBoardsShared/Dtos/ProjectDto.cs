namespace ProgressBoardsShared.Dtos
{
	public class ProjectDto
	{
		public int ProjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int CreatedByUserId { get; set; }
		public List<ProjectUserDto> ProjectUsers { get; set; } = new List<ProjectUserDto>();
	}
}
