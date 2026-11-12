using Gridify;
using PersonManager.Api.Endpoints.Common.Dtos;
using PersonManager.Api.Endpoints.Common.EndpointFilters;
using PersonManager.Api.Endpoints.Common.Parameters;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.Api.Endpoints.Modules.Identity.Users;

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
	private static async Task<MutationResponse> DeleteUsers(
		string? filter,
		IUsersService usersService
	)
	{
		var count = await usersService.DeleteUsers(filter);

		return new MutationResponse(count);
	}
}
