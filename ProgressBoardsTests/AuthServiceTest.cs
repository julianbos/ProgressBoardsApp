using Microsoft.EntityFrameworkCore;
using ProgressBoardsData;
using ProgressBoardsServices.Implementations;
using ProgressBoardsShared.Dtos;
using Xunit;

namespace ProgressBoardsServices.Tests
{
	public class AuthServiceTests
	{
		[Fact]
		public async Task Register_success()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				var authService = new AuthService(context);

				// Act
				var newUser = new UserDto
				{
					Email = "test@test.com",
					UserName = "test",
					Password = "wachtwoord123"
				};

				var result = await authService.RegisterAsync(newUser);

				Assert.True(result);
			}
		}

		//FAIL 
		[Fact]
		public async Task Register_dupe()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				var existingUser = new ProgressBoardsData.Models.User
				{
					Email = "test@test.com",
					UserName = "test",
					PasswordHash = new byte[64], 
					PasswordSalt = new byte[128] 
				};

				context.Users.Add(existingUser);
				await context.SaveChangesAsync();

				var authService = new AuthService(context);

				// Act
				var existingUserDto = new UserDto
				{
					Email = "test@test.com",
					UserName = "test",
					Password = "wachtwoord123"
				};

				var result = await authService.RegisterAsync(existingUserDto);

				// Assert
				Assert.False(result);
			}
		}
	}
}
