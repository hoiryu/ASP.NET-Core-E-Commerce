using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Controller.Api.CustomValidators;

public class DateRangeValidatorAttribute : ValidationAttribute
{
	public string OtherPropertyName { get; set; }

	public string? OtherPropertyDisplayName { get; private set; }

	public string DefaultErrorMessage { get; set; } = "{0} must be after {1}";

	public DateRangeValidatorAttribute(string otherPropertyName)
	{
		OtherPropertyName = otherPropertyName;
	}

	public override string FormatErrorMessage(string name) =>
		string.Format(ErrorMessage ?? DefaultErrorMessage, name, OtherPropertyDisplayName ?? OtherPropertyName);

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
			return null;

		DateTime toDate = (DateTime)value;
		PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(OtherPropertyName);

		if (otherProperty is null)
			return null;

		DateTime fromDate = Convert.ToDateTime(otherProperty.GetValue(validationContext.ObjectInstance));

		if (fromDate > toDate)
		{
			OtherPropertyDisplayName ??= otherProperty.GetCustomAttribute<DisplayAttribute>()?.Name;

			return new ValidationResult(
				FormatErrorMessage(validationContext.DisplayName),
				new[] { validationContext.MemberName }.OfType<string>()
			);
		}

		return ValidationResult.Success;
	}
}
