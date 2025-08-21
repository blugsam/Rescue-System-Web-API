namespace RescueSystem.Application.Commands;

public record RegisterUserCommand(string FullName, DateOnly DateOfBirth, string? MedicalNotes,
    string? EmergencyContact);