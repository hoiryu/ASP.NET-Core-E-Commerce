using System.ComponentModel.DataAnnotations;

namespace DependencyInjection.Api.Options;

public class ClientApiOptions
{
	/// <summary>
	/// appsettings.json 의 설정 섹션 이름.
	/// </summary>
	public const string SectionName = "ClientApi";

	[Required]
	public string? ClientId { get; set; }

	[Required]
	public string? ClientKey { get; set; }
}
