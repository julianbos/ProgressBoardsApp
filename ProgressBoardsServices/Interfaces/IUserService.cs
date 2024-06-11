using ProgressBoardsShared.Dtos;

namespace ProgressBoardsServices.Interfaces
{
	public interface IUserService
	{
		Task <IEnumerable<UserDto>> GetUsersAsync();
		Task<UserDto> GetUserAsync(int userId);
		Task<UserDto> GetUserByEmailAsync(string email);
		Task<IEnumerable<UserDto>> GetUsersByProjectAsync(int projectId);
		Task<bool> DeleteUserAsync(int userId);
	}
}
