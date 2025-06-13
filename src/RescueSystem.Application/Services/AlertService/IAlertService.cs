using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Services.AlertService;

public interface IAlertService
{
    Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequest request);
    Task<PagedResult<AlertSummaryDto>> GetAllAlertsSummaryAsync(PaginationQueryParameters queryParams);
    Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id);
    Task DeleteAlertAsync(Guid alertId);
}