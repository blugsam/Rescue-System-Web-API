namespace RescueSystem.Application.Contracts.Requests
{
    public class AlertDetailsDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Status { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IEnumerable<string> Triggers { get; set; } = Array.Empty<string>();
        public BraceletDetailsDto Bracelet { get; set; } = null!;
        public HealthMetricsDto HealthMetrics { get; set; } = null!;
    }
}
