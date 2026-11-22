using Gridify;
using PersonManager.Entities.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.RepositoryContracts.Modules.Identity.Users;

public interface IUsersRepository
{
	Task<List<User>> CreateUsers(List<User> users);

	Task<Paging<User>> GetUsers(GridifyQuery query);

	Task<int> UpdateUsers(string filter, UserUpdate values);

	Task<int> DeleteUsers(string filter);
}
