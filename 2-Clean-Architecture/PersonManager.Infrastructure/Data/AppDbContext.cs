using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PersonManager.Domain.Modules.Identity.Countries;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Country> Countries { get; set; }

	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

		string countriesJsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeds", "countries.json");
		string countriesJsonData = File.ReadAllText(countriesJsonPath);

		string usersJsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeds", "users.json");
		string usersJsonData = File.ReadAllText(usersJsonPath);

		// Gender 등 enum 을 문자열("Male")로 읽음
		JsonSerializerOptions jsonOptions = new() { Converters = { new JsonStringEnumConverter() } };

		List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJsonData, jsonOptions) ?? [];
		List<User> users = JsonSerializer.Deserialize<List<User>>(usersJsonData, jsonOptions) ?? [];

		// Seed to countries
		modelBuilder.Entity<Country>().HasData(countries);
		// Seed to users
		modelBuilder.Entity<User>().HasData(users);
	}
}
