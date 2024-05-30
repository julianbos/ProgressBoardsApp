using ProgressBoardsShared.Dtos;

namespace ProgressBoardsServices.Interfaces
{
	public interface ITaskService
	{
		Task<IEnumerable<TaskDto>> GetTasksAsync(int projectId);
		Task<TaskDto> GetTaskAsync(int taskId);
		Task<TaskDto> CreateTaskAsync(TaskDto taskDto);
		Task<TaskDto> EditTaskAsync(int taskId, TaskDto taskDto);
		Task<bool> DeleteTaskAsync(int taskId);
	}
}
