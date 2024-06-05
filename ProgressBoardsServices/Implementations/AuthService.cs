using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProgressBoardsData;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using ProgressBoardsData.Models;

namespace ProgressBoardsServices.Implementations
{
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext _context;

		public AuthService(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<UserDto> LoginAsync(string email, string password)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			{
				return null;
			}

			return new UserDto
			{
				UserId = user.UserId,
				UserName = user.UserName,
				Email = user.Email
			};
		}

		public async Task<bool> RegisterAsync(UserDto newUser)
		{
			if (await UserExists(newUser.Email))
				return false;

			CreatePasswordHash(newUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

			var user = new User
			{
				Email = newUser.Email,
				UserName = newUser.UserName,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return true;
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
		{
			using (var hmac = new HMACSHA512(storedSalt))
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(storedHash);
			}
		}

		private async Task<bool> UserExists(string email)
		{
			return await _context.Users.AnyAsync(u => u.Email == email);
		}
	}
}