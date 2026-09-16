using DependencyInjection.Api.Options;

namespace DependencyInjection.Api.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddValidatedOptions<ClientApiOptions>(configuration, ClientApiOptions.SectionName);

		return services;
	}
}
