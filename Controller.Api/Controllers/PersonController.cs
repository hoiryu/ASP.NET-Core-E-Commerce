using Controller.Api.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PersonController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var controllerName = ControllerContext.RouteData.Values["controller"];
			return Ok(new { message = $"controller name: {controllerName}" });
		}

		[HttpGet("{personId:int}")]
		public ActionResult<Person> GetPerson([FromRoute] int personId, [FromQuery] string? name, [FromQuery] int? age)
		{
			if (string.IsNullOrEmpty(name))
			{
				return BadRequest("Name is not supplied or empty");
			}

			if (age is null || age <= 0)
			{
				return BadRequest("Age can't be less than or equal to 0");
			}

			Person person = new()
			{
				Id = Guid.NewGuid(),
				PersonId = personId,
				Name = name,
				Age = age,
			};

			return person;
		}
	}
}
