using ProgressBoards.ViewModels;

namespace ProgressBoards.Views;

public partial class LoginView : ContentPage
{

	public LoginView(LoginViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is LoginViewModel viewModel)
		{
			viewModel.OnAppearing();
		}
	}
}