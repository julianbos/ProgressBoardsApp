using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProgressBoardsApi.Controllers;
using ProgressBoardsServices.Interfaces;
using ProgressBoardsShared.Dtos;
using Xunit;

namespace ProgressBoardsApi.Tests
{
	public class ProjectControllerTests
	{
		[Fact]
		public async Task GetProjects_ReturnsOkResult()
		{
			// Arrange
			var projectService = new Mock<IProjectService>();
			projectService.Setup(repo => repo.GetProjectsAsync(It.IsAny<int>()))
				.ReturnsAsync(GetTestProjects());
			var controller = new ProjectController(projectService.Object);
			var userId = 1;

			// Act
			var result = await controller.GetProjects(userId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
			Assert.Single(projects);
		}

		private IEnumerable<ProjectDto> GetTestProjects()
		{
			var projects = new List<ProjectDto>
			{
				new ProjectDto { ProjectId = 1, Name = "ProgressBoardsTest", Description = "ProgresBoardsTests making" }
			};
			return projects;
		}
	}
}
