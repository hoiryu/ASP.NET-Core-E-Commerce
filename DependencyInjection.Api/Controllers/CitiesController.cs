using Microsoft.AspNetCore.Mvc;
using Services;

namespace DependencyInjection.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController(CitiesService citiesService) : ControllerBase
	{
		private readonly CitiesService _citiesService = citiesService;

		[HttpGet()]
		public ActionResult<List<string>> GetCities()
		{
			var cities = _citiesService.GetCities();
			return cities;
		}
	}
}
