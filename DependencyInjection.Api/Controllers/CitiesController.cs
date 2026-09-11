using DependencyInjection.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;

namespace DependencyInjection.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController(ICitiesService citiesService, IOptions<ClientApiOptions> clientApiOptions)
		: ControllerBase
	{
		private readonly ICitiesService _citiesService = citiesService;
		private readonly ClientApiOptions _clientApiOptions = clientApiOptions.Value;

		[HttpGet()]
		public ActionResult<List<string>> GetCities()
		{
			var cities = _citiesService.GetCities();
			return cities;
		}

		[HttpGet("options")]
		public ActionResult<ClientApiOptions> GetOptions()
		{
			return _clientApiOptions;
		}
	}
}
