using ProgressBoardsShared.Dtos;

namespace ProgressBoardsServices.Interfaces
{
	public interface IAuthService
	{
		Task<UserDto> LoginAsync(string email, string password);
		Task<bool> RegisterAsync(UserDto newUser);

	}
}
