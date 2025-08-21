using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Application.DTO;

public record BraceletDto(Guid Id, string SerialNumber, DateTime RegistrationDate, DateTime? LastRepairDate, BraceletStatus Status, Guid? UserId);
