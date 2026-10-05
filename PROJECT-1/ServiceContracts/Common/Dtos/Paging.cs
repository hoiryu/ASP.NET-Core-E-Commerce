using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.Common.Dtos;

/// <summary>
/// 페이징 조건 (null 인 속성은 적용하지 않음)
/// </summary>
public record Paging
{
	[Range(0, int.MaxValue, ErrorMessage = "Skip must not be negative")]
	public int? Skip { get; set; }

	[Range(0, int.MaxValue, ErrorMessage = "Take must not be negative")]
	public int? Take { get; set; }
}
