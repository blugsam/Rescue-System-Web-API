using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Infrastructure.Data.Configurations;

public class AlertValidationErrorConfiguration : IEntityTypeConfiguration<AlertValidationError>
{
    public void Configure(EntityTypeBuilder<AlertValidationError> builder)
    {
        builder.ToTable($"{nameof(AlertValidationError)}s");

        builder.HasKey(ve => ve.Id);

        builder.Property(ve => ve.ErrorMessage).IsRequired();
    }
}