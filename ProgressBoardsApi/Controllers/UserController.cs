using Microsoft.AspNetCore.Mvc;
using ProgressBoardsShared.Dtos;
using ProgressBoardsServices.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ProgressBoardsApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public async Task<IActionResult> GetUsersAsync()
		{
			var users = await _userService.GetUsersAsync();
			return Ok(users);
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUserAsync(int userId)
		{
			try
			{
				var user = await _userService.GetUserAsync(userId);
				return Ok(user);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
		{
			try
			{
				if (userDto == null)
					return BadRequest("Invalid user data");

				var createdUser = await _userService.CreateUserAsync(userDto);
				return CreatedAtAction(nameof(GetUserAsync), new { userId = createdUser.UserId }, createdUser);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPut("{userId}")]
		public async Task<IActionResult> EditUserAsync(int userId, [FromBody] UserDto userDto)
		{
			try
			{
				if (userDto == null)
					return BadRequest("Invalid user data");

				var editedUser = await _userService.EditUserAsync(userId, userDto);
				if (editedUser == null)
					return NotFound($"User with ID {userId} not found.");

				return Ok(editedUser);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUserAsync(int userId)
		{
			try
			{
				await _userService.DeleteUserAsync(userId);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
			}
		}
	}
}
