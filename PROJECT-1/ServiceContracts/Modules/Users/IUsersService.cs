using ServiceContracts.Modules.Users.Dtos;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	UserResponse AddUser(UserAddRequest? userAddRequest);

	List<UserResponse> GetAllUsers();
}
