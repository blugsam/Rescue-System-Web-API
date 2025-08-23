using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.DTO;

namespace RescueSystem.Application.Services.AlertService;

public class AlertQueries(IApplicationDbContext dbContext)
{
    public async Task<AlertDto?> GetAlertByIdAsync(Guid bracletId, CancellationToken cancellationToken = default)
    {
        var alertDto = await dbContext.Alerts
            .Where(u =>  u.Id == bracletId)
            .Select(alert => new AlertDto(
                alert.Id,
                alert.CreatedAt,
                alert.Latitude,
                alert.Longitude,
                alert.Status,
                alert.QualityLevel,
                alert.BraceletId,
                alert.HeartRate,
                alert.BodyTemperature))
            .FirstOrDefaultAsync(cancellationToken);

        return alertDto;
    }

    public async Task<(IReadOnlyList<AlertDto> Items, int TotalCount)> GetPagedAlertsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await dbContext.Alerts.CountAsync(cancellationToken);

        var alertDtos = await dbContext.Alerts
            .OrderBy(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(alert => new AlertDto(
                alert.Id,
                alert.CreatedAt,
                alert.Latitude,
                alert.Longitude,
                alert.Status,
                alert.QualityLevel,
                alert.BraceletId,
                alert.HeartRate,
                alert.BodyTemperature))
            .ToListAsync(cancellationToken);

        return (alertDtos, totalCount);
    }
}
