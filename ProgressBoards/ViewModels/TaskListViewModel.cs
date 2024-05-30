using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoards.Views;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using System.Collections.ObjectModel;
using Task = System.Threading.Tasks.Task;

namespace ProgressBoards.ViewModels
{
	[QueryProperty(nameof(ProjectId), "ProjectId")]
	public partial class TaskListViewModel : BaseViewModel
	{
		public ObservableCollection<TaskDto> Tasks { get; } = new();
		public ObservableCollection<TaskDto> ToDoTasks { get; } = new ObservableCollection<TaskDto>();
		public ObservableCollection<TaskDto> DoingTasks { get; } = new ObservableCollection<TaskDto>();
		public ObservableCollection<TaskDto> DoneTasks { get; } = new ObservableCollection<TaskDto>();

		private readonly ITaskService _taskService;
		private readonly IUserService _userService;

		private TaskDto _draggedItem;

		[ObservableProperty]
		private int projectId;

		[ObservableProperty]
		TaskDto selectedTask;

		[ObservableProperty]
		private StatusDto selectedOption;

		public TaskListViewModel(ITaskService taskService, IUserService userService)
		{
			Title = "All tasks";
			_taskService = taskService;
			_userService = userService;
		}

		public void OnAppearing()
		{
			GetTasksAsync();
		}

		[RelayCommand]
		async Task GoToEditTask(TaskDto task)
		{
			if (task == null)
				return;

			var data = new Dictionary<string, object>
			{
				{ "Task", task },
				{ "ProjectId", ProjectId }
			};

			await Shell.Current.GoToAsync(nameof(TaskEditView), true, data);
		}

		[RelayCommand]
		async Task GoToAddTask()
		{
			var data = new Dictionary<string, object>
			{
				{ "ProjectId", ProjectId }
			};

			await Shell.Current.GoToAsync(nameof(TaskAddView), true, data);
		}

		[RelayCommand]
		public void DragStarted(TaskDto task)
		{
			_draggedItem = task;
		}

		[RelayCommand]
		async Task TaskDropped(string statusStr)
		{
			if (_draggedItem == null || IsBusy)
				return;

			try
			{
				IsBusy = true;
				var status = StatusDto.TODO;

				switch (statusStr)
				{
					case "0":
						status = StatusDto.TODO;
						break;
					case "1":
						status = StatusDto.DOING;
						break;
					case "2":
						status = StatusDto.DONE;
						break;
				}

				_draggedItem.Status = status;
				var updatedTask = await _taskService.EditTaskAsync(_draggedItem.TaskId, _draggedItem);
				if (updatedTask != null)
				{
					await GetTasksAsync();
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		async Task GetTasksAsync()
		{
			try
			{
				IEnumerable<TaskDto> tasks = await _taskService.GetTasksAsync(ProjectId);
				var tasklist = tasks.ToList();

				Tasks.Clear();
				ToDoTasks.Clear();
				DoingTasks.Clear();
				DoneTasks.Clear();

				foreach (TaskDto task in tasklist)
				{
					Tasks.Add(task);
					switch (task.Status)
					{
						case StatusDto.TODO:
							ToDoTasks.Add(task);
							break;
						case StatusDto.DOING:
							DoingTasks.Add(task);
							break;
						case StatusDto.DONE:
							DoneTasks.Add(task);
							break;
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Something went wrong!", ex.Message, "OK");
			}
		}
	}
}