using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class HealthProfileThresholdsConfiguration : IEntityTypeConfiguration<HealthProfileThresholds>
{
    private static readonly Guid DefaultProfileId = new("a66ca370-bf93-4c12-be9a-086b98136eea");
    private static readonly Guid HypertensiveProfileId = new("c1464740-bca9-4289-8415-2e8eadf6d623");
    private static readonly Guid AthleteProfileId = new("4d74cc16-2a76-4772-9643-e82cb122c898");

    public void Configure(EntityTypeBuilder<HealthProfileThresholds> builder)
    {
        builder.ToTable($"{nameof(HealthProfileThresholds)}s");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ProfileName)
              .IsRequired()
              .HasMaxLength(100);

        builder.HasIndex(h => h.ProfileName)
              .IsUnique();

        builder.HasData(
            new HealthProfileThresholds
            {
                Id = DefaultProfileId,
                ProfileName = "Default",
                HighPulseThreshold = 160,
                LowPulseThreshold = 50,
                HighTempThreshold = 38.5,
                LowTempThreshold = 35.0
            },
            new HealthProfileThresholds
            {
                Id = HypertensiveProfileId,
                ProfileName = "Hypertensive",
                HighPulseThreshold = 140,
                LowPulseThreshold = 55,
                HighTempThreshold = 38.5,
                LowTempThreshold = 35.0
            },
            new HealthProfileThresholds
            {
                Id = AthleteProfileId,
                ProfileName = "Athlete",
                HighPulseThreshold = 180,
                LowPulseThreshold = 40,
                HighTempThreshold = 38.5,
                LowTempThreshold = 35.0
            }
        );
    }
}