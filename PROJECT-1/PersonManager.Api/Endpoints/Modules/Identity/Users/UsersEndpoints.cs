using Gridify;
using PersonManager.Api.Endpoints.Common.Parameters;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;
using Serilog;

namespace PersonManager.Api.Endpoints.Modules.Identity.Users;

public static class UsersEndpoints
{
	public static RouteGroupBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/users");

		group.MapPost("/", CreateUsers);
		group.MapGet("/", GetUsers);
		group.MapPatch("/", UpdateUsers);
		group.MapDelete("/", DeleteUsers);

		return group;
	}

	/// <summary>
	/// 사용자 일괄 생성 (한 건도 배열로 전달)
	/// </summary>
	private static async Task<List<UserResponse>> CreateUsers(
		List<UserCreateRequest> userCreateRequests,
		IUsersService usersService,
		IDiagnosticContext diagnosticContext
	)
	{
		var users = await usersService.CreateUsers(userCreateRequests);
		diagnosticContext.Set("AffectedCount", users.Count); // 요청 완료 로그에 생성된 행 수 추가

		return users;
	}

	private static async Task<Paging<UserResponse>> GetUsers(
		[AsParameters] GridifyQueryParameters query,
		IUsersService usersService
	)
	{
		return await usersService.GetUsers(query.ToGridifyQuery());
	}

	/// <summary>
	/// filter 에 맞는 사용자 일괄 수정 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<object> UpdateUsers(
		string? filter,
		UserUpdateRequest userUpdateRequest,
		IUsersService usersService,
		IDiagnosticContext diagnosticContext
	)
	{
		var count = await usersService.UpdateUsers(filter, userUpdateRequest);
		diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

		return new { count };
	}

	/// <summary>
	/// filter 에 맞는 사용자 일괄 삭제 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<object> DeleteUsers(
		string? filter,
		IUsersService usersService,
		IDiagnosticContext diagnosticContext
	)
	{
		var count = await usersService.DeleteUsers(filter);
		diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

		return new { count };
	}
}
