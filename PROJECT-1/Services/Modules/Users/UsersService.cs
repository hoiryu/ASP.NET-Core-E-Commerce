using System.Globalization;
using Entities.Modules.Users;
using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using Services.Common.Helpers;

namespace Services.Modules.Users;

public class UsersService(ICountriesService countriesService) : IUsersService
{
	private readonly List<User> _users = [];

	private UserResponse ConvertUserToUserResponse(User user)
	{
		UserResponse userResponse = user.ToResponse();
		userResponse.Country = countriesService.GetCountryByCountryId(userResponse.Country?.Id);
		return userResponse;
	}

	public UserResponse AddUser(UserAddRequest? userAddRequest)
	{
		if (userAddRequest == null)
			throw new ArgumentException(nameof(UserAddRequest));

		ValidationHelper.ModelValidation(userAddRequest);

		User user = userAddRequest.ToEntity();
		user.Id = Guid.NewGuid();
		_users.Add(user);

		return ConvertUserToUserResponse(user);
	}

	public List<UserResponse> GetAllUsers()
	{
		return [.. _users.Select(ConvertUserToUserResponse)];
	}

	public UserResponse? GetUserByUserId(Guid? userId)
	{
		if (userId == null)
			return null;

		User? user = _users.FirstOrDefault(temp => temp.Id == userId);
		if (user == null)
			return null;

		return ConvertUserToUserResponse(user);
	}

	public List<UserResponse> GetFilteredUsers(string searchBy, string searchString)
	{
		List<UserResponse> users = GetAllUsers();
		List<UserResponse> matchingUsers = users;

		if (string.IsNullOrEmpty(searchBy) && string.IsNullOrEmpty(searchString))
			return matchingUsers;

		return searchBy switch
		{
			nameof(User.Name) =>
			[
				.. users.Where(temp => temp.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true),
			],

			nameof(User.Email) =>
			[
				.. users.Where(temp => temp.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true),
			],

			nameof(User.DateOfBirth) =>
			[
				.. users.Where(temp =>
					temp.DateOfBirth?.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)
						.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			nameof(User.Gender) =>
			[
				.. users.Where(temp =>
					string.Equals(temp.Gender?.ToString(), searchString, StringComparison.OrdinalIgnoreCase)
				),
			],

			nameof(User.Country) =>
			[
				.. users.Where(temp =>
					temp.Country?.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			nameof(User.Address) =>
			[
				.. users.Where(temp =>
					temp.Address?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			_ => users,
		};
	}

	public List<UserResponse> GetSortedUsers(List<UserResponse> users, string sortBy, SortOrderOptions sortOrderOptions)
	{
		if (string.IsNullOrEmpty(sortBy) || !Enum.IsDefined(sortOrderOptions))
			return users;

		return (sortBy, sortOrderOptions) switch
		{
			(nameof(UserResponse.Name), SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Name), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Email), SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Email), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.DateOfBirth), SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.DateOfBirth)],

			(nameof(UserResponse.DateOfBirth), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.DateOfBirth),
			],

			(nameof(UserResponse.Age), SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.Age)],

			(nameof(UserResponse.Age), SortOrderOptions.DESC) => [.. users.OrderByDescending(temp => temp.Age)],

			(nameof(UserResponse.Gender), SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.Gender)],

			(nameof(UserResponse.Gender), SortOrderOptions.DESC) => [.. users.OrderByDescending(temp => temp.Gender)],

			(nameof(UserResponse.Country), SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Country?.Name, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Country), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Country?.Name, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Address), SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.Address), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase),
			],

			(nameof(UserResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.ReceiveNewsLetters),
			],

			(nameof(UserResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.ReceiveNewsLetters),
			],

			_ => users,
		};
	}
}
