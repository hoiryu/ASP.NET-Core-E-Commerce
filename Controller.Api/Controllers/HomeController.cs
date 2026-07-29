using Controller.Api.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var controllerName = ControllerContext.RouteData.Values["controller"];
			return Ok(new { message = $"controller name: {controllerName}" });
		}

		[HttpGet("person")]
		public ActionResult<Person> GetPerson()
		{
			Person person = new()
			{
				Id = Guid.NewGuid(),
				FirstName = "류",
				LastName = "호이",
				Age = 33,
			};

			return person;
		}
	}
}
