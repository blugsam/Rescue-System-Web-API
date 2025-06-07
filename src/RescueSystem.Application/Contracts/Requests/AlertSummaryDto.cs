namespace RescueSystem.Application.Contracts.Requests
{
    public class AlertSummaryDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Status { get; set; } = null!;
        public IEnumerable<string> Triggers { get; set; } = Array.Empty<string>();
        public string UserFullName { get; set; } = null!;
        public string BraceletSerialNumber { get; set; } = null!;
    }
}
