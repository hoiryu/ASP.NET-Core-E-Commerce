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
			[FromQuery(Name = "filter")] CountryFilter filter,
			[FromQuery(Name = "sort")] CountrySort sort
		)
		{
			return _countriesService.GetCountries(filter, sort);
		}
	}
}
