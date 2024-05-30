using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class ProjectEditView : ContentPage
{
	public ProjectEditView(ProjectEditViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}