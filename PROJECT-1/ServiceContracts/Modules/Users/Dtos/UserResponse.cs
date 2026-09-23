using Entities.Modules.Users;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users.Dtos;

public record UserResponse
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public CountryResponse? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }
	public double? Age { get; set; }

	public override string ToString()
	{
		return $"User Id: {Id}, Name: {Name}, Email: {Email}, DateOfBirth: {DateOfBirth}, Gender: {Gender}, Country: {Country}";
	}
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
			Country = user.Country?.ToResponse(),
			Gender = Enum.TryParse<GenderOptions>(user.Gender, out var gender) ? gender : null,
			Address = user.Address,
			ReceiveNewsLetters = user.ReceiveNewsLetters,
			Age = (user.DateOfBirth != null) ? ((DateTime.Now - user.DateOfBirth.Value).TotalDays / 365.25) : null,
		};
}
