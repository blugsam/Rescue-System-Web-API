using RescueSystem.Contracts.Contracts.Responses;

namespace RescueSystem.Application.Interfaces;

public interface IAlertNotifier
{
    Task NotifyNewAlertAsync(AlertSummaryDto alertSummary);
}