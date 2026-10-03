using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;

namespace PersonManager.Api.Controllers.Modules.Countries
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountriesService _countriesService) : ControllerBase
	{
		public ActionResult<CountryResponse> CreateCountry([FromBody] CountryCreateRequest countryCreateRequest)
		{
			return _countriesService.CreateCountry(countryCreateRequest);
		}

		[HttpGet]
		public ActionResult<List<CountryResponse>> GetCountries(
			[FromQuery] CountryFilter filter,
			[FromQuery] CountrySort sort
		)
		{
			return _countriesService.GetCountries(filter, sort);
		}

		[HttpPatch("{countryId:guid}")]
		public ActionResult<CountryResponse?> UpdateCountry(
			[FromRoute] Guid countryId,
			[FromBody] CountryUpdateRequest countryUpdateRequest
		)
		{
			countryUpdateRequest.Id = countryId;

			CountryResponse? countryResponse = _countriesService.UpdateCountry(countryUpdateRequest);

			if (countryResponse is null)
				return NotFound();

			return countryResponse;
		}

		[HttpDelete("{countryId:guid}")]
		public ActionResult<CountryResponse> DeleteCountry([FromRoute] Guid countryId)
		{
			CountryResponse? countryResponse = _countriesService.DeleteCountry(countryId);

			if (countryResponse is null)
				return NotFound();

			return countryResponse;
		}
	}
}
