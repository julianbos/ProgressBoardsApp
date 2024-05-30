using ProgressBoardsServices.Interfaces;

public class NotificationService : INotificationService
{
	private readonly IEmailService _emailService;
	private readonly IUserService _userService;

	public NotificationService(IEmailService emailService, IUserService userService)
	{
		_emailService = emailService;
		_userService = userService;
	}

	public async Task NotifyTaskAssignmentAsync(int userId, string taskTitle)
	{
		var user = await _userService.GetUserAsync(userId);
		if (user != null)
		{
			var subject = "New Task Assigned";
			var body = $"You have been assigned to a new task: {taskTitle}.";
			await _emailService.SendEmailAsync(user.Email, subject, body);
		}
	}

	public async Task NotifyAccountCreatedAsync(int userId)
	{
		var user = await _userService.GetUserAsync(userId);
		if (user != null)
		{
			var subject = "Account created";
			var body = $"Your account has succesfully been created!";
			await _emailService.SendEmailAsync(user.Email, subject, body);
		}
	}
}
