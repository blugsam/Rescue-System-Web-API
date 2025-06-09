namespace RescueSystem.Domain.Entities.Health;

public class HealthProfileThresholds
{
    public Guid Id { get; set; }
    public string ProfileName { get; set; } = "Default";

    public double? HighPulseThreshold { get; set; }
    public double? LowPulseThreshold { get; set; }
    public double? HighTempThreshold { get; set; }
    public double? LowTempThreshold { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
