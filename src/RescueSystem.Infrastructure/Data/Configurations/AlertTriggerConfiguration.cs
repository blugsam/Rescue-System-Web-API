using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class AlertTriggerConfiguration : IEntityTypeConfiguration<AlertTrigger>
{
    public void Configure(EntityTypeBuilder<AlertTrigger> builder)
    {
        builder.ToTable($"{nameof(AlertTrigger)}s");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
               .HasConversion<string>()
               .IsRequired();

        builder.HasIndex(t => new { t.AlertId, t.Type })
               .IsUnique();
    }
}
