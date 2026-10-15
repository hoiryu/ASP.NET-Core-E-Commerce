using Gridify;
using Microsoft.AspNetCore.Mvc;
using PersonManager.ServiceContracts.Modules.Countries;
using PersonManager.ServiceContracts.Modules.Countries.Dtos;

namespace PersonManager.Api.Controllers.Modules.Countries
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountriesService _countriesService) : ControllerBase
	{
		[HttpPost]
		public async Task<ActionResult<CountryResponse>> CreateCountry([FromBody] CountryCreateRequest countryCreateRequest)
		{
			return await _countriesService.CreateCountry(countryCreateRequest);
		}

		[HttpGet]
		public async Task<ActionResult<Paging<CountryResponse>>> GetCountries([FromQuery] GridifyQuery query)
		{
			return await _countriesService.GetCountries(query);
		}

		/// <summary>
		/// filter 에 맞는 국가 일괄 수정 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpPatch]
		public async Task<ActionResult<object>> UpdateCountries(
			[FromQuery] GridifyQuery query,
			[FromBody] CountryUpdateRequest countryUpdateRequest
		)
		{
			// 빈 filter 는 전체 행이 대상이 되므로 거부
			if (string.IsNullOrWhiteSpace(query.Filter))
				return BadRequest("Filter is required");

			return new { count = await _countriesService.UpdateCountries(query.Filter, countryUpdateRequest) };
		}

		/// <summary>
		/// filter 에 맞는 국가 일괄 삭제 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpDelete]
		public async Task<ActionResult<object>> DeleteCountries([FromQuery] GridifyQuery query)
		{
			// 빈 filter 는 전체 행이 대상이 되므로 거부
			if (string.IsNullOrWhiteSpace(query.Filter))
				return BadRequest("Filter is required");

			return new { count = await _countriesService.DeleteCountries(query.Filter) };
		}
	}
}
