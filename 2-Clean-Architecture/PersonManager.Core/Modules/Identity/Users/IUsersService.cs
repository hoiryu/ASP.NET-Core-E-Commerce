using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Users.Dtos;

namespace PersonManager.Core.Modules.Identity.Users;

/// <summary>
/// User 유스케이스 (생성·조회·일괄 수정·일괄 삭제)
/// </summary>
public interface IUsersService
{
	Task<List<UserResponse>> CreateUsers(List<UserCreateRequest> userCreateRequests);

	Task<PagedResult<UserResponse>> GetUsers(ListQuery query);

	Task<int> UpdateUsers(string? filter, UserUpdateRequest userUpdateRequest);

	Task<int> DeleteUsers(string? filter);
}
