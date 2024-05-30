using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class AddProjectView : ContentPage
{
	public AddProjectView(AddProjectViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private void OnEmailEntryCompleted(object sender, EventArgs e)
	{
		if (BindingContext is AddProjectViewModel viewModel)
		{
			viewModel.AddEmailsCommand.Execute(null);
		}
	}
}