using Gridify;
using PersonManager.Api.Endpoints.Common.Parameters;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using Serilog;

namespace PersonManager.Api.Endpoints.Modules.Identity.Countries;

public static class CountriesEndpoints
{
	public static RouteGroupBuilder MapCountriesEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/countries");

		group.MapPost("/", CreateCountries);
		group.MapGet("/", GetCountries);
		group.MapPatch("/", UpdateCountries);
		group.MapDelete("/", DeleteCountries);

		return group;
	}

	/// <summary>
	/// 국가 일괄 생성 (한 건도 배열로 전달)
	/// </summary>
	private static async Task<List<CountryResponse>> CreateCountries(
		List<CountryCreateRequest> countryCreateRequests,
		ICountriesService countriesService,
		IDiagnosticContext diagnosticContext
	)
	{
		var countries = await countriesService.CreateCountries(countryCreateRequests);
		diagnosticContext.Set("AffectedCount", countries.Count); // 요청 완료 로그에 생성된 행 수 추가

		return countries;
	}

	private static async Task<Paging<CountryResponse>> GetCountries(
		[AsParameters] GridifyQueryParameters query,
		ICountriesService countriesService
	)
	{
		return await countriesService.GetCountries(query.ToGridifyQuery());
	}

	/// <summary>
	/// filter 에 맞는 국가 일괄 수정 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<object> UpdateCountries(
		string? filter,
		CountryUpdateRequest countryUpdateRequest,
		ICountriesService countriesService,
		IDiagnosticContext diagnosticContext
	)
	{
		var count = await countriesService.UpdateCountries(filter, countryUpdateRequest);
		diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

		return new { count };
	}

	/// <summary>
	/// filter 에 맞는 국가 일괄 삭제 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<object> DeleteCountries(
		string? filter,
		ICountriesService countriesService,
		IDiagnosticContext diagnosticContext
	)
	{
		var count = await countriesService.DeleteCountries(filter);
		diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

		return new { count };
	}
}
