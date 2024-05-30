using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;

namespace ProgressBoards.ViewModels
{
	[QueryProperty(nameof(SelectedTask), "Task")]
	[QueryProperty(nameof(ProjectId), "ProjectId")]
	public partial class TaskEditViewModel : BaseViewModel
	{
		private readonly ITaskService _taskService;
		private readonly IUserService _userService;

		public ObservableCollection<UserDto> ProjectUsers { get; set; } = new ObservableCollection<UserDto>();

		[ObservableProperty]
		private TaskDto selectedTask;

		[ObservableProperty]
		private int projectId;

		[ObservableProperty]
		private UserDto selectedUser;

		public TaskEditViewModel(ITaskService taskService, IUserService userService)
		{
			_taskService = taskService;
			_userService = userService;
		}
		public void OnAppearing()
		{
			GetProjectUsers();
			if (SelectedTask != null)
			{
				MainThread.BeginInvokeOnMainThread(() =>
				{
					SelectedUser = ProjectUsers.FirstOrDefault(u => u.UserId == SelectedTask.AssignedUserId);
				});
			}
		}

		partial void OnSelectedUserChanged(UserDto value)
		{
			if (SelectedTask != null && value != null)
			{
				SelectedTask.AssignedUserId = value.UserId;
			}
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

					if (SelectedTask != null)
					{
						SelectedUser = ProjectUsers.FirstOrDefault(u => u.UserId == SelectedTask.AssignedUserId);
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

				await _taskService.EditTaskAsync(SelectedTask.TaskId, SelectedTask);
				await Shell.Current.DisplayAlert("Task Saved!", "Task successfully edited", "OK");

				await Shell.Current.GoToAsync("..");
				SelectedTask = null;
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
		private async Task DeleteTask()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				await _taskService.DeleteTaskAsync(SelectedTask.TaskId);
				await Shell.Current.DisplayAlert("Task has been removed", "", "OK");

				await Shell.Current.GoToAsync("..");
				SelectedTask = null;

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
