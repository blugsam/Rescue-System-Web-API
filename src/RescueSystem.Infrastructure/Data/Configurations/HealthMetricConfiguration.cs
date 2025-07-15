using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class HealthMetricConfiguration : IEntityTypeConfiguration<HealthMetric>
{
    private const int MinPulse = 30;
    private const int MaxPulse = 250;
    private const double MinTemperature = 30.0;
    private const double MaxTemperature = 45.0;

    public void Configure(EntityTypeBuilder<HealthMetric> builder)
    {
        builder.ToTable($"{nameof(HealthMetric)}s");

        builder.HasKey(hm => hm.Id);

        builder.Property(hm => hm.AlertId)
               .IsRequired();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_HealthMetric_Pulse",
                $"\"Pulse\" IS NULL OR (\"Pulse\" >= {MinPulse} AND \"Pulse\" <= {MaxPulse})");
            t.HasCheckConstraint("CK_HealthMetric_Temp",
                $"\"BodyTemperature\" IS NULL OR (\"BodyTemperature\" >= {MinTemperature} AND \"BodyTemperature\" <= {MaxTemperature})");
        });
    }
}