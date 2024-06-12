using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProgressBoardsData;
using ProgressBoardsData.Models;

namespace ProgressBoardsServices.Implementations
{
	public class TaskService : ITaskService
	{
		private readonly ApplicationDbContext _context;
		private readonly INotificationService _notificationService;

		public TaskService(ApplicationDbContext context, INotificationService notificationService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_notificationService = notificationService;
		}

		public async Task<IEnumerable<TaskDto>> GetTasksAsync(int projectId)
		{
			var tasks = await _context.Tasks
				.Include(t => t.AssignedUser)
				.Include(t => t.Reporter)
				.Where(t => t.ProjectId == projectId)
				.Select(t => new TaskDto
				{
					TaskId = t.TaskId,
					Title = t.Title,
					Description = t.Description,
					Status = (StatusDto)t.Status,
					DueDate = t.DueDate,
					AssignedUserId = t.AssignedUserId,
					ReporterId = t.ReporterId,
					ProjectId = t.ProjectId
				}).ToListAsync();

			return tasks;
		}

		public async Task<TaskDto> GetTaskAsync(int taskId)
		{
			var task = await _context.Tasks
				.Include(t => t.AssignedUser)
				.Include(t => t.Reporter)
				.Where(t => t.TaskId == taskId)
				.Select(t => new TaskDto
				{
					TaskId = t.TaskId,
					Title = t.Title,
					Description = t.Description,
					Status = (StatusDto)t.Status,
					DueDate = t.DueDate,
					AssignedUserId = t.AssignedUserId,
					ReporterId = t.ReporterId,
					ProjectId = t.ProjectId
				})
				.FirstOrDefaultAsync();

			return task;
		}

		public async Task<TaskDto> CreateTaskAsync(TaskDto taskDto)
		{
			var newTask = new ProgressBoardsData.Models.Task
            {
				Title = taskDto.Title,
				Description = taskDto.Description,
				Status = (Status)taskDto.Status,
				DueDate = taskDto.DueDate,
				AssignedUserId = taskDto.AssignedUserId,
				ReporterId = taskDto.ReporterId,
				ProjectId = taskDto.ProjectId
			};

			await _context.Tasks.AddAsync(newTask);
			await _context.SaveChangesAsync();

			taskDto.TaskId = newTask.TaskId;

			if (taskDto.AssignedUserId.HasValue)
			{
				await _notificationService.NotifyTaskAssignmentAsync(taskDto.AssignedUserId.Value, taskDto.Title);
			}

			return taskDto;
		}

		public async Task<TaskDto> EditTaskAsync(int taskId, TaskDto taskDto)
		{
			var task = await _context.Tasks.FindAsync(taskId);

			if (task != null)
			{
				if(taskDto.AssignedUserId != null && (taskDto.AssignedUserId != task.AssignedUserId))
				{
					await _notificationService.NotifyTaskAssignmentAsync((int)taskDto.AssignedUserId, taskDto.Title);
				}

				task.Title = taskDto.Title;
				task.Description = taskDto.Description;
				task.Status = (Status)taskDto.Status;
				task.DueDate = taskDto.DueDate;
				task.AssignedUserId = taskDto.AssignedUserId;
				task.ReporterId = taskDto.ReporterId;

				_context.SaveChanges();
			}

			return taskDto;
		}

		public async Task<bool> DeleteTaskAsync(int taskId)
		{
			var task = await _context.Tasks.FindAsync(taskId);

			if(task != null)
			{
				_context.Tasks.Remove(task);
				_context.SaveChanges();
				return true;
			}
			return false;

		}
	}
}
