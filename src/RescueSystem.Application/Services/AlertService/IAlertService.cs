using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Common;

namespace RescueSystem.Application.Services.AlertService;

public interface IAlertService
{
    Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequestDto request);
    Task<PagedResult<AlertSummaryDto>> GetAllAlertsSummaryAsync(PaginationQueryParameters queryParams);
    Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id);
    Task DeleteAlertAsync(Guid alertId);
}