namespace PersonManager.RepositoryContracts.Modules.Identity.Countries.Dtos;

/// <summary>
/// 수정할 값 (null 은 "변경하지 않음")
/// </summary>
public record CountryUpdate(string? Name);
