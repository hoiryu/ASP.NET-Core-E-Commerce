using Gridify;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.ServiceContracts.Modules.Identity.Users;

public interface IUsersService
{
	Task<List<UserResponse>> CreateUsers(List<UserCreateRequest> userCreateRequests);

	Task<Paging<UserResponse>> GetUsers(GridifyQuery query);

	Task<int> UpdateUsers(string? filter, UserUpdateRequest userUpdateRequest);

	Task<int> DeleteUsers(string? filter);
}
