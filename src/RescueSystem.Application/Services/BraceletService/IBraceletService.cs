using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Services.BraceletService;

public interface IBraceletService
{
    Task<BraceletDetailsDto> CreateBraceletAsync(CreateBraceletRequestDto request);
    Task<BraceletDetailsDto?> GetBraceletByIdAsync(Guid braceletId);
    Task<PagedResult<BraceletDto>> GetAllBraceletsAsync(PaginationQueryParameters queryParams);
    Task UpdateBraceletStatusAsync(Guid braceletId, UpdateBraceletRequestDto request);
    Task DeleteBraceletAsync(Guid braceletId);
    Task AssignUserToBraceletAsync(Guid braceletId, Guid userId);
    Task UnassignUserFromBraceletAsync(Guid braceletId);
}