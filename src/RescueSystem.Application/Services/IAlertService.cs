using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;

namespace RescueSystem.Application.Services;

public interface IAlertService
{
    Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequest request);
    Task<IEnumerable<AlertSummaryDto>> GetAllAlertsSummaryAsync();
    Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id);

}