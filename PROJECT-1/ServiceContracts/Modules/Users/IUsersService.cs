using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Users.Dtos;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	UserResponse CreateUser(UserCreateRequest? userCreateRequest);

	List<UserResponse> GetUsers(SearchQuery<UserSearchOptions> query);

	UserResponse? UpdateUser(UserUpdateRequest? userUpdateRequest);

	UserResponse? DeleteUser(Guid? userId);
}
