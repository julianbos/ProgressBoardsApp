using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class ProjectListView : ContentPage
{
	public ProjectListView(ProjectListViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;

	}
	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is ProjectListViewModel viewModel)
		{
			viewModel.OnAppearing();
		}
	}
}