using PersonManager.Entities.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Users.Dtos;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Mappings;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;
using PersonManager.ServiceContracts.Modules.Identity.Users.Enums;

namespace PersonManager.ServiceContracts.Modules.Identity.Users.Mappings;

/// <summary>
/// User 관련 DTO ↔ Entity 변환
/// </summary>
public static class UserMappings
{
	public static User ToEntity(this UserCreateRequest request) =>
		new()
		{
			Name = request.Name,
			Email = request.Email,
			DateOfBirth = request.DateOfBirth,
			Gender = request.Gender?.ToString(),
			CountryId = request.Country?.Id,
			Address = request.Address,
			ReceiveNewsLetters = request.ReceiveNewsLetters,
		};

	public static UserUpdate ToUpdate(this UserUpdateRequest request) =>
		new(
			request.Name,
			request.Email,
			request.DateOfBirth,
			request.Gender?.ToString(),
			request.Country?.Id,
			request.Address,
			request.ReceiveNewsLetters
		);

	public static UserResponse ToResponse(this User user) =>
		new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			DateOfBirth = user.DateOfBirth,
			Country = user.Country?.ToResponse(),
			Gender = Enum.TryParse<GenderOptions>(user.Gender, out var gender) ? gender : null,
			Address = user.Address,
			ReceiveNewsLetters = user.ReceiveNewsLetters,
			Age = (user.DateOfBirth != null) ? ((DateTime.Now - user.DateOfBirth.Value).TotalDays / 365.25) : null,
		};
}
