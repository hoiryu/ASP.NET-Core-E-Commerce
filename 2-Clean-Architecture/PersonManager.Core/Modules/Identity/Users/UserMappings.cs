using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Users.Dtos;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Core.Modules.Identity.Users;

/// <summary>
/// User 관련 DTO ↔ Entity 변환
/// </summary>
internal static class UserMappings
{
	public static User ToEntity(this UserCreateRequest request) =>
		new()
		{
			Name = request.Name,
			Email = request.Email,
			DateOfBirth = request.DateOfBirth,
			Gender = request.Gender,
			CountryId = request.Country?.Id,
			Address = request.Address,
			ReceiveNewsLetters = request.ReceiveNewsLetters,
		};

	public static UserUpdate ToUpdate(this UserUpdateRequest request) =>
		new(
			request.Name,
			request.Email,
			request.DateOfBirth,
			request.Gender,
			request.Country?.Id,
			request.Address,
			request.ReceiveNewsLetters
		);

	/// <param name="now">나이 계산 기준 시각</param>
	public static UserResponse ToResponse(this User user, DateTime now) =>
		new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			DateOfBirth = user.DateOfBirth,
			Country = user.Country?.ToResponse(),
			Gender = user.Gender,
			Address = user.Address,
			ReceiveNewsLetters = user.ReceiveNewsLetters,
			Age = user.GetAge(now),
		};
}
