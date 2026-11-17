using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using PersonManager.Exceptions;

namespace PersonManager.Services.Common.Helpers;

public static class ValidationHelper
{
	internal static void ModelValidation(object? obj)
	{
		// 배열 안의 null 항목 등
		if (obj is null)
			throw new InvalidRequestException("Request body is required");

		ValidationContext validationContext = new(obj);

		List<ValidationResult> validationResults = [];

		bool isValid = Validator.TryValidateObject(
			obj,
			validationContext,
			validationResults,
			validateAllProperties: true
		);

		if (!isValid)
			throw new InvalidRequestException(validationResults.FirstOrDefault()?.ErrorMessage ?? "Invalid request");
	}

	internal static void ModelValidation<T>(IEnumerable<T>? objs)
		where T : notnull
	{
		if (objs is null || !objs.Any())
			throw new InvalidRequestException($"At least one {typeof(T).Name} is required");

		foreach (T obj in objs)
			ModelValidation(obj);
	}

	// 빈 filter 는 전체 행이 대상이 되므로 거부
	internal static void FilterValidation([NotNull] string? filter)
	{
		if (string.IsNullOrWhiteSpace(filter))
			throw new InvalidRequestException("Filter is required");
	}
}
