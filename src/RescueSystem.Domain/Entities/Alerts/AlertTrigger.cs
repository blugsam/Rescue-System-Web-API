namespace RescueSystem.Domain.Entities.Alerts
{
    public class AlertTrigger
    {
        public Guid Id { get; set; }
        public AlertType Type { get; set; }

        public Guid AlertId { get; set; }
        public Alert Alert { get; set; } = null!;
    }
}
