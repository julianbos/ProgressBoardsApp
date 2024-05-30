using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class TaskEditView : ContentPage
{
	public TaskEditView(TaskEditViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		var viewModel = BindingContext as TaskEditViewModel;
		viewModel?.OnAppearing();
	}
}