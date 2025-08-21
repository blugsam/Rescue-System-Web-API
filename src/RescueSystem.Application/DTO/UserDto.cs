namespace RescueSystem.Application.DTO;

public record UserDto(Guid Id, string FullName, DateOnly DateOfBirth,
    DateTime RegistrationDate, string? MedicalNotes, string? EmergencyContact, Guid? BraceletId);
