using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Domain.Entities
{
    public class HealthMetric
    {
        public Guid Id { get; set; }
        public double? Pulse { get; set; }
        public double? BodyTemperature { get; set; }

        public Guid AlertId { get; set; }
        public Alert Alert { get; set; } = null!;
    }
}