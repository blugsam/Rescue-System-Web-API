using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;

namespace RescueSystem.Application.Interfaces
{
    public interface IAlertService
    {
        Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequest request);
    }
}