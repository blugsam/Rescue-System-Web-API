using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Application.Mapping.Bracelets;

namespace RescueSystem.Application.Mapping;

public static class AlertMapping
{
    public static Alert ToEntity(this CreateAlertRequestDto dto)
    {
        return new Alert
        {
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
        };
    }

    public static AlertDetailsDto ToDetailsDto(this Alert entity)
    {
        return new AlertDetailsDto
        {
            Id = entity.Id,
            Timestamp = entity.Timestamp,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Status = entity.Status.ToString(),
            QualityLevel = entity.QualityLevel.ToString(),
            ValidationErrors = entity.ValidationErrors.Select(ve => ve.ErrorMessage).ToList(),
            Triggers = entity.Triggers.Select(t => t.ToDto()).ToList(),
            Bracelet = entity.Bracelet?.ToDetailsDto(),
            HealthMetrics = entity.HealthMetric?.ToDto()
        };
    }

    public static AlertSummaryDto ToSummaryDto(this Alert entity)
    {
        return new AlertSummaryDto
        {
            Id = entity.Id,
            Timestamp = entity.Timestamp,
            Status = entity.Status.ToString(),
            QualityLevel = entity.QualityLevel.ToString(),
            Triggers = entity.Triggers?.Select(t => t.ToDto()).ToList() ?? new List<AlertTriggerDto>(),
            UserFullName = entity.Bracelet?.User?.FullName,
            BraceletSerialNumber = entity.Bracelet?.SerialNumber
        };
    }

    public static AlertTriggerDto ToDto(this AlertTrigger entity)
    {
        return new AlertTriggerDto
        {
            Type = entity.Type.ToString()
        };
    }

    public static HealthMetric ToEntity(this HealthMetricsRequestDto dto)
    {
        return new HealthMetric
        {
            Pulse = dto.Pulse,
            BodyTemperature = dto.BodyTemperature,
        };
    }

    public static HealthMetricsDto? ToDto(this HealthMetric? entity)
    {

        return new HealthMetricsDto
        {
            Pulse = entity.Pulse,
            BodyTemperature = entity.BodyTemperature,
        };
    }
}
