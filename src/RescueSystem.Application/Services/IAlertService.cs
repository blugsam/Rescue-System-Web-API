using RescueSystem.Application.Contracts.Requests;

namespace RescueSystem.Application.Services
{
    public interface IAlertService
    {
        Task ProcessIncomingSignal(CreateAlertRequest request);
    }
}
