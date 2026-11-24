using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManager.Domain.Modules.Identity.Countries;

namespace PersonManager.Infrastructure.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Name).HasMaxLength(100);

		builder.HasIndex(c => c.Name).IsUnique();
	}
}
