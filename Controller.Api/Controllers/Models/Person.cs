namespace Controller.Api.Controllers.Models;

public class Person
{
	public Guid Id { get; set; }

	public int PersonId { get; set; }

	public string? Name { get; set; }

	public int? Age { get; set; }
}
