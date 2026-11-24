using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Core.Modules.Identity.Users.Dtos;

/// <summary>
/// 수정할 값 (null 은 "변경하지 않음")
/// </summary>
public record UserUpdate(
	string? Name,
	string? Email,
	DateTime? DateOfBirth,
	Gender? Gender,
	Guid? CountryId,
	string? Address,
	bool? ReceiveNewsLetters
);
