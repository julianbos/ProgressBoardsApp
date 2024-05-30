using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;

namespace ProgressBoards.ViewModels
{
	[QueryProperty(nameof(ProjectId), "ProjectId")]
	public partial class TaskAddViewModel : BaseViewModel
	{
		private readonly ITaskService _taskService;
		private readonly IUserService _userService;
		private readonly CurrentUserService _currentUserService;
		public ObservableCollection<UserDto> ProjectUsers { get; } = new ObservableCollection<UserDto>();

		[ObservableProperty]
		private int projectId;

		[ObservableProperty]
		private string title;

		[ObservableProperty]
		private string description;

		[ObservableProperty]
		private StatusDto status;

		[ObservableProperty]
		private DateTime dueDate = DateTime.Today;

		[ObservableProperty]
		private UserDto selectedAssignee;

		public TaskAddViewModel(ITaskService taskService, IUserService userService, CurrentUserService currentUserService)
		{
			Title = "Add Task";
			_taskService = taskService;
			_userService = userService;
			_currentUserService = currentUserService;
		}

		partial void OnProjectIdChanged(int value)
		{
			Task.Run(async () => await GetProjectUsers());
		}

		private async Task GetProjectUsers()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				IEnumerable<UserDto> users = await _userService.GetUsersByProjectAsync(ProjectId);
				var usersList = users.ToList();

				MainThread.BeginInvokeOnMainThread(() =>
				{
					ProjectUsers.Clear();
					foreach (UserDto user in usersList)
					{
						ProjectUsers.Add(user);
					}
				});
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
		private async Task SaveTask()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				TaskDto newTask = new TaskDto
				{
					Title = Title,
					Description = Description,
					Status = Status,
					DueDate = DueDate,
					ProjectId = ProjectId,
					AssignedUserId = SelectedAssignee?.UserId,
					ReporterId = _currentUserService.CurrentUser.UserId
				};

				await _taskService.CreateTaskAsync(newTask);
				await Shell.Current.DisplayAlert("Task Created!", "Task successfully created", "OK");

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
	}
}
