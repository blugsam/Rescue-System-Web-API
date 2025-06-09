using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Domain.Entities.Health;

public class HealthMetric
{
    public Guid Id { get; set; }
    public double? Pulse { get; set; }
    public double? BodyTemperature { get; set; }

    public Guid? AlertId { get; set; }
    public Alert? Alert { get; set; }
}