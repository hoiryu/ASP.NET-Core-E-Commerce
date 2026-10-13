using System.ComponentModel.DataAnnotations;

namespace PersonManager.Services.Common.Helpers;

public static class ValidationHelper
{
	internal static void ModelValidation(object obj)
	{
		ValidationContext validationContext = new(obj);

		List<ValidationResult> validationResults = [];

		bool isValid = Validator.TryValidateObject(
			obj,
			validationContext,
			validationResults,
			validateAllProperties: true
		);

		if (!isValid)
			throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
	}
}
