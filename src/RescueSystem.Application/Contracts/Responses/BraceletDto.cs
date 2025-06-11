namespace RescueSystem.Application.Contracts.Responses;

public class BraceletDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
}
