using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable($"{nameof(User)}s");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(u => u.DateOfBirth)
               .IsRequired();

        builder.HasOne(u => u.Bracelet)
               .WithOne(b => b.User)
               .HasForeignKey<Bracelet>(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.HealthProfile)
               .WithMany(h => h.Users)
               .HasForeignKey(u => u.HealthProfileId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}