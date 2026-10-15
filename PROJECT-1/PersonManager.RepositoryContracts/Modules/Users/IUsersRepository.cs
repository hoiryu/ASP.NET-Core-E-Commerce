using PersonManager.Entities.Modules.Users;
using Gridify;

namespace PersonManager.RepositoryContracts.Modules.Users;

public interface IUsersRepository
{
	Task<User> CreateUser(User user);

	Task<Paging<User>> GetUsers(GridifyQuery query);

	Task<User?> GetUserById(Guid userId);

	Task<User> UpdateUser(User user);

	Task<User> DeleteUser(User user);
}
