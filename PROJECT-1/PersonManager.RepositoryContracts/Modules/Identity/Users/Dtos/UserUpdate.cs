namespace PersonManager.RepositoryContracts.Modules.Identity.Users.Dtos;

/// <summary>
/// 수정할 값 (null 은 "변경하지 않음")
/// </summary>
public record UserUpdate(
	string? Name,
	string? Email,
	DateTime? DateOfBirth,
	string? Gender,
	Guid? CountryId,
	string? Address,
	bool? ReceiveNewsLetters
);
