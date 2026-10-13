using Gridify;
using PersonManager.ServiceContracts.Modules.Users.Dtos;

namespace PersonManager.ServiceContracts.Modules.Users;

public interface IUsersService
{
	Task<UserResponse> CreateUser(UserCreateRequest? userCreateRequest);

	Task<Paging<UserResponse>> GetUsers(GridifyQuery query);

	Task<UserResponse?> UpdateUser(UserUpdateRequest? userUpdateRequest);

	Task<UserResponse?> DeleteUser(Guid? userId);
}
