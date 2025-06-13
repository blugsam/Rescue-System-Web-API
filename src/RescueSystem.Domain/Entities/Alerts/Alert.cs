using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Domain.Entities.Alerts;

public class Alert
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public ICollection<AlertTrigger> Triggers { get; set; } = new List<AlertTrigger>();
    public AlertProcessingStatus Status { get; set; } = AlertProcessingStatus.New;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Guid? BraceletId { get; set; }
    public Bracelet? Bracelet { get; set; } = null!;
    public HealthMetric? HealthMetric { get; set; }

    public AlertQualityLevel QualityLevel { get; set; }
    public ICollection<AlertValidationError> ValidationErrors { get; set; } = new List<AlertValidationError>();
}