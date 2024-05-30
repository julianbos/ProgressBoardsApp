using Microsoft.AspNetCore.Mvc;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ProgressBoardsApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TaskController : ControllerBase
	{
		private readonly ITaskService _taskService;

		public TaskController(ITaskService taskService)
		{
			_taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
		}

		[HttpGet("tasks/{projectId}")]
		public async Task<IActionResult> GetTasks(int projectId)
		{
			try
			{
				var tasks = await _taskService.GetTasksAsync(projectId);
				return Ok(tasks);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpGet("task/{taskId}")]
		public async Task<IActionResult> GetTask(int taskId)
		{
			try
			{
				var task = await _taskService.GetTasksAsync(taskId);

				if (task == null)
					return NotFound("Task not found.");

				return Ok(task);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");

			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
		{
			try
			{
				var createdTask = await _taskService.CreateTaskAsync(taskDto);
				return CreatedAtAction(nameof(GetTask), new { taskId = createdTask.TaskId }, createdTask);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");

			}
		}

		[HttpPut("{taskId}")]
		public async Task<IActionResult> EditTask(int taskId, [FromBody] TaskDto taskDto)
		{
			try
			{
				var editedTask = await _taskService.EditTaskAsync(taskId, taskDto);

				if (editedTask == null)
					return NotFound("Task not found.");

				return Ok(editedTask);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpDelete("{taskId}")]
		public async Task<IActionResult> DeleteTask(int taskId)
		{
			try
			{
				await _taskService.DeleteTaskAsync(taskId);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}
	}
}
