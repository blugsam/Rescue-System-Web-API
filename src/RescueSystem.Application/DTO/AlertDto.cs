using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Application.DTO;

public record AlertDto(Guid Id, DateTime CreatedAt, double Latitude, 
    double Longitude, AlertProcessingStatus Status, AlertQualityLevel QualityLevel, 
    Guid? BraceletId, byte? HeartRate = null, short? BodyTemperature = null);
