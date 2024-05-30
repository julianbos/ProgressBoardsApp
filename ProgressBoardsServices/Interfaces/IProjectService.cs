using ProgressBoardsShared.Dtos;

namespace ProgressBoardsServices.Interfaces
{
	public interface IProjectService
	{
		Task<IEnumerable<ProjectDto>> GetProjectsAsync(int userId);
		Task<ProjectDto> GetProjectAsync(int projectId);
		Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);
		Task<ProjectDto> EditProjectAsync(int projectId, ProjectDto projectDto);
		Task<bool> DeleteProjectAsync(int projectId);
	}
}
