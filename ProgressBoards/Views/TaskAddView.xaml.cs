using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class TaskAddView : ContentPage
{
	public TaskAddView(TaskAddViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}