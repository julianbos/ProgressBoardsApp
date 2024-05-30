using Microsoft.EntityFrameworkCore;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using ProgressBoardsData;
using ProgressBoardsData.Models;

namespace ProgressBoardsServices.Implementations
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;

		public UserService(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task <IEnumerable<UserDto>> GetUsersAsync()
		{
			return await _context.Users
				.Select(u => new UserDto
				{
					UserId = u.UserId,
					UserName = u.UserName,
					Email = u.Email
				})
				.ToListAsync();
		}

		public async Task<UserDto> GetUserAsync(int userId)
		{
			var user = await _context.Users
				.Where(u => u.UserId == userId)
				.Select(u => new UserDto
				{
					UserId = u.UserId,
					UserName = u.UserName,
					Email = u.Email
				})
				.FirstOrDefaultAsync();

			if (user == null)
			{
				throw new KeyNotFoundException($"User with ID {userId} not found.");
			}
			return user;
		}

		public async Task<UserDto> CreateUserAsync(UserDto userDto)
		{
			var user = new User
			{
				UserName = userDto.UserName,
				Email = userDto.Email
			};

			await _context.Users.AddAsync(user);
			_context.SaveChanges();

			return new UserDto
			{
				UserId = user.UserId,
				UserName = user.UserName,
				Email = user.Email
			};
		}

		public async Task<UserDto> EditUserAsync(int userId, UserDto userDto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

			if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found."); ;

			user.UserName = userDto.UserName;
			user.Email = userDto.Email;

			_context.SaveChanges();

			return new UserDto
			{
				UserId = user.UserId,
				UserName = user.UserName,
				Email = user.Email
			};
		}

		public async Task<UserDto> GetUserByEmailAsync(string email)
		{
			var user = await _context.Users
				.Where(u => u.Email == email)
				.Select(u => new UserDto
				{
					UserId = u.UserId,
					UserName = u.UserName,
					Email = u.Email
				})
				.FirstOrDefaultAsync();

			return user;
		}

		public async Task<IEnumerable<UserDto>> GetUsersByProjectAsync(int projectId)
		{
			var projectUsers = await _context.ProjectUsers
				.Where(pu => pu.ProjectId == projectId)
				.ToListAsync();

			if (projectUsers == null || !projectUsers.Any())
			{
				return new List<UserDto>();
			}

			var userIds = projectUsers.Select(pu => pu.UserId).ToList();

			var users = await _context.Users
				.Where(u => userIds.Contains(u.UserId))
				.Select(u => new UserDto
				{
					UserId = u.UserId,
					UserName = u.UserName,
					Email = u.Email
				})
				.ToListAsync();

			return users;
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
			if (user != null)
			{
				_context.Users.Remove(user);
				_context.SaveChanges();
				return true;
			}
			return false;
		}
	}
}
