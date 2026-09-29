using System.Globalization;
using Entities.Modules.Countries;
using Entities.Modules.Users;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Countries.Enums;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using ServiceContracts.Modules.Users.Enums;
using Services.Common.Helpers;

namespace Services.Modules.Users;

public class UsersService(ICountriesService countriesService, bool initialize = true) : IUsersService
{
	private readonly List<User> _users = initialize ? CreateMockData(countriesService) : [];

	private static List<User> CreateMockData(ICountriesService countriesService)
	{
		List<Country> countries =
		[
			.. countriesService
				.GetCountries(new SearchQuery<CountrySearchOptions>())
				.Select(c => new Country { Id = c.Id, Name = c.Name }),
		];

		List<string> names = ["김아무개", "박아무개", "최아무개", "류아무개", "강아무개"];

		return
		[
			.. names.Select(
				(name, i) =>
					new User
					{
						Id = Guid.NewGuid(),
						Name = name,
						Email = $"user{i + 1}@example.com",
						DateOfBirth = new DateTime(1990 + i, i + 1, 10),
						Gender = i % 2 == 0 ? "Male" : "Female",
						Country = countries.ElementAtOrDefault(i),
						Address = $"서울시 {i + 1}번지",
						ReceiveNewsLetters = i % 2 == 0,
					}
			),
		];
	}

	private UserResponse ConvertUserToUserResponse(User user)
	{
		UserResponse userResponse = user.ToResponse();

		if (userResponse.Country is null)
			return userResponse;

		SearchQuery<CountrySearchOptions> searchQuery = new()
		{
			SearchBy = CountrySearchOptions.Name,
			SearchString = userResponse.Country.Name?.ToString(),
		};

		userResponse.Country = countriesService.GetCountries(searchQuery).FirstOrDefault();
		return userResponse;
	}

	public UserResponse CreateUser(UserCreateRequest? userCreateRequest)
	{
		ArgumentNullException.ThrowIfNull(userCreateRequest);

		ValidationHelper.ModelValidation(userCreateRequest);

		User user = userCreateRequest.ToEntity();
		user.Id = Guid.NewGuid();
		_users.Add(user);

		return ConvertUserToUserResponse(user);
	}

	public List<UserResponse> GetUsers(SearchQuery<UserSearchOptions> query)
	{
		List<UserResponse> users = [.. _users.Select(ConvertUserToUserResponse)];

		return SortUsers(FilterUsers(users, query), query);
	}

	private static List<UserResponse> FilterUsers(List<UserResponse> users, SearchQuery<UserSearchOptions> query)
	{
		string? searchString = query.SearchString;

		if (query.SearchBy is null || string.IsNullOrEmpty(searchString))
			return users;

		return query.SearchBy switch
		{
			UserSearchOptions.Id =>
			[
				.. users.Where(temp => Guid.TryParse(searchString, out Guid userId) && temp.Id == userId),
			],

			UserSearchOptions.Name =>
			[
				.. users.Where(temp => temp.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true),
			],

			UserSearchOptions.Email =>
			[
				.. users.Where(temp => temp.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true),
			],

			UserSearchOptions.DateOfBirth =>
			[
				.. users.Where(temp =>
					temp.DateOfBirth?.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)
						.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			UserSearchOptions.Gender =>
			[
				.. users.Where(temp =>
					string.Equals(temp.Gender?.ToString(), searchString, StringComparison.OrdinalIgnoreCase)
				),
			],

			UserSearchOptions.Country =>
			[
				.. users.Where(temp =>
					temp.Country?.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			UserSearchOptions.Address =>
			[
				.. users.Where(temp =>
					temp.Address?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
				),
			],

			UserSearchOptions.ReceiveNewsLetters =>
			[
				.. users.Where(temp =>
					bool.TryParse(searchString, out bool receiveNewsLetters)
					&& temp.ReceiveNewsLetters == receiveNewsLetters
				),
			],

			_ => users,
		};
	}

	private static List<UserResponse> SortUsers(List<UserResponse> users, SearchQuery<UserSearchOptions> query)
	{
		if (query.SortBy is null || !Enum.IsDefined(query.SortOrder))
			return users;

		return (query.SortBy, query.SortOrder) switch
		{
			(UserSearchOptions.Id, SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.Id)],

			(UserSearchOptions.Id, SortOrderOptions.DESC) => [.. users.OrderByDescending(temp => temp.Id)],

			(UserSearchOptions.Name, SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Name, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Email, SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Email, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.DateOfBirth, SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.DateOfBirth)],

			(UserSearchOptions.DateOfBirth, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.DateOfBirth),
			],

			(UserSearchOptions.Gender, SortOrderOptions.ASC) => [.. users.OrderBy(temp => temp.Gender)],

			(UserSearchOptions.Gender, SortOrderOptions.DESC) => [.. users.OrderByDescending(temp => temp.Gender)],

			(UserSearchOptions.Country, SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Country?.Name, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Country, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Country?.Name, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Address, SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.Address, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase),
			],

			(UserSearchOptions.ReceiveNewsLetters, SortOrderOptions.ASC) =>
			[
				.. users.OrderBy(temp => temp.ReceiveNewsLetters),
			],

			(UserSearchOptions.ReceiveNewsLetters, SortOrderOptions.DESC) =>
			[
				.. users.OrderByDescending(temp => temp.ReceiveNewsLetters),
			],

			_ => users,
		};
	}

	public UserResponse? UpdateUser(UserUpdateRequest? userUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(userUpdateRequest);

		ValidationHelper.ModelValidation(userUpdateRequest);

		User? matchingUser = _users.FirstOrDefault(temp => temp.Id == userUpdateRequest.Id);

		if (matchingUser == null)
			return null;

		// null 은 "보내지 않음" 으로 보고 기존 값을 유지
		matchingUser.Name = userUpdateRequest.Name ?? matchingUser.Name;
		matchingUser.Email = userUpdateRequest.Email ?? matchingUser.Email;
		matchingUser.DateOfBirth = userUpdateRequest.DateOfBirth ?? matchingUser.DateOfBirth;
		matchingUser.Gender = userUpdateRequest.Gender?.ToString() ?? matchingUser.Gender;
		matchingUser.Country = userUpdateRequest.Country ?? matchingUser.Country;
		matchingUser.Address = userUpdateRequest.Address ?? matchingUser.Address;
		matchingUser.ReceiveNewsLetters = userUpdateRequest.ReceiveNewsLetters ?? matchingUser.ReceiveNewsLetters;

		return ConvertUserToUserResponse(matchingUser);
	}

	public UserResponse? DeleteUser(Guid? userId)
	{
		if (!userId.HasValue)
		{
			throw new ArgumentNullException(nameof(userId));
		}

		int index = _users.FindIndex(temp => temp.Id == userId);
		if (index < 0)
			return null;

		User matchingUser = _users[index];
		_users.RemoveAt(index);

		return ConvertUserToUserResponse(matchingUser);
	}
}
