using PersonManager.Entities.Modules.Countries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PersonManager.Entities.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Name).HasMaxLength(100);

		builder.HasIndex(c => c.Name).IsUnique();
	}
}
