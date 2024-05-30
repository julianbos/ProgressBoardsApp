using ProgressBoardsShared.Dtos;

public class CurrentUserService
{
	public UserDto CurrentUser { get; private set; }

	public void SetCurrentUser(UserDto user)
	{
		CurrentUser = user;
	}

	public void ClearCurrentUser()
	{
		CurrentUser = null;
	}

	public bool IsUserLoggedIn => CurrentUser != null;
}
