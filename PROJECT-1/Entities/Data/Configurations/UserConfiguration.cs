using Entities.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(u => u.Id);

		builder.Property(u => u.Name).HasMaxLength(40);
		builder.Property(u => u.Email).HasMaxLength(40);
		builder.Property(u => u.Gender).HasMaxLength(10);
		builder.Property(u => u.Address).HasMaxLength(200);

		builder.HasIndex(u => u.Email).IsUnique();

		builder.HasOne(u => u.Country).WithMany().HasForeignKey(u => u.CountryId);
	}
}
