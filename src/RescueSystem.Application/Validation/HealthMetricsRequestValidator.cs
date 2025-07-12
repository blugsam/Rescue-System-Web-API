using FluentValidation;
using RescueSystem.Contracts.Contracts.Requests;

namespace RescueSystem.Application.Validation;

public class HealthMetricsRequestValidator : AbstractValidator<HealthMetricsRequestDto>
{
    public HealthMetricsRequestValidator()
    {
        RuleFor(x => x.Pulse)
            .InclusiveBetween(40, 220)
            .When(x => x.Pulse.HasValue)
            .WithMessage("Pulse value is invalid.");

        RuleFor(x => x.BodyTemperature)
            .InclusiveBetween(33, 42)
            .When(x => x.BodyTemperature.HasValue)
            .WithMessage("Body temperature value is invalid.");

        RuleFor(x => x)
            .Must(metrics => metrics.Pulse.HasValue || metrics.BodyTemperature.HasValue)
            .WithMessage("If HealthMetrics is provided, it must contain at least one measurement.");
    }
}