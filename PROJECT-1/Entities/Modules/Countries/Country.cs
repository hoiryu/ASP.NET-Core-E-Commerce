using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Entities.Modules.Countries;

[Index(nameof(Name), IsUnique = true)]
public class Country
{
	[Key]
	public Guid Id { get; set; }

	[StringLength(100)]
	public string? Name { get; set; }
}
