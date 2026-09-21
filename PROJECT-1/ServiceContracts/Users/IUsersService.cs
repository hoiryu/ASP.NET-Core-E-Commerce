using ServiceContracts.Users.Dtos;

namespace ServiceContracts.Users;

public interface IUsersService
{
	UserResponse AddUser(UserAddRequest userAddRequest);

	List<UserResponse> GetAllUsers();
}
