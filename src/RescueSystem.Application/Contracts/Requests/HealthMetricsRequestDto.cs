namespace RescueSystem.Application.Contracts.Requests
{
    public class HealthMetricsRequestDto
    {
        public double? Pulse { get; set; }
        public double? BodyTemperature { get; set; }
    }
}
