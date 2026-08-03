using System.ComponentModel.DataAnnotations;

namespace Controller.Api.CustomValidators;

public class MinimumYearValidatorAttribute : ValidationAttribute
{
	public int MinimumYear { get; set; } = 2000;

	public string DefaultErrorMessage { get; set; } = "{0} year should not be less than {1}";

	public MinimumYearValidatorAttribute() { }

	public MinimumYearValidatorAttribute(int minimumYear)
	{
		MinimumYear = minimumYear;
	}

	public override string FormatErrorMessage(string name) =>
		string.Format(ErrorMessage ?? DefaultErrorMessage, name, MinimumYear);

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
			return null;

		DateTime date = (DateTime)value;

		if (date.Year >= MinimumYear)
		{
			return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
		}
		else
		{
			return ValidationResult.Success;
		}
	}
}
