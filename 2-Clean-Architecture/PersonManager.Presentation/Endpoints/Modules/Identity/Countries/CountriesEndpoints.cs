using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Countries.Dtos;
using PersonManager.Presentation.Endpoints.Common.Dtos;
using PersonManager.Presentation.Endpoints.Common.EndpointFilters;
using PersonManager.Presentation.Endpoints.Common.Parameters;

namespace PersonManager.Presentation.Endpoints.Modules.Identity.Countries;

public static class CountriesEndpoints
{
	public static RouteGroupBuilder MapCountriesEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/countries");

		group.MapPost("/", CreateCountries).AddEndpointFilter<MutationEndpointFilter>();
		group.MapGet("/", GetCountries);
		group.MapPatch("/", UpdateCountries).AddEndpointFilter<MutationEndpointFilter>();
		group.MapDelete("/", DeleteCountries).AddEndpointFilter<MutationEndpointFilter>();

		return group;
	}

	/// <summary>
	/// 국가 일괄 생성 (한 건도 배열로 전달)
	/// </summary>
	private static async Task<List<CountryResponse>> CreateCountries(
		List<CountryCreateRequest> countryCreateRequests,
		ICountriesService countriesService
	)
	{
		return await countriesService.CreateCountries(countryCreateRequests);
	}

	private static async Task<PagedResult<CountryResponse>> GetCountries(
		[AsParameters] ListQueryParameters query,
		ICountriesService countriesService
	)
	{
		return await countriesService.GetCountries(query.ToListQuery());
	}

	/// <summary>
	/// filter 에 맞는 국가 일괄 수정 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<MutationResponse> UpdateCountries(
		string? filter,
		CountryUpdateRequest countryUpdateRequest,
		ICountriesService countriesService
	)
	{
		var count = await countriesService.UpdateCountries(filter, countryUpdateRequest);

		return new MutationResponse(count);
	}

	/// <summary>
	/// filter 에 맞는 국가 일괄 삭제 (orderBy·page·pageSize 는 무시)
	/// </summary>
	private static async Task<MutationResponse> DeleteCountries(string? filter, ICountriesService countriesService)
	{
		var count = await countriesService.DeleteCountries(filter);

		return new MutationResponse(count);
	}
}
