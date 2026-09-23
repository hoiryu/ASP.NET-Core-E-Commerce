using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Users.Dtos;

namespace ServiceContracts.Modules.Users;

public interface IUsersService
{
	UserResponse AddUser(UserAddRequest? userAddRequest);

	List<UserResponse> GetAllUsers();

	UserResponse? GetUserByUserId(Guid? userId);

	List<UserResponse> GetFilteredUsers(string searchBy, string searchString);

	List<UserResponse> GetSortedUsers(List<UserResponse> users, string sortBy, SortOrderOptions sortOrderOptions);
}
