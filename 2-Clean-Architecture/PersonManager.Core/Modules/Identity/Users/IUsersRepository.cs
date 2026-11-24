using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Users.Dtos;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Core.Modules.Identity.Users;

public interface IUsersRepository
{
	Task<List<User>> CreateUsers(List<User> users);

	Task<PagedResult<User>> GetUsers(ListQuery query);

	Task<int> UpdateUsers(string filter, UserUpdate values);

	Task<int> DeleteUsers(string filter);
}
