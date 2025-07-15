using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable($"{nameof(Alert)}s");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(a => a.QualityLevel)
               .HasConversion<string>()
               .IsRequired();

        builder.HasMany(a => a.ValidationErrors)
               .WithOne(ve => ve.Alert)
               .HasForeignKey(ve => ve.AlertId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
