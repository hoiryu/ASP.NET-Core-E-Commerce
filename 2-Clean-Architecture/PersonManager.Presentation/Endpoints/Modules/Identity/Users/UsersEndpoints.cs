using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Users;
using PersonManager.Core.Modules.Identity.Users.Dtos;
using PersonManager.Presentation.Endpoints.Common.Dtos;
using PersonManager.Presentation.Endpoints.Common.EndpointFilters;
using PersonManager.Presentation.Endpoints.Common.Parameters;

namespace PersonManager.Presentation.Endpoints.Modules.Identity.Users;

public static class UsersEndpoints
{
	public static RouteGroupBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/users");

		group.MapPost("/", CreateUsers).AddEndpointFilter<MutationEndpointFilter>();
		group.MapGet("/", GetUsers);
		group.MapPatch("/", UpdateUsers).AddEndpointFilter<MutationEndpointFilter>();
		group.MapDelete("/", DeleteUsers).AddEndpointFilter<MutationEndpointFilter>();

		return group;
	}

	/// <summary>
	/// 사용자 일괄 생성 (한 건도 배열로 전달)
	/// </summary>
	private static async Task<List<UserResponse>> CreateUsers(
		List<UserCreateRequest> userCreateRequests,
		IUsersService usersService
	)
	{
		return await usersService.CreateUsers(userCreateRequests);
	}

	private static async Task<PagedResult<UserResponse>> GetUsers(
		[AsParameters] ListQueryParameters query,
		IUsersService usersService
	)
	{
		return await usersService.GetUsers(query.ToListQuery());
	}

	/// <summary>
	/// filter 에 맞는 사용자 일괄 수정 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<MutationResponse> UpdateUsers(
		string? filter,
		UserUpdateRequest userUpdateRequest,
		IUsersService usersService
	)
	{
		var count = await usersService.UpdateUsers(filter, userUpdateRequest);

		return new MutationResponse(count);
	}

	/// <summary>
	/// filter 에 맞는 사용자 일괄 삭제 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<MutationResponse> DeleteUsers(string? filter, IUsersService usersService)
	{
		var count = await usersService.DeleteUsers(filter);

		return new MutationResponse(count);
	}
}
