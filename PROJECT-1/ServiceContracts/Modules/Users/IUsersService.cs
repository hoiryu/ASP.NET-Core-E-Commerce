using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Users.Dtos;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	Task<UserResponse> CreateUser(UserCreateRequest? userCreateRequest);

	Task<List<UserResponse>> GetUsers(UserFilter filter, UserOrder order, Paging paging);

	Task<UserResponse?> UpdateUser(UserUpdateRequest? userUpdateRequest);

	Task<UserResponse?> DeleteUser(Guid? userId);
}
