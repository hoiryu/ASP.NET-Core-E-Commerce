namespace DependencyInjection.Api.Extensions;

public static class OptionsRegistrationExtensions
{
	/// <summary>
	/// 설정 섹션을 <typeparamref name="TOptions"/> 에 바인딩하고,
	/// DataAnnotations 검증을 앱 시작 시점(ValidateOnStart)에 수행한다.
	/// 옵션이 늘어날 때 이 헬퍼를 재사용해 바인딩 보일러플레이트를 제거한다.
	/// </summary>
	public static IServiceCollection AddValidatedOptions<T>(
		this IServiceCollection services,
		IConfiguration configuration,
		string sectionName
	)
		where T : class
	{
		services
			.AddOptions<T>()
			.Bind(configuration.GetSection(sectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		return services;
	}
}
