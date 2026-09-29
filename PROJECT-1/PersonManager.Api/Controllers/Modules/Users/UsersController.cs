using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using ServiceContracts.Modules.Users.Enums;

namespace PersonManager.Api.Controllers.Modules.Users
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUsersService usersService) : ControllerBase
	{
		[HttpPost]
		public ActionResult<UserResponse> CreateUser([FromBody] UserCreateRequest userCreateRequest)
		{
			return usersService.CreateUser(userCreateRequest);
		}

		[HttpGet]
		public ActionResult<List<UserResponse>> GetUsers([FromQuery] SearchQuery<UserSearchOptions> query)
		{
			return usersService.GetUsers(query);
		}

		[HttpPatch("{userId:guid}")]
		public ActionResult<UserResponse> UpdateUser(
			[FromRoute] Guid userId,
			[FromBody] UserUpdateRequest userUpdateRequest
		)
		{
			userUpdateRequest.Id = userId;

			UserResponse? userResponse = usersService.UpdateUser(userUpdateRequest);
			if (userResponse is null)
				return NotFound();

			return userResponse;
		}

		[HttpDelete("{userId:guid}")]
		public ActionResult<UserResponse> DeleteUser([FromRoute] Guid userId)
		{
			UserResponse? userResponse = usersService.DeleteUser(userId);
			if (userResponse is null)
				return NotFound();

			return userResponse;
		}
	}
}
