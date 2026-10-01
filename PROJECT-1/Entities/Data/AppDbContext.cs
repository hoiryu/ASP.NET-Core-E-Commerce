using System.Text.Json;
using Entities.Modules.Countries;
using Entities.Modules.Users;
using Microsoft.EntityFrameworkCore;

namespace Entities.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Country> Countries { get; set; }

	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Country>();
		modelBuilder.Entity<User>();

		string countriesJsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeds", "countries.json");
		string countriesJsonData = File.ReadAllText(countriesJsonPath);
		string usersJsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeds", "users.json");
		string usersJsonData = File.ReadAllText(usersJsonPath);
		List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJsonData) ?? [];
		List<User> users = JsonSerializer.Deserialize<List<User>>(usersJsonData) ?? [];

		// Seed to Countries
		modelBuilder.Entity<Country>().HasData(countries);
		modelBuilder.Entity<User>().HasData(users);
	}
}
