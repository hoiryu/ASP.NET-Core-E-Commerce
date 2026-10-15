using Gridify;
using PersonManager.Entities.Modules.Users;
using PersonManager.RepositoryContracts.Modules.Countries;
using PersonManager.RepositoryContracts.Modules.Users;
using PersonManager.ServiceContracts.Modules.Users;
using PersonManager.ServiceContracts.Modules.Users.Dtos;
using PersonManager.ServiceContracts.Modules.Users.Mappings;
using PersonManager.Services.Common.Helpers;

namespace PersonManager.Services.Modules.Users;

public class UsersService(IUsersRepository _usersRepository, ICountriesRepository _countriesRepository) : IUsersService
{
	public async Task<UserResponse> CreateUser(UserCreateRequest? userCreateRequest)
	{
		ArgumentNullException.ThrowIfNull(userCreateRequest);

		ValidationHelper.ModelValidation(userCreateRequest);

		if (
			userCreateRequest.Country?.Id is Guid countryId
			&& !await _countriesRepository.ExistsCountryById(countryId)
		)
			throw new ArgumentException("Given country id doesn't exist");

		User user = await _usersRepository.CreateUser(userCreateRequest.ToEntity());

		return user.ToResponse();
	}

	public async Task<Paging<UserResponse>> GetUsers(GridifyQuery query)
	{
		Paging<User> users = await _usersRepository.GetUsers(query);

		return new Paging<UserResponse>(users.Count, users.Data.Select(user => user.ToResponse()));
	}

	public async Task<int> UpdateUsers(string? filter, UserUpdateRequest? userUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(userUpdateRequest);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (
			userUpdateRequest.Country?.Id is Guid countryId
			&& !await _countriesRepository.ExistsCountryById(countryId)
		)
			throw new ArgumentException("Given country id doesn't exist");

		return await _usersRepository.UpdateUsers(filter ?? "", userUpdateRequest.ToUpdate());
	}

	public async Task<int> DeleteUsers(string? filter)
	{
		return await _usersRepository.DeleteUsers(filter ?? "");
	}
}
