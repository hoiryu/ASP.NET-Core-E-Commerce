using Controller.Api.Models;
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
		public ActionResult<Person> GetPerson()
		{
			Person person = new()
			{
				Id = Guid.NewGuid(),
				Name = "김아무개",
				Email = "test@mail.com",
				Phone = "01011112222",
				Password = "1q2w3e4r",
				ConfirmPassword = "1q2w3e4r",
				Age = 20,
			};

			return person;
		}

		[HttpGet(":name")]
		public ActionResult<Person> GetPersonByName([FromRoute] string? name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return BadRequest("Name is not supplied or empty");
			}

			Person person = new()
			{
				Id = Guid.NewGuid(),
				Name = name,
				Email = "test@mail.com",
				Phone = "01011112222",
				Password = "1q2w3e4r",
				ConfirmPassword = "1q2w3e4r",
				Age = 20,
			};

			return person;
		}
	}
}
