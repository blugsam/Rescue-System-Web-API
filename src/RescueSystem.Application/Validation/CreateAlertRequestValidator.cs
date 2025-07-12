using FluentValidation;
using RescueSystem.Contracts.Contracts.Requests;

namespace RescueSystem.Application.Validation;

public class CreateAlertRequestValidator : AbstractValidator<CreateAlertRequest>
{
    public CreateAlertRequestValidator()
    {
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.")
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Serial number contains invalid characters.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.HealthMetrics)
            .NotNull()
            .WithMessage("Health metrics are required for automatic alarm signals.")
            .When(x => !x.IsSosSignal);

        RuleFor(x => x.HealthMetrics)
            .SetValidator(new HealthMetricsRequestValidator()!)
            .When(x => x.HealthMetrics is not null);
    }
}