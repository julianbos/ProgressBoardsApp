using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace ProgressBoards.ViewModels
{
	[QueryProperty(nameof(SelectedProject), "Project")]
	public partial class ProjectEditViewModel : BaseViewModel
	{
		private readonly IProjectService _projectService;
		private readonly IUserService _userService;

		[ObservableProperty]
		private ProjectDto selectedProject;

		[ObservableProperty]
		private string emailInput;

		public ObservableCollection<string> Emails { get; } = new ObservableCollection<string>();

		private ObservableCollection<ProjectUserDto> _projectUsers = new ObservableCollection<ProjectUserDto>();
		public ObservableCollection<ProjectUserDto> ProjectUsers
		{
			get => _projectUsers;
			set
			{
				_projectUsers = value;
				OnPropertyChanged(nameof(ProjectUsers));
			}
		}
		public ProjectEditViewModel(IProjectService projectService, IUserService userService)
		{
			Title = "Edit Project";
			_projectService = projectService;
			_userService = userService;
		}

		partial void OnSelectedProjectChanged(ProjectDto value)
		{
			if (value != null)
			{
				LoadEmailsCommand.Execute(value);
			}
		}

		[RelayCommand]
		private async Task LoadEmails(ProjectDto project)
		{
			ProjectUsers.Clear();
			Emails.Clear();

			foreach (ProjectUserDto user in project.ProjectUsers)
			{
				ProjectUsers.Add(user);

				var userDto = await _userService.GetUserAsync(user.UserId);
				if (userDto != null)
				{
					Emails.Add(userDto.Email);
				}
			}
		}

		[RelayCommand]
		async Task SaveProject()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				SelectedProject.ProjectUsers = ProjectUsers.ToList();

				await _projectService.EditProjectAsync(SelectedProject.ProjectId, SelectedProject);
				await Shell.Current.DisplayAlert("Project Edited!", "Your project has been successfully updated.", "OK");

				await Shell.Current.GoToAsync("..");
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
		async Task DeleteProject()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				await _projectService.DeleteProjectAsync(SelectedProject.ProjectId);
				await Shell.Current.DisplayAlert("Project Deleted!", "Your project has been successfully deleted.", "OK");

				await Shell.Current.GoToAsync("..");
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
		async Task AddEmails()
		{
			if (!string.IsNullOrEmpty(EmailInput))
			{
				var emails = EmailInput.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var email in emails)
				{
					if (IsValidEmail(email) && !Emails.Contains(email))
					{
						try
						{
							var user = await _userService.GetUserByEmailAsync(email);
							if (user != null)
							{
								Emails.Add(email);
								ProjectUsers.Add(new ProjectUserDto { UserId = user.UserId });
							}
							else
							{
								await Shell.Current.DisplayAlert("Error!", $"User with email {email} not found", "OK");
							}
						}
						catch (Exception ex)
						{
							await Shell.Current.DisplayAlert("Error!", $"An error occurred while retrieving user with email {email}: {ex.Message}", "OK");
						}
					}
				}
				EmailInput = string.Empty;
			}
		}

		[RelayCommand]
		async Task RemoveEmail(string email)
		{
			var user = await _userService.GetUserByEmailAsync(email);
			if (user != null && user.UserId == SelectedProject.CreatedByUserId)
			{
				await Shell.Current.DisplayAlert("Error!", "You cannot remove the project creator.", "OK");
				return;
			}

			Emails.Remove(email);
			try
			{
				if (user != null)
				{
					var projectUser = ProjectUsers.FirstOrDefault(u => u.UserId == user.UserId);
					if (projectUser != null)
					{
						ProjectUsers.Remove(projectUser);
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Error!", $"An error occurred while removing user with email {email}: {ex.Message}", "OK");
			}
		}

		private bool IsValidEmail(string email)
		{
			return Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
		}
	}
}
