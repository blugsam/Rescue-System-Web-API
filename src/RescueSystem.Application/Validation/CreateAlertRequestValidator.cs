using FluentValidation;
using RescueSystem.Contracts.Contracts.Requests;

namespace RescueSystem.Application.Validation;

public class CreateAlertRequestValidator : AbstractValidator<CreateAlertRequest>
{
    public CreateAlertRequestValidator()
    {
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Серийный номер обязателен.")
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Серийный номер содержит недопустимые символы.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0).WithMessage("Широта должна быть в диапазоне от -90 до 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0).WithMessage("Долгота должна быть в диапазоне от -180 до 180.");

        RuleFor(x => x.HealthMetrics)
            .NotNull()
            .WithMessage("Показатели здоровья обязательны для автоматических сигналов тревоги.")
            .When(x => !x.IsSosSignal);

        RuleFor(x => x.HealthMetrics)
            .SetValidator(new HealthMetricsRequestValidator()!)
            .When(x => x.HealthMetrics is not null);
    }
}