using System.ComponentModel.DataAnnotations;
using Entities.Modules.Users;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using Services.Common.Helpers;
using Services.Modules.Countries;

namespace Services.Modules.Users;

public class UsersService : IUsersService
{
	private readonly List<User> _users = [];
	private readonly ICountriesService _countriesService;

	public UsersService()
	{
		_countriesService = new CountriesService();
	}

	private UserResponse ConvertUserToUserResponse(User user)
	{
		UserResponse userResponse = user.ToResponse();

		userResponse.Country = _countriesService.GetCountryByCountryId(user.CountryId)?.Name;

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
		throw new NotImplementedException();
	}
}
