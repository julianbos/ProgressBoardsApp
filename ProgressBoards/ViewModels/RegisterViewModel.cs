using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressBoards.Views;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;

namespace ProgressBoards.ViewModels
{
	public partial class RegisterViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;

		[ObservableProperty]
		private string email;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string password;

		[ObservableProperty]
		private string confirmPassword;

		[ObservableProperty]
		private double passwordStrength;

		[ObservableProperty]
		private Color passwordStrengthColor;

		public RegisterViewModel(IAuthService authService)
		{
			_authService = authService;
			Title = "Register";
		}

		public void OnAppearing()
		{
			Email = string.Empty;
			UserName = string.Empty;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
			PasswordStrengthColor = Colors.Gray;
			PasswordStrength = 0;
		}

		[RelayCommand]
		private async Task Register()
		{
			IsBusy = true;
			try
			{
				if (Password != ConfirmPassword)
				{
					await Shell.Current.DisplayAlert("Error", "Passwords do not match", "OK");
					return;
				}

				var newUser = new UserDto
				{
					UserName = UserName,
					Email = Email,
					Password = Password
				};

				var success = await _authService.RegisterAsync(newUser);
				if (success)
				{
					await Shell.Current.GoToAsync(nameof(LoginView));
				}
				else
				{
					await Shell.Current.DisplayAlert("Error", "User already exists", "OK");
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Error", "An error occurred", "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		async Task GoToLogin()
		{
			await Shell.Current.GoToAsync(nameof(LoginView));
		}

		partial void OnPasswordChanged(string value)
		{
			var strength = CalculatePasswordStrength(value);
			PasswordStrength = strength / 5.0; 
			PasswordStrengthColor = GetPasswordStrengthColor(strength);
		}

		private Color GetPasswordStrengthColor(int strength)
		{
			return strength switch
			{
				>= 4 => Color.FromRgb(0, 128, 0),
				3 => Color.FromRgb(255, 165, 0),
				_ => Color.FromRgb(255, 0, 0),
			};
		}

		private int CalculatePasswordStrength(string password)
		{
			int safe = 0;
			if (password.Length >= 8) safe++;
			if (password.Any(char.IsUpper)) safe++;
			if (password.Any(char.IsLower)) safe++;
			if (password.Any(char.IsDigit)) safe++;
			if (password.Any(ch => !char.IsLetterOrDigit(ch))) safe++;

			return safe;
		}
	}
}
