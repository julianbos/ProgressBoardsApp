using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class TaskListView : ContentPage
{
	public TaskListView(TaskListViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is TaskListViewModel viewModel)
		{
			viewModel.OnAppearing();
		}
	}
}