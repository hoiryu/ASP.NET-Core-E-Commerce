using PersonManager.Entities.Modules.Users;
using Gridify;
using PersonManager.RepositoryContracts.Modules.Users.Dtos;

namespace PersonManager.RepositoryContracts.Modules.Users;

public interface IUsersRepository
{
	Task<User> CreateUser(User user);

	Task<Paging<User>> GetUsers(GridifyQuery query);

	Task<int> UpdateUsers(string filter, UserUpdate values);

	Task<int> DeleteUsers(string filter);
}
