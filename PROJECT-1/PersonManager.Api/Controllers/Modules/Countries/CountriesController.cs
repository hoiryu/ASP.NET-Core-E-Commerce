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
