using Entities.Modules.Users;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users.Dtos;

public record UserResponse
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public Guid? CountryId { get; set; }
	public string? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }
	public double? Age { get; set; }
}

public static class UserExtensions
{
	public static UserResponse ToResponse(this User user) =>
		new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			DateOfBirth = user.DateOfBirth,
			Gender = Enum.TryParse<GenderOptions>(user.Gender, out var gender) ? gender : null,
			CountryId = user.CountryId,
			Address = user.Address,
			ReceiveNewsLetters = user.ReceiveNewsLetters,
			Age = (user.DateOfBirth != null) ? ((DateTime.Now - user.DateOfBirth.Value).TotalDays / 365.25) : null,
		};
}
