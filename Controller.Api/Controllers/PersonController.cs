using Controller.Api.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PersonController : ControllerBase
	{
		[HttpPost("register")]
		public ActionResult<Person> CreatePerson([FromForm] Person person)
		{
			person.Id = Guid.NewGuid();
			return person;
		}

		[HttpGet()]
		public ActionResult<Person> GetPerson([FromQuery] string? name, [FromQuery] int? age)
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
				Name = name,
				Email = "test@mail.com",
				Phone = "01011112222",
				Password = "1q2w3e4r",
				ConfirmPassword = "1q2w3e4r",
				Age = age,
			};

			return person;
		}
	}
}
