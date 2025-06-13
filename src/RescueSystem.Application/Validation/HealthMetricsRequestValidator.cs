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
            .WithMessage("Значение пульса некорректно.");

        RuleFor(x => x.BodyTemperature)
            .InclusiveBetween(33, 42)
            .When(x => x.BodyTemperature.HasValue)
            .WithMessage("Значение температуры тела некорректно.");

        RuleFor(x => x)
            .Must(metrics => metrics.Pulse.HasValue || metrics.BodyTemperature.HasValue)
            .WithMessage("Если блок HealthMetrics передан, он должен содержать хотя бы один показатель.");
    }
}