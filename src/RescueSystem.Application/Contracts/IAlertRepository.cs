using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Application.Contracts;

public interface IAlertRepository : IGenericRepository<Alert>
{
    Task<(IReadOnlyList<Alert> Items, int TotalCount)> GetPagedByBraceletIdAsync(Guid braceletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}