namespace RescueSystem.Application.Contracts.Responses;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContact { get; set; }
}
