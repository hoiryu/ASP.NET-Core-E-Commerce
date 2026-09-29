using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Countries.Enums;

namespace PersonManager.Api.Controllers.Modules.Countries
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountriesService countriesService) : ControllerBase
	{
		public ActionResult<List<CountryResponse>> GetCountries([FromQuery] SearchQuery<CountrySearchOptions> query)
		{
			return countriesService.GetCountries(query);
		}
	}
}
