using Microsoft.AspNetCore.SignalR;
using RescueSystem.Api.Hubs;
using RescueSystem.Application.Interfaces;
using RescueSystem.Contracts.Contracts.Responses;

namespace RescueSystem.Api.Services
{
    public class SignalRAlertNotifier : IAlertNotifier
    {
        private readonly IHubContext<AlertHub> _hubContext;

        public SignalRAlertNotifier(IHubContext<AlertHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewAlertAsync(AlertSummaryDto alertSummary)
        {
            await _hubContext.Clients.All.SendAsync("NewAlertReceived", alertSummary);
        }
    }
}