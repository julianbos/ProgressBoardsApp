using Microsoft.AspNetCore.Mvc;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ProgressBoardsApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ProjectController : ControllerBase
	{
		private readonly IProjectService _projectService;

		public ProjectController(IProjectService projectService)
		{
			_projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
		}

		[HttpGet("projects/{userId}")]
		public async Task<IActionResult> GetProjects(int userId)
		{
			try
			{
				var projects = await _projectService.GetProjectsAsync(userId);
				return Ok(projects);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpGet("project/{projectId}")]
		public async Task<IActionResult> GetProject(int projectId)
		{
			try
			{
				var project = await _projectService.GetProjectAsync(projectId);

				if (project == null)
					return NotFound("Project not found.");

				return Ok(project);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
		{
			try
			{
				var createdProject = await _projectService.CreateProjectAsync(projectDto);
				return CreatedAtAction(nameof(GetProject), new { projectId = createdProject.ProjectId }, createdProject);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPut("{projectId}")]
		public async Task<IActionResult> EditProject(int projectId, [FromBody] ProjectDto projectDto)
		{
			try
			{
				var editedProject = await _projectService.EditProjectAsync(projectId, projectDto);

				if (editedProject == null)
					return NotFound("Project not found.");

				return Ok(editedProject);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpDelete("{projectId}")]
		public async Task<IActionResult> DeleteProject(int projectId)
		{
			try
			{
				await _projectService.DeleteProjectAsync(projectId);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}
	}
}
