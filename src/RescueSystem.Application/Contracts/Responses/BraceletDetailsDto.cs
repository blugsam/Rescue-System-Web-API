namespace RescueSystem.Application.Contracts.Responses;

public class BraceletDetailsDto : BraceletDto
{
    public UserSummaryDto? AssignedUser { get; set; }

}