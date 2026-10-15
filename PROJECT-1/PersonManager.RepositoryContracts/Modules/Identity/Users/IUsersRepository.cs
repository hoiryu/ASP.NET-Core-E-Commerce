using PersonManager.Entities.Modules.Identity.Users;
using Gridify;
using PersonManager.RepositoryContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.RepositoryContracts.Modules.Identity.Users;

public interface IUsersRepository
{
	Task<User> CreateUser(User user);

	Task<Paging<User>> GetUsers(GridifyQuery query);

	Task<int> UpdateUsers(string filter, UserUpdate values);

	Task<int> DeleteUsers(string filter);
}
