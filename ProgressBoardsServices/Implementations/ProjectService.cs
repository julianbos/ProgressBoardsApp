using ProgressBoardsData;
using ProgressBoardsData.Models;
using ProgressBoardsServices.Interfaces;
using ProgressBoardsShared.Dtos;
using Microsoft.EntityFrameworkCore;
using ProgressBoardsData;

namespace ProgressBoardsServices.Implementations
{
	public class ProjectService : IProjectService
	{
		private readonly ApplicationDbContext _context;

		public ProjectService(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(int userId)
		{
			var projects = await _context.Projects
				.Include(p => p.ProjectUsers)
					.ThenInclude(pu => pu.User)
				.Where(p => p.ProjectUsers.Any(pu => pu.User.UserId == userId))
				.Select(p => new ProjectDto
				{
					ProjectId = p.ProjectId,
					Name = p.Name,
					Description = p.Description,
					CreatedByUserId = p.CreatedByUserId,
					ProjectUsers = p.ProjectUsers.Select(pu => new ProjectUserDto
					{
						UserId = pu.UserId,
					}).ToList(),
				})
				.ToListAsync();

			return projects;
		}

		public async Task<ProjectDto> GetProjectAsync(int projectId)
		{
			var project = await _context.Projects
				.Include(p => p.ProjectUsers)
					.ThenInclude(pu => pu.User)
				.Where(p => p.ProjectId == projectId)
				.Select(p => new ProjectDto
				{
					ProjectId = p.ProjectId,
					Name = p.Name,
					Description = p.Description,
					ProjectUsers = p.ProjectUsers.Select(pu => new ProjectUserDto
					{
						UserId = pu.UserId
					}).ToList(),
				})
				.FirstOrDefaultAsync();

			return project;
		}

		public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
		{
			if (projectDto == null)
				throw new ArgumentNullException(nameof(projectDto));

			var newProject = new Project
			{
				Name = projectDto.Name,
				Description = projectDto.Description,
				CreatedByUserId = projectDto.CreatedByUserId,
				ProjectUsers = projectDto.ProjectUsers.Select(pu => new ProjectUser
				{
					UserId = pu.UserId,
				}).ToList()
			};

			_context.Projects.Add(newProject);
			await _context.SaveChangesAsync();

			projectDto.ProjectId = newProject.ProjectId;

			return projectDto;
		}

		public async Task<ProjectDto> EditProjectAsync(int projectId, ProjectDto projectDto)
		{
			if (projectDto == null)
				throw new ArgumentNullException(nameof(projectDto));

			var project = await _context.Projects
				.Include(p => p.ProjectUsers)
				.FirstOrDefaultAsync(p => p.ProjectId == projectId);

			if (project != null)
			{
				project.Name = projectDto.Name;
				project.Description = projectDto.Description;

				var existingUserIds = project.ProjectUsers.Select(pu => pu.UserId).ToList();

				foreach (var projectUserDto in projectDto.ProjectUsers)
				{
					if (!existingUserIds.Contains(projectUserDto.UserId))
					{
						var projectUser = new ProjectUser
						{
							UserId = projectUserDto.UserId,
							ProjectId = projectId
						};
						project.ProjectUsers.Add(projectUser);
					}
				}

				var projectUsersToRemove = project.ProjectUsers
					.Where(pu => !projectDto.ProjectUsers.Any(pudto => pudto.UserId == pu.UserId))
					.ToList();

				foreach (var projectUser in projectUsersToRemove)
				{
					project.ProjectUsers.Remove(projectUser);
				}

				await _context.SaveChangesAsync();
			}

			return projectDto;
		}

		public async Task<bool> DeleteProjectAsync(int projectId)
		{
			var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

			if (project != null)
			{
				_context.Projects.Remove(project);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}
	}
}
