namespace RescueSystem.Application.Contracts.Responses;

public class AlertSummaryDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Status { get; set; } = null!;
    public IEnumerable<AlertTriggerDto> Triggers { get; set; } = Array.Empty<AlertTriggerDto>();

    public string UserFullName { get; set; } = null!;
    public string BraceletSerialNumber { get; set; } = null!;
}
