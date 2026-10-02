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
		public ActionResult<UserResponse> CreateUser([FromBody] UserCreateRequest userCreateRequest)
		{
			return _usersService.CreateUser(userCreateRequest);
		}

		[HttpGet]
		public ActionResult<List<UserResponse>> GetUsers(
			[FromQuery(Name = "filter")] UserFilter filter,
			[FromQuery(Name = "sort")] UserSort sort
		)
		{
			return _usersService.GetUsers(filter, sort);
		}

		[HttpPatch("{userId:guid}")]
		public ActionResult<UserResponse> UpdateUser(
			[FromRoute] Guid userId,
			[FromBody] UserUpdateRequest userUpdateRequest
		)
		{
			userUpdateRequest.Id = userId;

			UserResponse? userResponse = _usersService.UpdateUser(userUpdateRequest);
			if (userResponse is null)
				return NotFound();

			return userResponse;
		}

		[HttpDelete("{userId:guid}")]
		public ActionResult<UserResponse> DeleteUser([FromRoute] Guid userId)
		{
			UserResponse? userResponse = _usersService.DeleteUser(userId);
			if (userResponse is null)
				return NotFound();

			return userResponse;
		}
	}
}
