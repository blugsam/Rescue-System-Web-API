using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Application.DTO;

public record BraceletDto(Guid id, string serialNumber, DateTime registrationDate, DateTime? lastRepairDate, BraceletStatus status, Guid? userId);
