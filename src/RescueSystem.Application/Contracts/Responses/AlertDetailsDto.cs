namespace RescueSystem.Application.Contracts.Responses;

public class AlertDetailsDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Status { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public IEnumerable<AlertTriggerDto> Triggers { get; set; } = Array.Empty<AlertTriggerDto>();

    public BraceletDetailsDto Bracelet { get; set; } = null!;
    public HealthMetricsDto HealthMetrics { get; set; } = null!;

    public string QualityLevel { get; set; } = null!;
    public IEnumerable<string> ValidationErrors { get; set; } = Array.Empty<string>();
}
