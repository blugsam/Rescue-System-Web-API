using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class BraceletConfiguration : IEntityTypeConfiguration<Bracelet>
{
    public void Configure(EntityTypeBuilder<Bracelet> builder)
    {
        builder.ToTable($"{nameof(Bracelet)}s");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.SerialNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(b => b.SerialNumber)
               .IsUnique();

        builder.Property(b => b.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.HasMany(b => b.Alerts)
               .WithOne(a => a.Bracelet)
               .HasForeignKey(a => a.BraceletId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}