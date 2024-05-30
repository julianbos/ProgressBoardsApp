using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgressBoardsData;
using ProgressBoardsServices.Implementations;
using ProgressBoardsServices.Interfaces;
using ProgressBoards.ViewModels;
using ProgressBoards.Views;

namespace ProgressBoards
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			try
			{
				var builder = MauiApp.CreateBuilder();

				try
				{
					builder.Services.AddDbContext<ApplicationDbContext>(options =>
						options.UseSqlServer("Server=localhost;Database=ProgressBoards;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"));

					builder.UseMauiApp<App>()
						   .ConfigureFonts(fonts =>
						   {
							   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
							   fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
						   });

					// Register ViewModels
					builder.Services.AddSingleton<LoginViewModel>();
					builder.Services.AddSingleton<RegisterViewModel>();
					builder.Services.AddSingleton<TaskListViewModel>();
					builder.Services.AddSingleton<ProjectListViewModel>();
					builder.Services.AddSingleton<ProjectEditViewModel>();
					builder.Services.AddSingleton<AddProjectViewModel>();
					builder.Services.AddSingleton<TaskEditViewModel>();
					builder.Services.AddSingleton<TaskAddViewModel>();

					// Register Services
					builder.Services.AddSingleton<IEmailService>(sp => new EmailService(
						smtpServer: "sandbox.smtp.mailtrap.io",
						smtpPort: 587,
						fromEmail: "noreply@progressboards.nl",
						smtpUser: "988ad0699b76e6",
						smtpPass: "fff62bdae66436"
					));

					builder.Services.AddSingleton<INotificationService, NotificationService>();

					builder.Services.AddSingleton<IAuthService, AuthService>();
					builder.Services.AddSingleton<IProjectService, ProjectService>();
					builder.Services.AddSingleton<ITaskService, TaskService>();
					builder.Services.AddSingleton<IUserService, UserService>();

					builder.Services.AddSingleton<CurrentUserService>();

					// Register Views
					builder.Services.AddTransient<LoginView>();
					builder.Services.AddTransient<RegisterView>();
					builder.Services.AddTransient<ProjectListView>();
					builder.Services.AddTransient<ProjectEditView>();
					builder.Services.AddTransient<TaskListView>();
					builder.Services.AddTransient<AddProjectView>();
					builder.Services.AddTransient<TaskEditView>();
					builder.Services.AddTransient<TaskAddView>();

#if DEBUG
					builder.Logging.AddDebug();
#endif

					return builder.Build();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Exception during CreateMauiApp: {ex.Message}");
					throw;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unhandled exception: {ex.Message}");
				throw;
			}
		}
	}
}