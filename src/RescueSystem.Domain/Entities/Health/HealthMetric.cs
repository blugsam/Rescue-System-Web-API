namespace RescueSystem.Domain.Entities.Health;

public class HealthMetric
{
    public Guid Id { get; private set; }
    public Guid BraceletId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public int? HeartRate { get; private set; }
    public double? BodyTemperature { get; private set; }

    public HealthMetric() 
    {

    }

    private HealthMetric(Guid id, Guid braceletId, DateTime createdAt, int? heartRate, double? bodyTemperature)
    {
        Id = id;
        BraceletId = braceletId;
        CreatedAt = createdAt;
        HeartRate = heartRate;
        BodyTemperature = bodyTemperature;
    }

    public static HealthMetric Create(Guid braceletId, int? heartRate, double? bodyTemperature)
    {
        return new HealthMetric(Guid.NewGuid(), braceletId, DateTime.UtcNow, heartRate, bodyTemperature);
    }
}
