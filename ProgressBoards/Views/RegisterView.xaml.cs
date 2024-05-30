using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class RegisterView : ContentPage
{

	public RegisterView(RegisterViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is RegisterViewModel viewModel)
		{
			viewModel.OnAppearing();
		}
	}
}