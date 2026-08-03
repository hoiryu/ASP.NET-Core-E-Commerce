using System.ComponentModel.DataAnnotations;
using Controller.Api.CustomValidators;

namespace Controller.Api.Controllers.Models;

public class Person
{
	[Required]
	public Guid Id { get; set; }

	[Required(ErrorMessage = "{0} can't be empty or null")]
	[Display(Name = "Person Name")]
	[Length(3, 4, ErrorMessage = "{0} should be between {1} and {2} characters long")]
	public required string Name { get; set; }

	[Required(ErrorMessage = "{0} can't be empty or null")]
	[EmailAddress(ErrorMessage = "{0} should  be a proper email")]
	public required string Email { get; set; }

	[Phone(ErrorMessage = "{0} should contain 10 digits")]
	public string? Phone { get; set; }

	[Required(ErrorMessage = "{0} can't be empty or null")]
	public required string Password { get; set; }

	[Required(ErrorMessage = "{0} can't be empty or null")]
	[Compare("Password", ErrorMessage = "{0} and {1} do not match")]
	public required string ConfirmPassword { get; set; }

	[Range(0, 999, ErrorMessage = "{0} should be between {1} and {2}")]
	public decimal? Price { get; set; }

	public int? Age { get; set; }

	// [MinimumYearValidatorAttribute(2000, ErrorMessage = "{0} should not be newer than Jan 01, {1}")]
	[MinimumYearValidatorAttribute(2000)]
	[Display(Name = "Date of birth")]
	public DateTime? DateOfBirth { get; set; }
}
