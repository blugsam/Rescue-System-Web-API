using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Application.Contracts;

public interface IHealthMetricRepository
{
    Task AddAsync(HealthMetric metric, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<HealthMetric> Items, int TotalCount)> GetPagedByBraceletIdAsync(Guid braceletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}