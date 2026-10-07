using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Common.Dtos;
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
		public async Task<ActionResult<List<UserResponse>>> GetUsers(
			[FromQuery(Name = "filter")] UserFilter filter,
			[FromQuery(Name = "order")] UserOrder order,
			[FromQuery(Name = "paging")] Paging paging
		)
		{
			return await _usersService.GetUsers(filter, order, paging);
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
