using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Domain.Entities.Alerts;

public class Alert
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Guid? BraceletId { get; private set; }
    public AlertProcessingStatus Status { get; private set; }
    public AlertQualityLevel QualityLevel { get; private set; }

    public int? HeartRate { get; private set; }
    public double? BodyTemperature { get; private set; }

    public Alert()
    {

    }

    private Alert(Guid id, DateTime createdAt, double latitude,
        double longitude, AlertProcessingStatus status, AlertQualityLevel qualityLevel,
        Guid? braceletId, int? heartRate, double? bodyTemperature)
    {
        Id = id;
        CreatedAt = createdAt;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        QualityLevel = qualityLevel;
        BraceletId = braceletId;
        HeartRate = heartRate;
        BodyTemperature = bodyTemperature;
    }

    public static Alert Create(double latitude, double longitude, AlertProcessingStatus status,
        AlertQualityLevel qualityLevel, Guid? braceletId, int? heartRate, double? bodyTemperature)
    {
        return new Alert(Guid.NewGuid(), DateTime.UtcNow, latitude,
            longitude, AlertProcessingStatus.New,
            qualityLevel, braceletId, heartRate, bodyTemperature);
    }

    public void ChangeProcessingStatus(AlertProcessingStatus newStatus)
    {
        if (newStatus == Status)
            return;

        if (Status == AlertProcessingStatus.Resolved || Status == AlertProcessingStatus.FalseAlarm)
            throw new InvalidOperationException("Cannot change status after alert is completed.");

        if (Status == AlertProcessingStatus.New && newStatus == AlertProcessingStatus.Resolved)
            throw new InvalidOperationException("Alert cannot be resolved without being processed.");

        Status = newStatus;
    }

    public void ChangeAlertQualityLevel(AlertQualityLevel newQualityLevel)
    {
        if (newQualityLevel == QualityLevel)
            return;

        QualityLevel = newQualityLevel;
    }
}