using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Users.Dtos;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	UserResponse CreateUser(UserCreateRequest? userCreateRequest);

	List<UserResponse> GetUsers(UserFilter filter, UserOrder order, Paging paging);

	UserResponse? UpdateUser(UserUpdateRequest? userUpdateRequest);

	UserResponse? DeleteUser(Guid? userId);
}
