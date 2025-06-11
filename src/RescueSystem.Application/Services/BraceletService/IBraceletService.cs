using RescueSystem.Application.Contracts;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Services.BraceletService;

public interface IBraceletService
{
    Task<BraceletDetailsDto> CreateBraceletAsync(CreateBraceletRequest request);
    Task<BraceletDetailsDto?> GetBraceletByIdAsync(Guid braceletId);
    Task<PagedResult<BraceletDto>> GetAllBraceletsAsync(PaginationQueryParameters queryParams);
    Task UpdateBraceletStatusAsync(Guid braceletId, UpdateBraceletRequest request);
    Task DeleteBraceletAsync(Guid braceletId);
    Task AssignUserToBraceletAsync(Guid braceletId, Guid userId);
    Task UnassignUserFromBraceletAsync(Guid braceletId);
}