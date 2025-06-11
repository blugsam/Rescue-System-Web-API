namespace RescueSystem.Domain.Entities.Alerts;

public enum AlertProcessingStatus
{
    New,
    Acknowledged,
    InProgress,
    Resolved,
    FalseAlarm
}