using Gridify;
using Microsoft.AspNetCore.Mvc;
using PersonManager.ServiceContracts.Modules.Users;
using PersonManager.ServiceContracts.Modules.Users.Dtos;

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

		/// <summary>
		/// filter 에 맞는 사용자 일괄 수정 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpPatch]
		public async Task<ActionResult<object>> UpdateUsers(
			[FromQuery] GridifyQuery query,
			[FromBody] UserUpdateRequest userUpdateRequest
		)
		{
			// 빈 filter 는 전체 행이 대상이 되므로 거부
			if (string.IsNullOrWhiteSpace(query.Filter))
				return BadRequest("Filter is required");

			return new { count = await _usersService.UpdateUsers(query.Filter, userUpdateRequest) };
		}

		/// <summary>
		/// filter 에 맞는 사용자 일괄 삭제 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpDelete]
		public async Task<ActionResult<object>> DeleteUsers([FromQuery] GridifyQuery query)
		{
			// 빈 filter 는 전체 행이 대상이 되므로 거부
			if (string.IsNullOrWhiteSpace(query.Filter))
				return BadRequest("Filter is required");

			return new { count = await _usersService.DeleteUsers(query.Filter) };
		}
	}
}
