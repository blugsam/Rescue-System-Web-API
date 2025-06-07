namespace RescueSystem.Application.Contracts.Requests
{
    public class CreateAlertRequest
    {
        public string SerialNumber { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsSosSignal { get; set; } = false;
        public HealthMetricsRequestDto? HealthMetrics { get; set; }
    }
}
