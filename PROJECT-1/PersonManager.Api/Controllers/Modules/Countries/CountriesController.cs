using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;

namespace PersonManager.Api.Controllers.Modules.Countries
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountriesService _countriesService) : ControllerBase
	{
		public async Task<ActionResult<CountryResponse>> CreateCountry([FromBody] CountryCreateRequest countryCreateRequest)
		{
			return await _countriesService.CreateCountry(countryCreateRequest);
		}

		[HttpGet]
		public async Task<ActionResult<List<CountryResponse>>> GetCountries(
			[FromQuery] CountryFilter filter,
			[FromQuery(Name = "order")] CountryOrder order,
			[FromQuery] Paging paging
		)
		{
			return await _countriesService.GetCountries(filter, order, paging);
		}

		[HttpPatch("{countryId:guid}")]
		public async Task<ActionResult<CountryResponse?>> UpdateCountry(
			[FromRoute] Guid countryId,
			[FromBody] CountryUpdateRequest countryUpdateRequest
		)
		{
			countryUpdateRequest.Id = countryId;

			CountryResponse? countryResponse = await _countriesService.UpdateCountry(countryUpdateRequest);

			if (countryResponse is null)
				return NotFound();

			return countryResponse;
		}

		[HttpDelete("{countryId:guid}")]
		public async Task<ActionResult<CountryResponse>> DeleteCountry([FromRoute] Guid countryId)
		{
			CountryResponse? countryResponse = await _countriesService.DeleteCountry(countryId);

			if (countryResponse is null)
				return NotFound();

			return countryResponse;
		}
	}
}
