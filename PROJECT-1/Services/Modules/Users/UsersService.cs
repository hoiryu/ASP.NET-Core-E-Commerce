using Entities.Modules.Users;
using Gridify;
using RepositoryContracts.Modules.Countries;
using RepositoryContracts.Modules.Users;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using Services.Common.Helpers;

namespace Services.Modules.Users;

public class UsersService(IUsersRepository _usersRepository, ICountriesRepository _countriesRepository) : IUsersService
{
	public async Task<UserResponse> CreateUser(UserCreateRequest? userCreateRequest)
	{
		ArgumentNullException.ThrowIfNull(userCreateRequest);

		ValidationHelper.ModelValidation(userCreateRequest);

		if (
			userCreateRequest.Country?.Id is Guid countryId
			&& (await _countriesRepository.GetCountries(new GridifyQuery { Filter = $"id={countryId}", PageSize = 1 })).Count == 0
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

	public async Task<UserResponse?> UpdateUser(UserUpdateRequest? userUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(userUpdateRequest);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (
			userUpdateRequest.Country?.Id is Guid countryId
			&& (await _countriesRepository.GetCountries(new GridifyQuery { Filter = $"id={countryId}", PageSize = 1 })).Count == 0
		)
			throw new ArgumentException("Given country id doesn't exist");

		User? matchingUser = (
			await _usersRepository.GetUsers(new GridifyQuery { Filter = $"id={userUpdateRequest.Id}", PageSize = 1 })
		).Data.FirstOrDefault();

		if (matchingUser == null)
			return null;

		// null 은 "보내지 않음" 으로 보고 기존 값을 유지
		matchingUser.Name = userUpdateRequest.Name ?? matchingUser.Name;
		matchingUser.Email = userUpdateRequest.Email ?? matchingUser.Email;
		matchingUser.DateOfBirth = userUpdateRequest.DateOfBirth ?? matchingUser.DateOfBirth;
		matchingUser.Gender = userUpdateRequest.Gender?.ToString() ?? matchingUser.Gender;
		matchingUser.CountryId = userUpdateRequest.Country?.Id ?? matchingUser.CountryId;
		matchingUser.Address = userUpdateRequest.Address ?? matchingUser.Address;
		matchingUser.ReceiveNewsLetters = userUpdateRequest.ReceiveNewsLetters ?? matchingUser.ReceiveNewsLetters;

		User updatedUser = await _usersRepository.UpdateUser(matchingUser);

		return updatedUser.ToResponse();
	}

	public async Task<UserResponse?> DeleteUser(Guid? userId)
	{
		if (!userId.HasValue)
		{
			throw new ArgumentNullException(nameof(userId));
		}

		User? deletedUser = await _usersRepository.DeleteUser(userId.Value);

		return deletedUser?.ToResponse();
	}
}
