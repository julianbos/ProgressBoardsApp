using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace ProgressBoards.ViewModels
{
	public partial class AddProjectViewModel : BaseViewModel
	{
		[ObservableProperty]
		private string name;

		[ObservableProperty]
		private string description;

		[ObservableProperty]
		private string emailInput;

		private readonly IProjectService _projectService;
		private readonly IUserService _userService;
		private readonly CurrentUserService _currentUserService;

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

		public AddProjectViewModel(IProjectService projectService, IUserService userService, CurrentUserService currentUserService)
		{
			Title = "Create Project";
			_projectService = projectService;
			_userService = userService;
			_currentUserService = currentUserService;

			var currentUser = _currentUserService.CurrentUser;
			if (currentUser != null)
			{
				ProjectUsers.Add(new ProjectUserDto { UserId = currentUser.UserId });
				Emails.Add(currentUser.Email);
			}
		}

		[RelayCommand]
		private async Task SaveProject()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				var projectDto = new ProjectDto
				{
					Name = Name,
					Description = Description,
					CreatedByUserId = _currentUserService.CurrentUser.UserId, 
					ProjectUsers = ProjectUsers.ToList()
				};

				await _projectService.CreateProjectAsync(projectDto);
				await Shell.Current.DisplayAlert("Project Created!", "Your project has been successfully created.", "OK");

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
			var currentUserEmail = _currentUserService.CurrentUser.Email;
			if (currentUserEmail == email)
			{
				await Shell.Current.DisplayAlert("Error!", "You cannot remove the project creator.", "OK");
				return;
			}

			Emails.Remove(email);
			try
			{
				var user = await _userService.GetUserByEmailAsync(email);
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
