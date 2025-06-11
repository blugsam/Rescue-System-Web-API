namespace RescueSystem.Application.Contracts.Responses;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
}
