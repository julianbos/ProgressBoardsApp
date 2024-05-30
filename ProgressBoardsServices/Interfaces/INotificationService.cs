namespace ProgressBoardsServices.Interfaces
{
	public interface INotificationService
	{
		Task NotifyTaskAssignmentAsync(int userId, string taskTitle);
		Task NotifyAccountCreatedAsync(int userId);
	}
}
