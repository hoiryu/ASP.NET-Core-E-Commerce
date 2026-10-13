using Gridify;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;

namespace PersonManager.Api.Controllers.Modules.Users
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUsersService _usersService) : ControllerBase
	{
		[HttpPost]
		public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserCreateRequest userCreateRequest)
		{
			return await _usersService.CreateUser(userCreateRequest);
		}

		[HttpGet]
		public async Task<ActionResult<Paging<UserResponse>>> GetUsers([FromQuery] GridifyQuery query)
		{
			return await _usersService.GetUsers(query);
		}

		[HttpPatch("{userId:guid}")]
		public async Task<ActionResult<UserResponse>> UpdateUser(
			[FromRoute] Guid userId,
			[FromBody] UserUpdateRequest userUpdateRequest
		)
		{
			userUpdateRequest.Id = userId;

			UserResponse? userResponse = await _usersService.UpdateUser(userUpdateRequest);

			if (userResponse is null)
				return NotFound();

			return userResponse;
		}

		[HttpDelete("{userId:guid}")]
		public async Task<ActionResult<UserResponse>> DeleteUser([FromRoute] Guid userId)
		{
			UserResponse? userResponse = await _usersService.DeleteUser(userId);

			if (userResponse is null)
				return NotFound();

			return userResponse;
		}
	}
}
