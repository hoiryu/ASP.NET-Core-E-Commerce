using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace DependencyInjection.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController(ICitiesService citiesService) : ControllerBase
	{
		private readonly ICitiesService _citiesService = citiesService;

		[HttpGet()]
		public ActionResult<List<string>> GetCities()
		{
			var cities = _citiesService.GetCities();
			return cities;
		}
	}
}
