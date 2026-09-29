using ServiceContracts.Common.Enums;

namespace ServiceContracts.Common.Dtos;

public class SearchQuery<T>
	where T : struct, Enum
{
	public T? SearchBy { get; set; }
	public string? SearchString { get; set; }
	public T? SortBy { get; set; }
	public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;
}
