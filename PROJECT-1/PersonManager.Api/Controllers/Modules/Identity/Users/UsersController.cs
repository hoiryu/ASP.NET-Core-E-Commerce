using Gridify;
using Microsoft.AspNetCore.Mvc;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;
using Serilog;

namespace PersonManager.Api.Controllers.Modules.Identity.Users
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUsersService _usersService, IDiagnosticContext _diagnosticContext) : ControllerBase
	{
		/// <summary>
		/// 사용자 일괄 생성 (한 건도 배열로 전달)
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<List<UserResponse>>> CreateUsers(
			[FromBody] List<UserCreateRequest> userCreateRequests
		)
		{
			var users = await _usersService.CreateUsers(userCreateRequests);
			_diagnosticContext.Set("AffectedCount", users.Count); // 요청 완료 로그에 생성된 행 수 추가

			return users;
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
			var count = await _usersService.UpdateUsers(query.Filter, userUpdateRequest);
			_diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

			return new { count };
		}

		/// <summary>
		/// filter 에 맞는 사용자 일괄 삭제 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpDelete]
		public async Task<ActionResult<object>> DeleteUsers([FromQuery] GridifyQuery query)
		{
			var count = await _usersService.DeleteUsers(query.Filter);
			_diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

			return new { count };
		}
	}
}
