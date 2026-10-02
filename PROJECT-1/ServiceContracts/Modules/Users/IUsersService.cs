using ServiceContracts.Modules.Users.Dtos;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	UserResponse CreateUser(UserCreateRequest? userCreateRequest);

	List<UserResponse> GetUsers(UserFilter filter, UserSort sort);

	UserResponse? UpdateUser(UserUpdateRequest? userUpdateRequest);

	UserResponse? DeleteUser(Guid? userId);
}
