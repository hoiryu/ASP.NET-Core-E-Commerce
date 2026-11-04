using Gridify;
using PersonManager.Entities.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;
using PersonManager.Services.Common.Helpers;
using PersonManager.Services.Modules.Identity.Users.Mappings;

namespace PersonManager.Services.Modules.Identity.Users;

public class UsersService(IUsersRepository _usersRepository, ICountriesRepository _countriesRepository) : IUsersService
{
	public async Task<List<UserResponse>> CreateUsers(List<UserCreateRequest> userCreateRequests)
	{
		// 저장 전에 전체를 검증 (하나라도 실패하면 아무것도 저장하지 않음)
		ValidationHelper.ModelValidation(userCreateRequests);

		List<Guid> countryIds = [.. userCreateRequests.Select(temp => temp.Country?.Id).OfType<Guid>().Distinct()];
		if (countryIds.Count > 0 && !await _countriesRepository.ExistsCountriesByIds(countryIds))
			throw new ArgumentException("Given country id doesn't exist");

		List<User> users = await _usersRepository.CreateUsers([.. userCreateRequests.Select(temp => temp.ToEntity())]);

		return [.. users.Select(user => user.ToResponse())];
	}

	public async Task<Paging<UserResponse>> GetUsers(GridifyQuery query)
	{
		Paging<User> users = await _usersRepository.GetUsers(query);

		return new Paging<UserResponse>(users.Count, users.Data.Select(user => user.ToResponse()));
	}

	public async Task<int> UpdateUsers(string? filter, UserUpdateRequest userUpdateRequest)
	{
		ValidationHelper.FilterValidation(filter);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (userUpdateRequest.Country?.Id is Guid countryId && !await _countriesRepository.ExistsCountryById(countryId))
			throw new ArgumentException("Given country id doesn't exist");

		return await _usersRepository.UpdateUsers(filter, userUpdateRequest.ToUpdate());
	}

	public async Task<int> DeleteUsers(string? filter)
	{
		ValidationHelper.FilterValidation(filter);

		return await _usersRepository.DeleteUsers(filter);
	}
}
