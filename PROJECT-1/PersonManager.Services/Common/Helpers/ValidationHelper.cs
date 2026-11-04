using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PersonManager.Services.Common.Helpers;

public static class ValidationHelper
{
	internal static void ModelValidation(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

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

	internal static void ModelValidation<T>(IEnumerable<T> objs)
		where T : notnull
	{
		ArgumentNullException.ThrowIfNull(objs);

		if (!objs.Any())
			throw new ArgumentException($"At least one {typeof(T).Name} is required");

		foreach (T obj in objs)
			ModelValidation(obj);
	}

	// 빈 filter 는 전체 행이 대상이 되므로 거부
	internal static void FilterValidation([NotNull] string? filter)
	{
		if (string.IsNullOrWhiteSpace(filter))
			throw new ArgumentException("Filter is required");
	}
}
