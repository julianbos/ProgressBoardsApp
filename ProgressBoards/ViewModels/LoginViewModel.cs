using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoards.Views;
using ProgressBoardsServices.Interfaces;
using System.ComponentModel;

namespace ProgressBoards.ViewModels
{
	public partial class LoginViewModel : BaseViewModel, INotifyPropertyChanged
	{
		private readonly IAuthService _authService;
		private readonly CurrentUserService _currentUserService;

		[ObservableProperty]
		private string email;

		[ObservableProperty]
		private string password;

		public LoginViewModel(IAuthService authService, CurrentUserService currentUserService)
		{
			_authService = authService;
			_currentUserService = currentUserService;
			Title = "Login";
		}

		public void OnAppearing()
		{
			Email = string.Empty;
			Password = string.Empty;
		}

		[RelayCommand]
		private async Task Login()
		{
			IsBusy = true;
			try
			{
				var user = await _authService.LoginAsync(Email, Password);
				if (user != null)
				{
					_currentUserService.SetCurrentUser(user);
					await Shell.Current.GoToAsync(nameof(ProjectListView));
				}
				else
				{
					await Shell.Current.DisplayAlert("Error", "Invalid login", "OK");
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
		async Task GoToRegister()
		{
			await Shell.Current.GoToAsync(nameof(RegisterView));
		}
	}
}