using Controller.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Controller.Api.CustomModelBinders;

public class PersonModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext);

		var age = bindingContext.ValueProvider.GetValue(nameof(Person.Age));
		var dateOfBirth = bindingContext.ValueProvider.GetValue(nameof(Person.DateOfBirth));

		if (age.Length <= 0 && dateOfBirth.Length <= 0)
		{
			bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Age or DateOfBirth is required");
			bindingContext.Result = ModelBindingResult.Failed();

			return Task.CompletedTask;
		}

		int? ageValue = ParseValue<int>(age);
		DateTime? dateOfBirthValue = ParseValue<DateTime>(dateOfBirth);

		if (age == ValueProviderResult.None)
		{
			// Age가 없으면 DateOfBirth로부터 계산해서 채움
			if (dateOfBirthValue.HasValue)
			{
				var today = DateTime.Today;
				var birthDate = dateOfBirthValue.Value;
				var currentAge = today.Year - birthDate.Year;

				// 아직 생일이 안 지났으면 나이를 하나 빼줌 (예: 오늘이 8월인데 생일이 12월이면 -1)
				if (birthDate.Date > today.AddYears(-currentAge))
				{
					currentAge--;
				}

				ageValue = currentAge;
			}
			else
			{
				ageValue = null;
			}
		}

		if (dateOfBirth == ValueProviderResult.None)
		{
			// DateOfBirth가 없으면 Age로 역산해서 채움 (오늘 날짜 기준 -Age년 전으로 가정)
			dateOfBirthValue = ageValue.HasValue ? DateTime.Today.AddYears(-ageValue.Value) : null;
		}

		// 이 바인더는 Person 파라미터 전체를 대신 바인딩하므로 나머지 프로퍼티도 여기서 채워야 함
		var person = new Person
		{
			// Required 문자열 프로퍼티는 값이 없으면 빈 문자열로 채우고, 실제 필수 여부 검증은
			// Person에 이미 붙어 있는 [Required] 등 DataAnnotations가 처리하도록 맡김
			Name = GetValue<string>(bindingContext, nameof(Person.Name)) ?? string.Empty,
			Email = GetValue<string>(bindingContext, nameof(Person.Email)) ?? string.Empty,
			Phone = GetValue<string>(bindingContext, nameof(Person.Phone)),
			Password = GetValue<string>(bindingContext, nameof(Person.Password)) ?? string.Empty,
			ConfirmPassword = GetValue<string>(bindingContext, nameof(Person.ConfirmPassword)) ?? string.Empty,
			Price = GetValue<decimal>(bindingContext, nameof(Person.Price)),
			Age = ageValue,
			DateOfBirth = dateOfBirthValue,
			FromDate = GetValue<DateTime>(bindingContext, nameof(Person.FromDate)),
			ToDate = GetValue<DateTime>(bindingContext, nameof(Person.ToDate)),
		};

		bindingContext.Result = ModelBindingResult.Success(person);
		return Task.CompletedTask;
	}

	// string, int, decimal, DateTime 등 IParsable<T>를 구현하는 타입이면 모두 재사용 가능한 공용 파싱 로직
	// (string도 .NET 7부터 IParsable<string>을 구현하므로 값 타입 전용 struct 제약 없이 그대로 재사용 가능)
	private static T? GetValue<T>(ModelBindingContext bindingContext, string key)
		where T : IParsable<T>
	{
		var result = bindingContext.ValueProvider.GetValue(key);
		return ParseValue<T>(result);
	}

	private static T? ParseValue<T>(ValueProviderResult result)
		where T : IParsable<T>
	{
		return result != ValueProviderResult.None && T.TryParse(result.FirstValue, null, out var value)
			? value
			: default;
	}
}
