namespace RescueSystem.Domain.Entities.Alerts;

public class AlertValidationError
{
    public Guid Id { get; set; }
    public string? ErrorMessage { get; set; }

    public Guid AlertId { get; set; }
    public Alert Alert { get; set; } = null!;
}
