using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoards.Views;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;
using Task = System.Threading.Tasks.Task;

namespace ProgressBoards.ViewModels
{
	public partial class ProjectListViewModel : BaseViewModel
	{
		public ObservableCollection<ProjectDto> Projects { get; } = new();

		private readonly IProjectService _projectService;
		private readonly IAuthService _authService;
		private readonly CurrentUserService _currentuserService;

		[ObservableProperty]
		ProjectDto _selectedProject;

		public ProjectListViewModel(IProjectService projectService, IAuthService authService, CurrentUserService currentUserService)
		{
			Title = "Your projects";
			_projectService = projectService;
			_authService = authService;
			_currentuserService = currentUserService;
		}

		public void OnAppearing()
		{
			GetProjects();
		}

		[RelayCommand]
		async Task GetProjects()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;

				var projects = await _projectService.GetProjectsAsync(_currentuserService.CurrentUser.UserId);
				var projectList = projects.ToList();

				if (Projects.Any())
					Projects.Clear();

				foreach (var project in projectList)
					Projects.Add(project);
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		async Task GoToAddProject()
		{
			await Shell.Current.GoToAsync(nameof(AddProjectView));
		}

		[RelayCommand]
		async Task GoToEditProject(ProjectDto project)
		{
			if (project != null || !IsBusy)
			{
				var data = new Dictionary<string, object>
				{
					{ "Project", project }
				};
				await Shell.Current.GoToAsync(nameof(ProjectEditView), data);
			}
		}

		[RelayCommand]
		async Task GoToTasks()
		{
			if (IsBusy || SelectedProject == null)
				return;

			try
			{
				IsBusy = true;

				var data = new Dictionary<string, object>
				{
					{ "ProjectId", SelectedProject.ProjectId }
				};
				await Shell.Current.GoToAsync(nameof(TaskListView), data);
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
