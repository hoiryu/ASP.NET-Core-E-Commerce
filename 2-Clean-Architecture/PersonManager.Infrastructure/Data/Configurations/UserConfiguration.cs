using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(u => u.Id);

		builder.Property(u => u.Name).HasMaxLength(40);
		builder.Property(u => u.Email).HasMaxLength(40);
		builder.Property(u => u.Gender).HasConversion<string>().HasMaxLength(10); // enum 을 이름 문자열(Male 등)로 저장
		builder.Property(u => u.Address).HasMaxLength(200);

		builder.HasIndex(u => u.Email).IsUnique();

		// Country 삭제 시 참조하는 User 의 CountryId 를 DB 가 null 로 변경
		builder.HasOne(u => u.Country).WithMany().HasForeignKey(u => u.CountryId).OnDelete(DeleteBehavior.SetNull);
	}
}
