using Microsoft.AspNetCore.Mvc;
using Moq;
using ProgressBoardsApi.Controllers;
using ProgressBoardsServices.Interfaces;
using ProgressBoardsShared.Dtos;
using Xunit;

namespace ProgressBoardsTests
{
	public class ProjectAndTaskTest
	{
		[Fact]
		public async Task CreateProjectAndTask_success()
		{
			// Arrange
			var projectService = new Mock<IProjectService>();
			var taskService = new Mock<ITaskService>();
			var projectController = new ProjectController(projectService.Object);
			var taskController = new TaskController(taskService.Object);

			var newProject = new ProjectDto { Name = "New Project", Description = "Project Description", CreatedByUserId = 1 };
			var createdProject = new ProjectDto { ProjectId = 1, Name = "New Project", Description = "Project Description", CreatedByUserId = 1 };
			projectService.Setup(service => service.CreateProjectAsync(It.IsAny<ProjectDto>())).ReturnsAsync(createdProject);

			var newTask = new TaskDto { Title = "New Task", ProjectId = 1 };
			var createdTask = new TaskDto { TaskId = 1, Title = "New Task", ProjectId = 1 };
			taskService.Setup(service => service.CreateTaskAsync(It.IsAny<TaskDto>())).ReturnsAsync(createdTask);

			// Act
			var projectResult = await projectController.CreateProject(newProject) as CreatedAtActionResult;
			var taskResult = await taskController.CreateTask(newTask) as CreatedAtActionResult;

			// Assert
			Assert.NotNull(projectResult);
			Assert.Equal("New Project", ((ProjectDto)projectResult.Value).Name);
			projectService.Verify(service => service.CreateProjectAsync(It.Is<ProjectDto>(p => p.Name == "New Project")), Times.Once);

			Assert.NotNull(taskResult);
			Assert.Equal("New Task", ((TaskDto)taskResult.Value).Title);
			taskService.Verify(service => service.CreateTaskAsync(It.Is<TaskDto>(t => t.Title == "New Task" && t.ProjectId == 1)), Times.Once);
		}
	}
}
