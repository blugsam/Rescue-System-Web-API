namespace RescueSystem.Application.Contracts.Requests;

public class CreateUserRequest
{
    public string FullName { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContact { get; set; }
}
